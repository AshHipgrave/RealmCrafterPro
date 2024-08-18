// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#ifndef __C_VIDEO_DIRECTX_9_H_INCLUDED__
#define __C_VIDEO_DIRECTX_9_H_INCLUDED__

#include "IrrCompileConfig.h"
#include "..\..\ShaderConstantSet.h"
//#ifdef _IRR_WINDOWS_
//#define WIN32_LEAN_AND_MEAN
#include "CNullDriver.h"
#include <windows.h>
#include "IMaterialRendererServices.h"
#include "CSceneManager.h"
#ifdef _IRR_COMPILE_WITH_DIRECT3D_9_

#ifdef NDEBUG
#define D3D_DEBUG_INFO
#endif
#include <d3d9.h>


//#include "CD3DEffectsRenderer.h"
#include <stdio.h>
#include <iostream>
#include <fstream>
#include "..\..\cRT.h"
#include "..\..\CPPEffect.h"
#include "CD3D9Texture.h"
#include "..\..\IMovieTexture.h"


typedef void (GeneralCallbackFN)(void);
typedef void (RTCallbackFN)(int);
typedef void (XYCallbackFN)(float, float);
typedef void (ShadowMapCallbackFN)(const float*);
typedef void (ShadowMapVPCallbackFN)(const float*, const float*);
extern GeneralCallbackFN* RenderGUICallback[10];
extern GeneralCallbackFN* RenderSolidCallback[10];
extern RTCallbackFN* RenderSolidCallbackRT[10];
extern GeneralCallbackFN* DeviceResetCallback[10];
extern GeneralCallbackFN* DeviceLostCallback[10];
extern XYCallbackFN* DeviceResetXYCallback[10];
extern ShadowMapCallbackFN* RenderShadowDepthCallback[10];
extern ShadowMapVPCallbackFN* RenderShadowDepthVPCallback[10];

void CallRenderGUICallback();
void CallRenderSolidCallback();
void CallRenderSolidCallbackRT(int rtindex);
void CallDeviceResetCallback();
void CallDeviceLostCallback();
void CallDeviceResetXYCallback(float, float);
void CallShadowMapCallback(const float*, const float*, const float*);


using namespace irr::core;

namespace irr
{
namespace video
{
	struct SAnimatedVert
	{
		core::vector3df Pos;
		float BlendWeights[4];
		unsigned char BlendIndex[4];
		core::vector3df Normal;
		video::SColor Color;
		core::vector2df TexCoord1;
		core::vector2df TexCoord2;
		core::vector3df Tangent;
		core::vector3df Binormal;
	};
	/*struct SAnimatedVert
	{
		core::vector3df Pos;
		core::vector3df Normal;
		video::SColor Color;
		core::vector2df TexCoord1;
	};*/

	class CD3D9Driver : public CNullDriver, IMaterialRendererServices
	{
	public:
	
		IDirect3DDevice9* pID3DDevice;
		irr::scene::ISceneManager* Smgr;

		// FX stuff
		virtual u32 loadFXShader(const c8* ShaderProgram);
		virtual void ReloadShaders();
		virtual void ReloadEffect(u32 i);
		virtual ID3DXEffect* getEffect(u32 i);
		virtual void setShader(u32 Effect);
		virtual void Destroy();

		virtual IDirect3DDevice9* GetDXDevice();

		//! constructor
		CD3D9Driver(const core::dimension2d<s32>& screenSize, HWND window, bool fullscreen,
			bool stencibuffer, io::IFileSystem* io, bool pureSoftware=false);

		//! destructor
		virtual ~CD3D9Driver();

		//! applications must call this method before performing any rendering. returns false if failed.
		virtual bool beginScene(bool backBuffer, bool zBuffer, SColor color);

		//! applications must call this method after performing any rendering. returns false if failed.
		virtual bool endScene( s32 windowId = 0, core::rect<s32>* sourceRect=0 );

		//! queries the features of the driver, returns true if feature is available
		virtual bool queryFeature(E_VIDEO_DRIVER_FEATURE feature);

		//! sets transformation
		virtual void setTransform(E_TRANSFORMATION_STATE state, core::matrix4& mat);

		//! sets transformation
		virtual void setTransformN(E_TRANSFORMATION_STATE state, core::matrix4& mat);

		//! sets a material
		virtual void setMaterial(const SMaterial& material);

		//! sets a viewport
		virtual void setViewPort(const core::rect<s32>& area);

		//! gets the area of the current viewport
		virtual const core::rect<s32>& getViewPort() const;

		//! Draws a 3d line.
		virtual void draw3DLine(const core::vector3df& start,
			const core::vector3df& end, SColor color = SColor(255,255,255,255));

		//! initialises the Direct3D API
		bool initDriver(const core::dimension2d<s32>& screenSize, HWND hwnd,
				u32 bits, bool fullScreen, bool pureSoftware,
				bool highPrecisionFPU, bool vsync, int antiAlias);

		//! \return Returns the name of the video driver. Example: In case of the DIRECT3D8
		//! driver, it would return "Direct3D8.1".
		virtual const wchar_t* getName();


		//! Returns the maximum amount of primitives (mostly vertices) which
		//! the device is able to render with one drawIndexedTriangleList
		//! call.
		virtual s32 getMaximalPrimitiveCount();

		//! Enables or disables a texture creation flag.
		virtual void setTextureCreationFlag(E_TEXTURE_CREATION_FLAG flag, bool enabled);

		//! Sets the fog mode.
		virtual void setFog(SColor color, bool linearFog, f32 start,
			f32 end, f32 density, bool pixelFog, bool rangeFog);

		//! Only used by the internal engine. Used to notify the driver that
		//! the window was resized.
		virtual void OnResize(const core::dimension2d<s32>& size);

		//! Returns type of video driver
		virtual E_DRIVER_TYPE getDriverType();

		//! Returns the transformation set by setTransform
		virtual const core::matrix4& getTransform(E_TRANSFORMATION_STATE state);

		//! Returns pointer to the IGPUProgrammingServices interface.
		virtual IGPUProgrammingServices* getGPUProgrammingServices();

		//! Returns a pointer to the IVideoDriver interface. (Implementation for
		//! IMaterialRendererServices)
		virtual IVideoDriver* getVideoDriver();

		//! Clears the ZBuffer.
		virtual void clearZBuffer();;

		//Ja Hardware buffer changes
		virtual u32 getFVFSize(video::E_VERTEX_TYPE vType);
		virtual bool updateHardwareBuffer(scene::IMeshBuffer* buffer);
		virtual bool updateAnimatedHardwareBuffer(scene::IMeshBuffer* buffer, core::array<scene::VertexBlends>* Blends);
		virtual bool updateHardwareBufferLM(scene::SMeshBufferLightMap* buffer);
		virtual bool deleteHardwareBuffer(scene::IMeshBuffer* buffer);
		virtual void drawIndexedTriangleListHw(void* indexP,void* vertexP,int indexC,int vertexC,video::E_VERTEX_TYPE vt,int TCount);
		virtual ITexture* loadDDSTexture(c8* filename, bool Mask, bool Cubemap, bool Volume);
		virtual ITexture* CopyTexture(ITexture* tex);

		SColorf AmbientLight;
		bool ResetRenderStates; // bool to make all renderstates be reseted if set
		SMaterial Material, LastMaterial;

		virtual void SetReflectionMapTarget();
		virtual void UnSetReflectionMapTarget();
		virtual void SetBackBuffer2Target(bool clear = true);
		virtual void UnSetBackBuffer2Target();

		virtual void SetClipPlane( int iPlane, plane3df plane ); // iPlane == -1 disable all planes

		virtual void SetShadowTarget();
		virtual void UnSetShadowTarget();
		virtual void SetBlurShadowTarget();
		virtual void UnSetBlurShadowTarget();
		virtual void SetBackBufferTarget();
		virtual IDirect3DTexture9* GetShadowTexture(bool useBlur);
		virtual IDirect3DTexture9* GetReflectionTexture();
//		virtual IDirect3DTexture9* GetBackBufferTexture() { return BackBuffer->pTexture; }
		core::array<ShaderConstantSet*> EffectsList;

		virtual void UpdateFrameBuffer();
		virtual IDirect3DVertexDeclaration9*  GetStandardDefinition();
		virtual void ResetVertexDeclarationCache();
		virtual void ResizeScreen(dimension2d<irr::s32> NewSize, bool FullScreen, int AntiAlias, int Anisotropy);
		virtual void* LoadFont(const char* FontName, int Size, int Bold, int Italic);
		virtual void FreeFont(void* Font);

		static void RegisterMovieTexture(IMovieTexture* MovieTexture);
		static void UnRegisterMovieTexture(IMovieTexture* MovieTexture);
		static irr::core::array<IMovieTexture*> ActiveMovies;

		virtual ITexture* getEmptyTexture(irr::c8* filename);

		virtual void setShadowMapSize(int newSize);
		virtual int getShadowMapSize();

		virtual IDirect3DTexture9* getBackBuffer2()
		{
			return NULL;//BackBuffer2->pTexture;
		}

		virtual void UnSetBackBuffer2()
		{
			//if(BackBuffer2 != 0)
			//	BackBuffer2->UnSet(false, false, 1);
		}

		virtual void SetBackBuffer2()
		{
			//if(BackBuffer2 != 0)
			//	BackBuffer2->Set(-1, -1, false, 1);
		}

		virtual int GetAntiAlias()
		{
			return AntiAlias;
		}

		virtual IDirect3DTexture9* GetBackBuffer2Texture();

	private:

		//ID3DXEffect* FinalEffect;

		bool CheckFormat(D3DFORMAT depthFormat, D3DFORMAT targetFormat);

		irr::core::array<ID3DXFont*> Fonts;
		irr::core::array<CPPEffect*> PostProcessList;
		cRT* BackBuffer;
		//cRT* BackBuffer2;
		cRT* ShadowMapRT;
		cRT* ShadowMapBlur;
		cRT* ReflectionMap; // Used by the water for planar reflections
		cRT* BackBuffer2;
		int ShadowMapSize;
		D3DFORMAT ShadowFormat;
		int AntiAlias;

		CD3D9Texture* BackBufferFrame;
		CD3D9Texture* BackBuffer2Frame;
		IDirect3DTexture9* TexBBFrame;
		IDirect3DTexture9* TexBB2Frame;

		//IDirect3DTexture9* PostProcessBackBuffer;

		// enumeration for rendering modes such as 2d and 3d for minizing the switching of renderStates.
		enum E_RENDER_MODE
		{
			ERM_NONE = 0,	// no render state has been set yet.
			ERM_2D,			// 2d drawing rendermode
			ERM_3D,			// 3d rendering mode
			ERM_STENCIL_FILL, // stencil fill mode
			ERM_SHADOW_VOLUME_ZFAIL, // stencil volume draw mode
			ERM_SHADOW_VOLUME_ZPASS // stencil volume draw mode
		};

		//! sets the needed renderstates
		void setRenderStatesStencilFillMode(bool alpha);

		//! sets the needed renderstates
		void setRenderStatesStencilShadowMode(bool zfail);

		//! sets the current Texture
		void setTexture(s32 stage, video::ITexture* texture);

		//! resets the device
		bool reset();

		// returns the current size of the screen or rendertarget
		core::dimension2d<s32> getCurrentRenderTargetSize();

		void removeTexture(ITexture* texture);

		inline D3DCOLORVALUE colorToD3D(const SColor& col)
		{
			const f32 f = 1.0f / 255.0f;
			D3DCOLORVALUE v;
			v.r = col.getRed() * f;
			v.g = col.getGreen() * f;
			v.b = col.getBlue() * f;
			v.a = col.getAlpha() * f;
			return v;
		}


		E_RENDER_MODE CurrentRenderMode;

		public:
		D3DPRESENT_PARAMETERS present;
		private:

		core::matrix4 Matrices[ETS_COUNT]; // matrizes of the 3d mode we need to restore when we switch back from the 2d mode.

		
		
		bool Transformation3DChanged;
		bool StencilBuffer;
		bool LastTextureMipMapsAvailable[4];
		ITexture* CurrentTexture[4];

		HINSTANCE D3DLibrary;
		IDirect3D9* pID3D;
		
		irr::u32 CurrentEffect;

		IDirect3DSurface9* PrevRenderTarget;
		core::dimension2d<s32> CurrentRendertargetSize;

		
		IDirect3DVertexDeclaration9* S3DVertexVDecl;
		IDirect3DVertexDeclaration9* S3DVertex2TCoordsVDecl;
		IDirect3DVertexDeclaration9* S3DVertexTangentsVDecl;
		IDirect3DVertexDeclaration9* SAnimatedVertVDecl;
		IDirect3DVertexDeclaration9* CurrentDecl;

		D3DCAPS9 Caps;

		E_VERTEX_TYPE LastVertexType;

		f32 MaxLightDistance;
		s32 LastSetLight;
		bool DeviceLost;
		bool Fullscreen;
		bool PPActive;

	};


} // end namespace video
} // end namespace irr


#endif // _IRR_COMPILE_WITH_DIRECT3D_9_
//#endif // _IRR_WINDOWS_
#endif // __C_VIDEO_DIRECTX_8_H_INCLUDED__

