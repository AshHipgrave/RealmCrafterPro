#include "PhysXErrorStream.h"
#include <stdio.h>
#include <windows.h>

void ErrorStream::reportError(NxErrorCode e, const char* message, const char* file, int line)
{
	char DBO[4096];
	sprintf(DBO, "%s (%d) :", file, line);
	switch (e)
	{
		case NXE_INVALID_PARAMETER:
			sprintf(DBO, "%sinvalid parameter", DBO);
			break;
		case NXE_INVALID_OPERATION:
			sprintf(DBO, "%sinvalid operation", DBO);
			break;
		case NXE_OUT_OF_MEMORY:
			sprintf(DBO, "%sout of memory", DBO);
			break;
		case NXE_DB_INFO:
			sprintf(DBO, "%sinfo", DBO);
			break;
		case NXE_DB_WARNING:
			sprintf(DBO, "%swarning", DBO);
			break;
		default:
			sprintf(DBO, "%sunknown error", DBO);
	}

	sprintf(DBO, "%s : %s\n", DBO, message);
	OutputDebugString(DBO);
}

NxAssertResponse ErrorStream::reportAssertViolation(const char* message, const char* file, int line)
{
	char DBO[2048];
	sprintf(DBO, "access violation : %s (%s line %d)\n", message, file, line);
	OutputDebugString(DBO);
	switch (MessageBoxA(0, message, "AssertViolation, see console for details.", MB_ABORTRETRYIGNORE))
	{
		case IDRETRY:
			return NX_AR_CONTINUE;
		case IDIGNORE:
			return NX_AR_IGNORE;
		case IDABORT:
		default:
			return NX_AR_BREAKPOINT;
	}
}

void ErrorStream::print(const char* message)
{
	OutputDebugString(message);
	OutputDebugString("\n");
}
