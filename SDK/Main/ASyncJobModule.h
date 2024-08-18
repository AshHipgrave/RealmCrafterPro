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
