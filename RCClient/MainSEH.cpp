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
#include <windows.h>
#include <string>
#include <imagehlp.h>
#include <Strsafe.h>

#define USESEH

DWORD HandleGlobalException(_EXCEPTION_POINTERS* ExInfo);

// This declaration refers to the applications main entrypoint
int Main();

// Set EntryPoint (to capture SEH)
int WINAPI WinMain(HINSTANCE HInstance, HINSTANCE, LPSTR commandLine, int)
//int main()
{
	_CrtSetDbgFlag(0);
#ifdef USESEH
	__try
	{
		return Main();
	}
	__except (HandleGlobalException(GetExceptionInformation()))
	{
	}
#else
	return Main();
#endif

	return 0;
}

#ifdef USESEH
// Nabbed from MSDN
int GenerateDump(_EXCEPTION_POINTERS* pExceptionPointers)
{
	BOOL bMiniDumpSuccessful;
	CHAR szPath[MAX_PATH]; 
	CHAR szFileName[MAX_PATH]; 
	CHAR* szAppName = "Realm Crafter";
	CHAR* szVersion = "v2.53";
	DWORD dwBufferSize = MAX_PATH;
	HANDLE hDumpFile;
	SYSTEMTIME stLocalTime;
	MINIDUMP_EXCEPTION_INFORMATION ExpParam;

	GetLocalTime( &stLocalTime );
	//GetTempPath( dwBufferSize, szPath );
	StringCchPrintf( szPath, MAX_PATH, "%s", ".\\");

	StringCchPrintf( szFileName, MAX_PATH, "%s%s", szPath, "ErrorReports" );
	CreateDirectory( szFileName, NULL );

	StringCchPrintf( szFileName, MAX_PATH, "%s%s\\%s-%04d%02d%02d-%02d%02d%02d-%ld-%ld.dmp", 
		szPath, "ErrorReports", szVersion, 
		stLocalTime.wYear, stLocalTime.wMonth, stLocalTime.wDay, 
		stLocalTime.wHour, stLocalTime.wMinute, stLocalTime.wSecond, 
		GetCurrentProcessId(), GetCurrentThreadId());
	hDumpFile = CreateFile(szFileName, GENERIC_READ|GENERIC_WRITE, 
		FILE_SHARE_WRITE|FILE_SHARE_READ, 0, CREATE_ALWAYS, 0, 0);

	ExpParam.ThreadId = GetCurrentThreadId();
	ExpParam.ExceptionPointers = pExceptionPointers;
	ExpParam.ClientPointers = TRUE;

	bMiniDumpSuccessful = MiniDumpWriteDump(GetCurrentProcess(), GetCurrentProcessId(), 
		hDumpFile, MiniDumpWithDataSegs, &ExpParam, NULL, NULL);

	return EXCEPTION_EXECUTE_HANDLER;
}


DWORD HandleGlobalException(_EXCEPTION_POINTERS* ExInfo)
{

	std::string ExceptionType = "Unknown Exception";

	//_EXCEPTION_POINTERS* ExInfo;
	//ExInfo = GetExceptionInformation;

	switch(ExInfo->ExceptionRecord->ExceptionCode)
	{
	case EXCEPTION_ACCESS_VIOLATION:
		{
			ExceptionType = "Memory Access Violation";

			if(ExInfo->ExceptionRecord->NumberParameters > 0)
			{
				if(ExInfo->ExceptionRecord->ExceptionInformation[0] == 0)
					ExceptionType = "Read Error: Memory Access Violation";
				else if(ExInfo->ExceptionRecord->ExceptionInformation[0] == 1)
					ExceptionType = "Write Error: Memory Access Violation";
				if(ExInfo->ExceptionRecord->ExceptionInformation[0] == 8)
					ExceptionType = "Read/Write Error: Data Execution Prevention Violation";
			}

			if(ExInfo->ExceptionRecord->NumberParameters > 1)
			{
				char Addr[128];
				sprintf(Addr, "0x%p", ExInfo->ExceptionRecord->ExceptionInformation[1]);
				ExceptionType += std::string(" at Address: ") + Addr;
			}

			break;
		}
	case EXCEPTION_ARRAY_BOUNDS_EXCEEDED:
		ExceptionType = "Array Index was Out of Bounds";
		break;
	case EXCEPTION_BREAKPOINT:
		ExceptionType = "Breakpoint was encountered";
		break;
	case EXCEPTION_DATATYPE_MISALIGNMENT:
		ExceptionType = "Attempted to access misaligned data";
		break;
	case EXCEPTION_FLT_DENORMAL_OPERAND:
		ExceptionType = "Denomalized Floating-Point Value";
		break;
	case EXCEPTION_FLT_DIVIDE_BY_ZERO:
		ExceptionType = "Float Divided by Zero";
		break;
	case EXCEPTION_FLT_INEXACT_RESULT:
		ExceptionType = "Inexact Floating Result";
		break;
	case EXCEPTION_FLT_INVALID_OPERATION:
		ExceptionType = "Invalid Float Operation";
		break;
	case EXCEPTION_FLT_OVERFLOW:
		ExceptionType = "Floating-Point Overflow";
		break;
	case EXCEPTION_FLT_STACK_CHECK:
		ExceptionType = "Floating-Point Stack Over or UnderFlow";
		break;
	case EXCEPTION_FLT_UNDERFLOW:
		ExceptionType = "Floating-Point Underflow";
		break;
	case EXCEPTION_ILLEGAL_INSTRUCTION:
		ExceptionType = "Attempted to Execute and Illegal Instruction";
		break;
	case EXCEPTION_IN_PAGE_ERROR:
		ExceptionType = "Page Error";
		break;
	case EXCEPTION_INT_DIVIDE_BY_ZERO:
		ExceptionType = "Integer Divide by Zero";
		break;
	case EXCEPTION_INT_OVERFLOW:
		ExceptionType = "Integer Overflow";
		break;
	case EXCEPTION_INVALID_DISPOSITION:
		ExceptionType = "Invalid Disposition";
		break;
	case EXCEPTION_NONCONTINUABLE_EXCEPTION:
		ExceptionType = "Execution continued after exception occurred";
		break;
	case EXCEPTION_PRIV_INSTRUCTION:
		ExceptionType = "Illegal Instruction Execution Attempt on Incompatible Mode";
		break;
	case EXCEPTION_SINGLE_STEP:
		ExceptionType = "Single Step";
		break;
	case EXCEPTION_STACK_OVERFLOW:
		ExceptionType = "Stack Overflow";
		break;
	}

	ExceptionType.append("\n\nClick OK to save a log file and close the application.");

	MessageBoxA(NULL, ExceptionType.c_str(), "Critical Exception Thrown", MB_ICONERROR);

	GenerateDump(ExInfo);

	return EXCEPTION_EXECUTE_HANDLER;
}
#endif


