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


//#include <dshow.h>
//#include <mmstream.h>
//#include <amstream.h>
//#include <ddstream.h>
//
//
//static GUID MY_CLSID_AMMultiMediaStream={0x49C47CE5,0x9BA4,0x11D0,0x82,0x12,0x00,0xC0,0x4F,0xC3,0x2C,0x45};
//static GUID MY_IID_IAMMultiMediaStream={0xBEBE595C,0x9A6F,0x11D0,0x8F,0xDE,0x00,0xC0,0x4F,0xD9,0x18,0x9D};
//static GUID MY_MSPID_PrimaryVideo={0xA35FF56A,0x9FDA,0x11D0,0x8F,0xDF,0x00,0xC0,0x4F,0xD9,0x18,0x9D};
//static GUID MY_IID_IDirectDrawMediaStream={0xF4104FCE,0x9A70,0x11D0,0x8F,0xDE,0x00,0xC0,0x4F,0xD9,0x18,0x9D}; 
//static GUID MY_MSPID_PrimaryAudio={0xA35FF56B,0x9FDA,0x11D0,0x8F,0xDF,0x00,0xC0,0x4F,0xD9,0x18,0x9D};
//
//class TMovie
// {
//         IAMMultiMediaStream*     pAMStream;
//         IMediaStream*            pPrimaryVidStream;
//         IDirectDrawMediaStream*  pDDStream;
//         IDirectDrawStreamSample* pSample;
//         IDirectDrawSurface*      pSurface;
//         RECT                     Movie_rect;
//         LONG                     MoviePitch;
//         void*                    MovieBuffer;
//         DWORD                    time;
//         DWORD                    oldtick;
//     
//     public:   
//           TMovie()
//            {
//               CoInitialize(0);
//               pAMStream         = 0;
//               pPrimaryVidStream = 0; 
//               pDDStream         = 0;   
//               pSample           = 0;
//               pSurface          = 0;
//               time              = 0;
//            }
//               
//           ~TMovie()
//            {
//                pPrimaryVidStream->Release();
//                pDDStream->Release();
//                pSample->Release();
//                pSurface->Release();
//                pAMStream->Release();
//                CoUninitialize();
//            }
//           
//           void LoadMovie(char* filename)
//            {
//                WCHAR buf[512];
//                MultiByteToWideChar(CP_ACP,0,filename,-1,buf,512);
//                CoCreateInstance(MY_CLSID_AMMultiMediaStream,0,1,MY_IID_IAMMultiMediaStream,(void**)&pAMStream);
//                pAMStream->Initialize((STREAM_TYPE) 0, 0, NULL);
//                pAMStream->AddMediaStream( 0, &MY_MSPID_PrimaryVideo, 0, NULL);
//                pAMStream->OpenFile(buf,4);
//                pAMStream->GetMediaStream( MY_MSPID_PrimaryVideo, &pPrimaryVidStream);
//                pPrimaryVidStream->QueryInterface(MY_IID_IDirectDrawMediaStream,(void**)&pDDStream);
//                pDDStream->CreateSample(0,0,0,&pSample);
//                pSample->GetSurface(&pSurface,&Movie_rect);
//                pAMStream->SetState((STREAM_STATE)1);
//            }
//           
//           void NextMovieFrame()
//            {
//               if(GetTickCount()-oldtick < time)return ;
//               oldtick = GetTickCount(); 
//               pSample->Update( 0, NULL, NULL, 0);
//            }
//           
//           int MovieWidth() { return (Movie_rect.right - Movie_rect.left);}
//           
//           int MovieHeight() { return (Movie_rect.bottom - Movie_rect.top);} 
//           
//           void DrawMovie(int x,int y,ITexture* Buf)
//            {   
//                void* pBits = Buf->lock();
//				//MessageBox(0,"1","",MB_OK);
//                LONG  Pitch = Buf->getPitch(); 
//				//MessageBox(0,"2","",MB_OK);
//                DDSURFACEDESC  ddsd;
//				//MessageBox(0,"3","",MB_OK);
//                ddsd.dwSize=sizeof(DDSURFACEDESC);
//				//MessageBox(0,"4","",MB_OK);
//                pSurface->Lock( NULL,&ddsd, DDLOCK_SURFACEMEMORYPTR | DDLOCK_WAIT , NULL);
//				//MessageBox(0,"5","",MB_OK);
//                int wmin=(Pitch<ddsd.lPitch)?Pitch:ddsd.lPitch;
//				//MessageBox(0,"6","",MB_OK);
//				char tmp[1024];
//				sprintf(tmp,"%i",ddsd.dwHeight);
//				//MessageBox(0,(LPCSTR)tmp,"",MB_OK);
//                for(int h=0; h<240/*ddsd.dwHeight*/; h++)
//				{
//					//sprintf(tmp,"%i",h);
//					//MessageBox(0,(LPCSTR)tmp,"",MB_OK);              
//					memcpy((BYTE*)pBits+((y+h)*Pitch)+x*4, (BYTE*)ddsd.lpSurface+h*ddsd.lPitch,wmin);
//				}
//				 //MessageBox(0,"7","",MB_OK);
//                pSurface->Unlock(NULL);
//				//MessageBox(0,"8","",MB_OK);
//                Buf->unlock();
//				//MessageBox(0,"9","",MB_OK);
//            } 
//           
//          void SetMovieFPS(int fps)
//            {
//                time = fps;
//            }           
//           
// } ;  
//
//TMovie* TMoveHan[10];
//IGUIImage* TDrawStk[10];
//ITexture* TTexStk[10];
//int TDrawCnt = 0;
//int TMoveCnt = -1;
//
//DLLPRE int DLLEX outa(char* filename)
//{
//	TMoveCnt++;
//	TMoveHan[TMoveCnt] = new TMovie;
//	TMoveHan[TMoveCnt]->LoadMovie(filename);
//	TMoveHan[TMoveCnt]->SetMovieFPS(25);
//	return TMoveCnt;
//}
//
//DLLPRE void DLLEX jabberwock(int h)
//{
//	delete TMoveHan[h];
//}
//
//DLLPRE int DLLEX towipe(int h)
//{
//	return TMoveHan[h]->MovieWidth();
//}
//
//DLLPRE int DLLEX onour(int h)
//{
//	return TMoveHan[h]->MovieHeight();
//}
//
//DLLPRE void DLLEX factoid(int ha, int x, int y, int w, int h)
//{
//	TMoveHan[ha]->NextMovieFrame();
//	if(!TTexStk[ha])
//	{
//		TTexStk[ha] = driver->addTexture(dimension2d<irr::s32>(TMoveHan[ha]->MovieWidth(),TMoveHan[ha]->MovieWidth()),"movetemp.tex");
//	}
//	TMoveHan[ha]->DrawMovie(x,y,TTexStk[ha]);
//	//TDrawStk[ha] = env->addImage(temp,irr::core::position2d<irr::s32>(TMoveHan[ha]->MovieWidth(),TMoveHan[ha]->MovieHeight()));
//	//TDrawCnt++;
//}
//
//DLLPRE ITexture* DLLEX desktops(int ha)
//{
//	return TTexStk[ha];
//}



