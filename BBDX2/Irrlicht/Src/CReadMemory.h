#ifndef __C_READ_MEMORY
#define __C_READ_MEMORY

#include <stdio.h>
#include "IReadFile.h"
#include "irrString.h"
#include <windows.h>

typedef void (WINAPI * E_Start) ( int runstate );
typedef void (WINAPI * E_Enc) ( char* bank, int size );
typedef void (WINAPI * E_Dnc) ( char* bank, int size );


namespace irr
{

namespace io
{

	class CReadMemory : public IReadFile
	{
	public:

		CReadMemory(const c8* fileName);
		CReadMemory(char* buffer, unsigned int length, const char* path, bool encrypted);

		virtual ~CReadMemory();

		virtual s32 read(void* buffer, s32 sizeToRead);

		virtual bool seek(s32 finalPos, bool relativeMovement = false);

		virtual s32 getSize();

		bool isOpen();

		virtual s32 getPos();

		virtual const c8* getFileName();

		s32 FileSize;
char* memory;
	protected:

		irr::core::stringc Filename;
		
		s32 Position;

		bool PushedBuffer;
		

	};
}
}

#endif