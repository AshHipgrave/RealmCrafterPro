/*************************************************
* Asynchronous Task Queue
*
* Jared Belkus (jared.belkus@solstargames.com)
*
**************************************************/

#ifndef __ASYNCJOBQUEUE_H
#define __ASYNCJOBQUEUE_H

#include "ASyncJobModule.h"

typedef void (*bbdx2_FatalErrorFn)(const char* message);
typedef int (*bbdx2_ASyncJobFn)(ASyncJobDesc* jobDesc);

//! Get all of the ASync* functions in a structure.
extern "C" __declspec(dllexport) ASyncFuncs* bbdx2_ASyncGetLib();

//! Initialize ASynchronous library
/*! Note: ioBuffer and its size must strictly match the internal size (20MB).
\param errorFunction Pointer to fatal error handling function.
\param ioBuffer Pointer to pre-allocated memory for IO Buffers.
\param ioBufferSize Size of ioBuffer.
*/
void bbdx2_ASyncJobInit(bbdx2_FatalErrorFn errorFunction, void* ioBuffer, unsigned int ioBufferSize);

//! Shutdown and free all ASync resources
void bbdx2_ASyncJobTerm();

//! Get a free buffer from the IO Buffer pool
void* bbdx2_ASyncGetFreeBuffer();

//! Mark an IO Buffer with its current usage
/*!
\param buffer Pointer to buffer (This must exist in the IO Buffer Pool).
\param usage New usage type for the buffer.
*/
void bbdx2_ASyncMarkBuffer( void* buffer, IOBufferUsage usage );

//! Handler for IO Proc
int bbdx2_ASyncIOProc( ASyncJobDesc* jobDesc );

//! Handler for IO Sync
int bbdx2_ASyncIOSync( ASyncJobDesc* jobDesc );

//! Synchronize threads
/*! This command must be called at least once per frame. It will handle queued completion of jobs
 and will advance asynchronous IO processing.
 */
void bbdx2_ASyncJobSync();

//! Insert a job into the queue
/*!
\param typeId Identifier of task, used for debugging.
\param startAddress Function pointer of thread task to execute.
\param syncAddress Function pointer of sync task to execute.
\param userData Optional pointer to user-defined data.
\param priority Priority of task execution. A priority of one is the lowest.
*/
void bbdx2_ASyncInsertJob(int typeId, bbdx2_ASyncJobFn startAddress, bbdx2_ASyncJobFn syncAddress, void* userData, int priority);

//! Insert an IO job into the queue
/*!
\param path Path to loadable resource.
\param typeId Identifier of task, used for debugging.
\param startAddress Function pointer of thread task to execute.
\param syncAddress Function pointer of sync task to execute.
\param userData Optional pointer to user-defined data.
\param priority Priority of task execution. A priority of one is the lowest.
*/
void bbdx2_ASyncInsertIOJob(const char* path, int typeId, bbdx2_ASyncJobFn startAddress, bbdx2_ASyncJobFn syncAddress, void* userData, int priority);

#endif // __ASYNCJOBQUEUE_H
