
/*
HINSTANCE rce = LoadLibrary("rc63.dll");
if(!rce)
{
	MessageBox(NULL, "Could not load \"rc63.dll\" - Please check it exists","ERROR", MB_ICONERROR | MB_OK);
	//exit(0);
}

E_Start BF_Start = (E_Start)GetProcAddress(rce,"BF_Start");
E_Enc BF_encrypt = (E_Enc)GetProcAddress(rce,"BF_encrypt");
E_Dnc BF_decrypt = (E_Dnc)GetProcAddress(rce,"BF_decrypt");

if(!BF_Start){MessageBox(NULL,"Could not load function: BF_Start in rc63.dll","Library Module",MB_OK | MB_ICONERROR);}
if(!BF_encrypt){MessageBox(NULL,"Could not load function: BF_encrypt in rc63.dll","Library Module",MB_OK | MB_ICONERROR);}
if(!BF_decrypt){MessageBox(NULL,"Could not load function: BF_decrypt in rc63.dll","Library Module",MB_OK | MB_ICONERROR);}
*/

/*
extern HINSTANCE rce = LoadLibrary("rc63.dll");
if(!rce)
{
	MessageBox(NULL, "Could not load \"rc63.dll\" - Please check it exists","ERROR", MB_ICONERROR | MB_OK);
	exit(0);
}

typedef void (WINAPI * E_Start) ( int runstate );
typedef void (WINAPI * E_Enc) ( char* bank, int size );
typedef void (WINAPI * E_Dnc) ( char* bank, int size );


extern E_Start BF_Start = (E_Start)GetProcAddress(rce,"BF_Start");
extern E_Enc BF_encrypt = (E_Enc)GetProcAddress(rce,"BF_encrypt");
extern E_Dnc BF_decrypt = (E_Dnc)GetProcAddress(rce,"BF_decrypt");

if(!BF_Start){MessageBox(NULL,"Could not load function: BF_Start in rc63.dll","Library Module",MB_OK | MB_ICONERROR);}
if(!BF_encrypt){MessageBox(NULL,"Could not load function: BF_encrypt in rc63.dll","Library Module",MB_OK | MB_ICONERROR);}
if(!BF_decrypt){MessageBox(NULL,"Could not load function: BF_decrypt in rc63.dll","Library Module",MB_OK | MB_ICONERROR);}
*/

