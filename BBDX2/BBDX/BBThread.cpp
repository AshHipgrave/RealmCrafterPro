//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################
#include "BBThread.h"
#include <windows.h>
#include <Strsafe.h>
#include "DebugFrameProfiler.h"
#include "DebugOverlay.h"

// Foward declarations
DWORD WINAPI bbdx2_ASyncJobThread(VOID*);

bbdx2_FatalErrorFn FatalError;
HANDLE JobThread;
DWORD JobThreadId;
CRITICAL_SECTION CriticalSection;

struct JobInstance
{
	int Priority;
	int TypeId;
	void* UserData;
	void* Buffer;
	unsigned int BufferSize;
	bbdx2_ASyncJobFn StartAddress;
	bbdx2_ASyncJobFn SyncAddress;
	unsigned int ThreadTime;
	unsigned int SyncTime;
};

JobInstance JobQueue[64];
JobInstance CompletedJobs[64];
JobInstance LeftProc[64];

LARGE_INTEGER CounterFrequency;

// ---------------------------------------------
// IO Handler
// ---------------------------------------------

#define MAX_IO_BUFFERS 2
#define IO_BUFFERS_SIZE 1024 * 1024 * 10 // 10MB
#define MAX_IO_JOBS 1024
JobInstance IOJobQueue[MAX_IO_JOBS];
char IOPaths[MAX_IO_JOBS][256];
int CurrentIOTask = -1;

// Win32 specific IO
HANDLE CurrentIOHandle = NULL;
OVERLAPPED CurrentIOOverlapped;

// Memory buffers for read/proc operations
void* IOBuffers[MAX_IO_BUFFERS];
IOBufferUsage IOUsage[MAX_IO_BUFFERS];

#define StartTimer() \
LARGE_INTEGER mcr_StartTime, mcr_EndTime; \
QueryPerformanceCounter( &mcr_StartTime );

#define EndTimer( writeTo ) \
QueryPerformanceCounter( &mcr_EndTime ); \
writeTo = (unsigned int)((mcr_EndTime.QuadPart - mcr_StartTime.QuadPart) / CounterFrequency.QuadPart);

ASyncFuncs asyncFuncs;

extern "C" __declspec(dllexport) ASyncFuncs* bbdx2_ASyncGetLib()
{
	asyncFuncs.bbdx2_ASyncJobInit = &bbdx2_ASyncJobInit;
	asyncFuncs.bbdx2_ASyncJobTerm = &bbdx2_ASyncJobTerm;
	asyncFuncs.bbdx2_ASyncGetFreeBuffer = &bbdx2_ASyncGetFreeBuffer;
	asyncFuncs.bbdx2_ASyncMarkBuffer = &bbdx2_ASyncMarkBuffer;
	asyncFuncs.bbdx2_ASyncIOProc = &bbdx2_ASyncIOProc;
	asyncFuncs.bbdx2_ASyncIOSync = &bbdx2_ASyncIOSync;
	asyncFuncs.bbdx2_ASyncJobSync = &bbdx2_ASyncJobSync;
	asyncFuncs.bbdx2_ASyncInsertJob = &bbdx2_ASyncInsertJob;
	asyncFuncs.bbdx2_ASyncInsertIOJob = &bbdx2_ASyncInsertIOJob;
	asyncFuncs.FP_FrameSync = &FP_FrameSync;
	asyncFuncs.FP_PushTask = &FP_PushTask;
	asyncFuncs.FP_PopTask = &FP_PopTask;
	asyncFuncs.DO_Initialize = &DO_Initialize;
	asyncFuncs.DO_LostDevice = &DO_LostDevice;
	asyncFuncs.DO_ResetDevice = &DO_ResetDevice;
	asyncFuncs.DO_Render = &DO_Render;
	asyncFuncs.DO_Update = &DO_Update;

	return &asyncFuncs;
}

void bbdx2_ASyncJobInit(bbdx2_FatalErrorFn errorFunction, void* ioBuffer, unsigned int ioBufferSize)
{
	FatalError = errorFunction;

	// Check buffers
	if( ioBuffer == NULL )
		FatalError("Parameter 'ioBuffer' must be set!");

	if( ioBufferSize != IO_BUFFERS_SIZE * MAX_IO_BUFFERS )
		FatalError("Parameter 'ioBufferSize' is of an incorrect size!");
	
	JobThread = CreateThread(
		NULL,
		0,
		&bbdx2_ASyncJobThread,
		NULL,
		0,
		&JobThreadId);

	if( JobThread == NULL )
	{
		errorFunction( "Error: Could not create job thread!" );
	}

	InitializeCriticalSection( &CriticalSection );

	memset( JobQueue, 0, sizeof(JobQueue) );
	memset( CompletedJobs, 0, sizeof(CompletedJobs) );
	memset( IOJobQueue, 0, sizeof(IOJobQueue) );
	memset( IOPaths, 0, sizeof(IOPaths) );
	memset( IOBuffers, 0, sizeof(IOBuffers) );
	memset( IOUsage, 0, sizeof(IOBufferUsage) );

	// Allocate IO buffers
	for( int i = 0; i < MAX_IO_BUFFERS; ++i )
	{
		// Maybe pass in memory later?
		IOBuffers[i] = (void*)((char*)ioBuffer + (i * IO_BUFFERS_SIZE));
	}

	// Get counter frequency
	if( QueryPerformanceFrequency( &CounterFrequency ) == FALSE )
	{
		FatalError( "QueryPerformanceFrequency Failed!" ); 
	}
	CounterFrequency.QuadPart /= 1000;

	// Unset/clear jobs
	for(int i = 0; i < sizeof(JobQueue) / sizeof(JobInstance); ++i)
	{
		JobQueue[i].Priority = -1;
		CompletedJobs[i].Priority = -1;
	}

	for(int i = 0; i < sizeof(IOJobQueue) / sizeof(JobInstance); ++i)
	{
		IOJobQueue[i].Priority = -1;
	}
}

void bbdx2_ASyncJobTerm()
{
	for( int i = 0; i < MAX_IO_BUFFERS; ++i )
	{
		IOBuffers[i] = NULL;
	}

	DeleteCriticalSection( &CriticalSection );
}

void* bbdx2_ASyncGetFreeBuffer()
{
	for( int i = 0; i < MAX_IO_BUFFERS; ++i )
	{
		if( IOUsage[i] == IOBU_NONE )
			return IOBuffers[i];
	}
	
	return NULL;
}

void bbdx2_ASyncMarkBuffer( void* buffer, IOBufferUsage usage )
{
	for( int i = 0; i < MAX_IO_BUFFERS; ++i )
	{
		if( buffer == IOBuffers[i] )
		{
			IOUsage[i] = usage;
			return;
		}
	}
}

int bbdx2_ASyncIOProc( ASyncJobDesc* jobDesc )
{
	JobInstance* instance = reinterpret_cast<JobInstance*>(jobDesc->UserData);

	ASyncJobDesc subDesc;
	subDesc.Buffer = instance->Buffer;
	subDesc.BufferSize = instance->BufferSize;
	subDesc.UserData = instance->UserData;

	if( instance->StartAddress != NULL && instance->StartAddress( &subDesc ) == 0 )
	{
		instance->SyncAddress = NULL;
	}

	return 1;
}

int bbdx2_ASyncIOSync( ASyncJobDesc* jobDesc )
{
	JobInstance* instance = reinterpret_cast<JobInstance*>(jobDesc->UserData);

	ASyncJobDesc subDesc;
	subDesc.Buffer = instance->Buffer;
	subDesc.BufferSize = instance->BufferSize;
	subDesc.UserData = instance->UserData;

	if( instance->SyncAddress != NULL )
		instance->SyncAddress( &subDesc );

	bbdx2_ASyncMarkBuffer( instance->Buffer, IOBU_NONE );
	instance->Priority = -1;

	return 1;
}

DWORD WINAPI bbdx2_ASyncJobThread(VOID*)
{
	while( true )
	{
		// Find a task
		int topIdx = -1;
		int topPriority = 0;
		for(int i = 0; i < sizeof(JobQueue) / sizeof(JobInstance); ++i)
		{
			if( JobQueue[i].Priority > topPriority )
			{
				topPriority = JobQueue[i].Priority;
				topIdx = i;
			}
		}

		// Run task
		if( topIdx > -1 )
		{
			StartTimer();

			ASyncJobDesc desc;
			desc.UserData = JobQueue[topIdx].UserData;
			desc.Buffer = JobQueue[topIdx].Buffer;
			desc.BufferSize = JobQueue[topIdx].BufferSize;

			int success = JobQueue[topIdx].StartAddress( &desc );

			unsigned int ThreadTime;
			EndTimer( ThreadTime );

			// Write out
			for(int i = 0; i < sizeof(CompletedJobs) / sizeof(JobInstance); ++i)
			{
				if( CompletedJobs[i].Priority == -1 )
				{
					EnterCriticalSection( &CriticalSection );

					if( success > 0 )
					{
						memcpy( &CompletedJobs[i], &JobQueue[topIdx], sizeof(JobInstance) );

						CompletedJobs[i].ThreadTime = ThreadTime;
					}

					JobQueue[topIdx].Priority = -1;

					LeaveCriticalSection( &CriticalSection );

					break;
				}
			}

		}

		// Nap time
		Sleep( 0 );
	}
}

void bbdx2_ASyncJobSync()
{
	// Copy out sync data to reduce lock time.
	memcpy( LeftProc, CompletedJobs, sizeof(CompletedJobs) );

	EnterCriticalSection( &CriticalSection );

	for(int i = 0; i < sizeof(CompletedJobs) / sizeof(JobInstance); ++i)
	{
		CompletedJobs[i].Priority = -1;
	}

	LeaveCriticalSection( &CriticalSection );

	// Call sync functions
	for(int i = 0; i < sizeof(LeftProc) / sizeof(JobInstance); ++i)
	{
		if( LeftProc[i].Priority != -1 )
		{
			StartTimer();

			ASyncJobDesc desc;
			desc.UserData = LeftProc[i].UserData;
			desc.Buffer = LeftProc[i].Buffer;
			desc.BufferSize = LeftProc[i].BufferSize;

			LeftProc[i].SyncAddress( &desc );

			EndTimer( LeftProc[i].SyncTime );

			//char OO[128];
			//sprintf(OO, "Job Done: Thread(%i), Sync(%i)\n", LeftProc[i].ThreadTime, LeftProc[i].SyncTime);
			//OutputDebugStringA(OO);
		}
	}

	// Update IO
	if( CurrentIOTask != -1 )
	{
		DWORD BytesRead = 0;
		if( GetOverlappedResult( CurrentIOHandle, &CurrentIOOverlapped, &BytesRead, FALSE ) )
		{

			CloseHandle( CurrentIOHandle );

			bbdx2_ASyncMarkBuffer( IOJobQueue[CurrentIOTask].Buffer, IOBU_PROC );
			IOJobQueue[CurrentIOTask].BufferSize = (unsigned int)BytesRead;

			bbdx2_ASyncInsertJob( IOJobQueue[CurrentIOTask].TypeId, &bbdx2_ASyncIOProc, &bbdx2_ASyncIOSync, &IOJobQueue[CurrentIOTask], IOJobQueue[CurrentIOTask].Priority );
			IOJobQueue[CurrentIOTask].Priority = -2;

			CurrentIOTask = -1;
		}
		else
		{
			DWORD error = GetLastError();

			// Its not processing, so it failed
			if( error != ERROR_IO_PENDING )
			{
				CloseHandle(CurrentIOHandle);

				bbdx2_ASyncMarkBuffer( IOJobQueue[CurrentIOTask].Buffer, IOBU_NONE );
				IOJobQueue[CurrentIOTask].Priority = -1;

				CurrentIOTask = -1;
			}
		}

	}

	if( CurrentIOTask == -1 )
	{
		// Find a task
		int topIdx = -1;
		int topPriority = 0;
		for(int i = 0; i < sizeof(IOJobQueue) / sizeof(JobInstance); ++i)
		{
			if( IOJobQueue[i].Priority > topPriority )
			{
				topPriority = IOJobQueue[i].Priority;
				topIdx = i;
			}
		}

		// Run task
		if( topIdx > -1 )
		{
			// Check for a buffer. Only a limited number are available,
			// and we could already be processing
			void* buffer = bbdx2_ASyncGetFreeBuffer();

			if( buffer != NULL )
			{
				// Win32 specific load
				CurrentIOHandle = CreateFile( IOPaths[topIdx],
					GENERIC_READ,
					FILE_SHARE_READ,
					NULL,
					OPEN_EXISTING,
					FILE_FLAG_SEQUENTIAL_SCAN | FILE_FLAG_OVERLAPPED,
					NULL);

				// Error!
				if( GetLastError() == ERROR_FILE_NOT_FOUND )
				{
					IOJobQueue[topIdx].Priority = -1;
					CloseHandle( CurrentIOHandle );

					return;
				}

				bbdx2_ASyncMarkBuffer( buffer, IOBU_READ );

				IOJobQueue[topIdx].Buffer = buffer;
				IOJobQueue[topIdx].BufferSize = IO_BUFFERS_SIZE;

				CurrentIOTask = topIdx;

				// Read
				memset( &CurrentIOOverlapped, 0, sizeof(OVERLAPPED) );
				ReadFile( CurrentIOHandle,
					buffer,
					IO_BUFFERS_SIZE,
					(LPDWORD)&IOJobQueue[topIdx].BufferSize,
					&CurrentIOOverlapped );

			}
		}
	}
	
#if 0
	if(((GetKeyState(VK_F10) >> 16) & 0xffff) != 0)
	{
		// Dump Queue
		FILE* fp = fopen("ioqueue_dbg.txt", "w");

		EnterCriticalSection( &CriticalSection );

		if( CurrentIOTask != -1 )
		{
			fprintf(fp, "TypeId: %02x; Priority: %02x; Path: %s; StartAddress: 0x%08x; SyncAddress: 0x%08x;\n",
				IOJobQueue[CurrentIOTask].TypeId,
				IOJobQueue[CurrentIOTask].Priority,
				IOPaths[CurrentIOTask],
				IOJobQueue[CurrentIOTask].StartAddress,
				IOJobQueue[CurrentIOTask].SyncAddress);
		}
		else
		{
			fprintf(fp, "CurrentIOTask is -1\n");
		}

		for(int i = 0; i < sizeof(IOJobQueue) / sizeof(JobInstance); ++i)
		{
			if( IOJobQueue[i].Priority != -1 )
			{
				fprintf(fp, "TypeId: %02x; Priority: %02x; Path: %s; StartAddress: 0x%08x; SyncAddress: 0x%08x;\n",
					IOJobQueue[i].TypeId,
					IOJobQueue[i].Priority,
					IOPaths[i],
					IOJobQueue[i].StartAddress,
					IOJobQueue[i].SyncAddress);
			}
		}
		LeaveCriticalSection( &CriticalSection );

		fclose(fp);

		// Dump Jobs
		fp = fopen("jobqueue_dbg.txt", "w");

		EnterCriticalSection( &CriticalSection );

		for(int i = 0; i < sizeof(JobQueue) / sizeof(JobInstance); ++i)
		{
			if( JobQueue[i].Priority != -1 )
			{
				fprintf(fp, "TypeId: %02x; Priority: %02x; StartAddress: 0x%08x; SyncAddress: 0x%08x;\n",
					JobQueue[i].TypeId,
					JobQueue[i].Priority,
					JobQueue[i].StartAddress,
					JobQueue[i].SyncAddress);
			}
		}

		LeaveCriticalSection( &CriticalSection );

		fclose(fp);
	}
#endif
}

void bbdx2_ASyncInsertJob(int typeId, bbdx2_ASyncJobFn startAddress, bbdx2_ASyncJobFn syncAddress, void* userData, int priority)
{
	// Find a place and insert
	for(int i = 0; i < sizeof(JobQueue) / sizeof(JobInstance); ++i)
	{
		if( JobQueue[i].Priority == -1 )
		{
			EnterCriticalSection( &CriticalSection );

			JobQueue[i].Priority = priority;
			JobQueue[i].TypeId = typeId;
			JobQueue[i].UserData = userData;
			JobQueue[i].StartAddress = startAddress;
			JobQueue[i].SyncAddress = syncAddress;
			JobQueue[i].ThreadTime = 0;
			JobQueue[i].SyncTime = 0;

			LeaveCriticalSection( &CriticalSection );

			return;
		}
	}

	// No space, fuck!
	FatalError( "Error bbdx2_ASyncInsertJob failed: Ran out of room in the queue!" );
}

void bbdx2_ASyncInsertIOJob(const char* path, int typeId, bbdx2_ASyncJobFn startAddress, bbdx2_ASyncJobFn syncAddress, void* userData, int priority)
{
	// Find a place and insert
	for(int i = 0; i < sizeof(IOJobQueue) / sizeof(JobInstance); ++i)
	{
		if( IOJobQueue[i].Priority == -1 )
		{
			EnterCriticalSection( &CriticalSection );

			strncpy( IOPaths[i], path, 256 );
			IOJobQueue[i].Priority = priority;
			IOJobQueue[i].TypeId = typeId;
			IOJobQueue[i].UserData = userData;
			IOJobQueue[i].StartAddress = startAddress;
			IOJobQueue[i].SyncAddress = syncAddress;
			IOJobQueue[i].ThreadTime = 0;
			IOJobQueue[i].SyncTime = 0;

			LeaveCriticalSection( &CriticalSection );

			return;
		}
	}

	// No space, fuck!

	// Dump Queue
	FILE* fp = fopen("ioqueue.txt", "w");

	EnterCriticalSection( &CriticalSection );
	for(int i = 0; i < sizeof(IOJobQueue) / sizeof(JobInstance); ++i)
	{
		if( IOJobQueue[i].Priority != -1 )
		{
			fprintf(fp, "TypeId: %02x; Priority: %02x; Path: %s; StartAddress: 0x%08x; SyncAddress: 0x%08x;\n",
				IOJobQueue[i].TypeId,
				IOJobQueue[i].Priority,
				IOPaths[i],
				IOJobQueue[i].StartAddress,
				IOJobQueue[i].SyncAddress);
		}
	}
	LeaveCriticalSection( &CriticalSection );

	fclose(fp);

	FatalError( "Error bbdx2_ASyncInsertIOJob failed: Ran out of room in the queue!" );
}
