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
//#include <curl/curl.h>
//#include <stdio.h>
#include "BlitzPlus.h"
#include <winsock.h>
#include <NGinString.h>
//
//#pragma comment(lib, "libcurl.lib")
//
//class CURLDownloader
//{
//	CURL* CurlHandle;
//
//	static size_t Write(void* Buffer, size_t Size, size_t NMemb, void* File);
//
//public:
//
//	CURLDownloader();
//	~CURLDownloader();
//	bool DownloadFile(String File, String SaveTo);
//};
//
//CURLDownloader::CURLDownloader()
//{
//	CurlHandle = curl_easy_init();
//}
//
//CURLDownloader::~CURLDownloader()
//{
//
//}
//
//bool CURLDownloader::DownloadFile(String File, String SaveTo)
//{
//	// Set URL
//	if(curl_easy_setopt(CurlHandle, CURLOPT_URL, File.c_str()) != CURLE_OK)
//		return false;
//
//	// Set Download callback
//	if(curl_easy_setopt(CurlHandle, CURLOPT_WRITEFUNCTION, Write) != CURLE_OK)
//		return false;
//
//	// If there is no filename, then get one!
//	if(SaveTo == String(""))
//	{
//		const char* Str = File.c_str();
//		int i = File.Length() - 1;
//		for(; i >= 0; --i)
//			if(Str[i] == '/' || Str[i] == '\\')
//				break;
//		SaveTo = File.Substr(i + 1);
//	}
//
//	// Write the file
//	FILE* F = fopen(SaveTo.c_str(), "w");
//	if(F == 0)
//		return false;
//
//	// Set the file for writing
//	if(curl_easy_setopt(CurlHandle, CURLOPT_WRITEDATA, F) != CURLE_OK)
//	{
//		fclose(F);
//		return false;
//	}
//
//	// Actually do the downloading
//	if(curl_easy_perform(CurlHandle) != CURLE_OK)
//	{
//		fclose(F);
//		return false;
//	}
//
//	// Cleanup and return
//	fclose(F);
//	return true;
//}
//
//size_t CURLDownloader::Write(void* Buffer, size_t Size, size_t NMemb, void* File)
//{
//	fwrite(Buffer, Size, NMemb, (FILE*)File);
//	return Size * NMemb;
//}

namespace BlitzPlus
{

// 	bool DownloadFile(std::string File, std::string SaveTo)
// 	{
// 		//CURLDownloader TDl;
// 		//return TDl.DownloadFile(File, SaveTo);
// 		return false;
// 	}


	bool DownloadFile(std::string File, std::string SaveTo)
	{
		if(File.size() > 7 && File.substr(0, 7) == std::string("http://"))
			File = File.substr(7);

		std::string Host, PostPath;
		int Port = 80;

		int Slash = File.find('/');
		if(Slash == std::string::npos)
		{
			Host = File;
			PostPath = "/";
		}else
		{
			Host = File.substr(0, Slash);
			PostPath = File.substr(Slash);
		}

		int Colon = Host.find(':');
		if(Colon != std::string::npos)
		{
			std::string PortStr = Host.substr(Colon + 1);
			Host = Host.substr(0, Colon);

			Port = atoi(PortStr.c_str());
		}

		char DBO[2048];
		sprintf(DBO, "Host: %s; Port: %i; Path: %s\n",
			Host.c_str(), Port, PostPath.c_str());
		OutputDebugString(DBO);

		WSADATA WSAData;
		WORD Version = MAKEWORD(2, 0);
		int Error = 0;

		if(WSAStartup(Version, &WSAData) != 0)
		{
			OutputDebugStringA("StartRemoveDebugging Failed: WinSock2 is an incorrect version.\n");
			return false;
		}

		SOCKET Client = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
		SOCKADDR_IN SockAddr;
		memset(&SockAddr, 0, sizeof(SOCKADDR_IN));

		HOSTENT* HostAddr = gethostbyname(Host.c_str());

		SockAddr.sin_family = AF_INET;
		SockAddr.sin_port = htons(Port);
		SockAddr.sin_addr.s_addr = ((IN_ADDR*)HostAddr->h_addr)->s_addr;

		if(connect(Client, (const sockaddr*)&SockAddr, sizeof(SOCKADDR_IN)) == SOCKET_ERROR)
		{
			OutputDebugStringA("Download Failed: Could not connect to host.\n");
			return false;
		}

		//int Val = 1;
		//setsockopt(DebuggingClient, IPPROTO_TCP, TCP_NODELAY, (const char*)&Val, 4);

		char RequestStr[4096];
		sprintf(RequestStr,
			"GET %s HTTP/1.1\r\n"
			"Host: %s\r\n"
			"User-Agent: BBDX/BB (Windows)\r\n"
			"Accept: */*\r\n\r\n",
			PostPath.c_str(), Host.c_str());

		send(Client, RequestStr, strlen(RequestStr), 0);

		NGin::CString Packet;
		int FileLength = 0;
		bool HadLength = false;
		bool OKReceived = false;

		while(true)
		{
			char Buffer[4096];
			int BytesRead = recv(Client, Buffer, 4096, 0);
			if(BytesRead == 0)
			{
				OutputDebugStringA("Streamed closed too early.\n");
				return false;
			}

			Packet.Append(Buffer, false, BytesRead);

			OutputDebugStringA(Packet.c_str());
			OutputDebugStringA("\n");

			int StartOffset = 0;
			bool FoundEnd = false;
			for(int Idx = 0; Idx < Packet.Length() - 1; ++Idx)
			{
				if(Packet.GetRealChar(Idx) == '\r' && Packet.GetRealChar(Idx + 1) == '\n')
				{
					// End of headers
					if(Idx == StartOffset)
					{
						if(Packet.Length() > Idx + 2)
							Packet = Packet.Substr(Idx + 2);
						else
							Packet.Set("");
						FoundEnd = true;
						break;
					}

					NGin::CString FullHeader = Packet.Substr(StartOffset, Idx - StartOffset);
					if(FullHeader.Length() > 15 && FullHeader.AsLower().Substr(0, 15) == NGin::CString("content-length:"))
					{
						NGin::CString TLen = FullHeader.Substr(15);
						TLen.Trim();
						FileLength = TLen.ToInt();
						HadLength = true;
						OutputDebugStringA("Downloader: Length\n");
					}

					if(FullHeader.Length() > 12)
					{
						if(FullHeader.Substr(0, 7).AsLower() == NGin::CString("http/1."))
						{
							if(FullHeader.Substr(0, 12).AsLower() == NGin::CString("http/1.1 200") || FullHeader.Substr(0, 12).AsLower() == NGin::CString("http/1.0 200"))
							{
								OKReceived = true;
								OutputDebugStringA("Downloader: HeaderStart OK\n");
							}else
							{
								OutputDebugStringA("Downloader: Received an unfriendly header '\n");
								OutputDebugStringA(FullHeader.c_str());
								OutputDebugStringA("'\n");
							}
						}
					}

					Idx +=1;
					StartOffset = Idx + 1;
				}
			}

			if(FoundEnd)
				break;

			Packet = Packet.Substr(StartOffset);
		} // Headers

		FILE* F = fopen(SaveTo.c_str(), "wb");
		
		fwrite(Packet.c_str(), 1, Packet.Length(), F);
		if(HadLength)
			FileLength -= Packet.Length();

		while(!HadLength || FileLength > 0)
		{
			char Buffer[4096];
			int BytesRead = recv(Client, Buffer, 4096, 0);
			if(BytesRead == 0)
				break;

			fwrite(Buffer, 1, BytesRead, F);
			if(HadLength)
				FileLength -= BytesRead;
		}

		closesocket(Client);
		fclose(F);

		return true;

		
	}
}
