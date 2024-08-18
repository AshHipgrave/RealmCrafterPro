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
/*************************************************
* Asynchronous Task Queue
*
* Jared Belkus
*
**************************************************/

#ifndef __ASYNCJOBMODULE_H
#define __ASYNCJOBMODULE_H

//! Structure passed to Job functions
struct ASyncJobDesc
{
	//! User defined data
	void* UserData;

	//! Buffer for file IO
	void* Buffer;

	//! Size of buffer for IO (NOTE: Defines safe-readable length, not allocated length).
	unsigned int BufferSize;
};

//! Buffer usage enumeration
enum IOBufferUsage
{
	//! Buffer is not used.
	IOBU_NONE,

	//! Buffer is part of an IO read operation.
	IOBU_READ,

	//! Buffer is part of an IO proc operation.
	IOBU_PROC
};


typedef void (*bbdx2_FatalErrorFn)(const char* message);
typedef int (*bbdx2_ASyncJobFn)(ASyncJobDesc* jobDesc);

typedef void (bbdx2_ASyncJobInitfn)(bbdx2_FatalErrorFn errorFunction, void* ioBuffer, unsigned int ioBufferSize);
typedef void (bbdx2_ASyncJobTermfn)();
typedef void* (bbdx2_ASyncGetFreeBufferfn)();
typedef void (bbdx2_ASyncMarkBufferfn)( void* buffer, IOBufferUsage usage );
typedef int (bbdx2_ASyncIOProcfn)( ASyncJobDesc* jobDesc );
typedef int (bbdx2_ASyncIOSyncfn)( ASyncJobDesc* jobDesc );
typedef void (bbdx2_ASyncJobSyncfn)();
typedef void (bbdx2_ASyncInsertJobfn)(int typeId, bbdx2_ASyncJobFn startAddress, bbdx2_ASyncJobFn syncAddress, void* userData, int priority);
typedef void (bbdx2_ASyncInsertIOJobfn)(const char* path, int typeId, bbdx2_ASyncJobFn startAddress, bbdx2_ASyncJobFn syncAddress, void* userData, int priority);
typedef void (FP_FrameSyncfn)();
typedef void (FP_PushTaskfn)( const char* name );
typedef void (FP_PopTaskfn)( );
typedef void (DO_Initializefn)(void* device, int width, int height);
typedef void (DO_LostDevicefn)();
typedef void (DO_ResetDevicefn)();
typedef void (DO_Renderfn)();
typedef void (DO_Updatefn)();

struct ASyncFuncs
{
	bbdx2_ASyncJobInitfn* bbdx2_ASyncJobInit;
	bbdx2_ASyncJobTermfn* bbdx2_ASyncJobTerm;
	bbdx2_ASyncGetFreeBufferfn* bbdx2_ASyncGetFreeBuffer;
	bbdx2_ASyncMarkBufferfn* bbdx2_ASyncMarkBuffer;
	bbdx2_ASyncIOProcfn* bbdx2_ASyncIOProc;
	bbdx2_ASyncIOSyncfn* bbdx2_ASyncIOSync;
	bbdx2_ASyncJobSyncfn* bbdx2_ASyncJobSync;
	bbdx2_ASyncInsertJobfn* bbdx2_ASyncInsertJob;
	bbdx2_ASyncInsertIOJobfn* bbdx2_ASyncInsertIOJob;
	FP_FrameSyncfn* FP_FrameSync;
	FP_PushTaskfn* FP_PushTask;
	FP_PopTaskfn* FP_PopTask;
	DO_Initializefn* DO_Initialize;
	DO_LostDevicefn* DO_LostDevice;
	DO_ResetDevicefn* DO_ResetDevice;
	DO_Renderfn* DO_Render;
	DO_Updatefn* DO_Update;
};

#endif // __ASYNCJOBMODULE_H
