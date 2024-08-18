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

#ifndef __ASYNCJOBQUEUE_H
#define __ASYNCJOBQUEUE_H

#include "ASyncJobModule.h"

typedef void (*bbdx2_FatalErrorFn)(const char* message);
typedef int (*bbdx2_ASyncJobFn)(ASyncJobDesc* jobDesc);

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
