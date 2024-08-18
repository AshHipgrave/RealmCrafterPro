// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#include "IrrCompileConfig.h"
#ifdef _IRR_COMPILE_WITH_DIRECT3D_9_

#define _IRR_DONT_DO_MEMORY_DEBUGGING_HERE
#include "CD3D9Texture.h"
#include "os.h"

#include <stdio.h>
#include <d3dx9tex.h>
#include "..\..\Common\JaredsUtils.h"

#ifndef _IRR_COMPILE_WITH_DIRECT3D_8_
// The D3DXFilterTexture function seems to get linked wrong when
// compiling with both D3D8 and 9, causing it not to work in the D3D9 device.
// So mipmapgeneration is replaced with my own bad generation in d3d 8 when
// compiling with both D3D 8 and 9.
#define _IRR_USE_D3DXFilterTexture_
#endif // _IRR_COMPILE_WITH_DIRECT3D_8_

#ifdef _IRR_USE_D3DXFilterTexture_
#pragma comment(lib, "d3dx9.lib")
#endif

extern int bbdxTextureAlloc;

namespace irr
{
namespace video
{



CD3D9Texture::CD3D9Texture(IDirect3DBaseTexture9* image, IDirect3DDevice9* device, core::dimension2d<s32> size, const char* name)
: ITexture(name), Image(0), Device(device), TextureSize(size),
	Texture(0), Pitch(0), ImageSize(size), HasMipMaps(0), HardwareMipMaps(true),
	IsRenderTarget(true), RTTSurface(0)
{

	if(Device)
		Device->AddRef();

	this->Texture = image;
}

//! destructor
CD3D9Texture::~CD3D9Texture()
{

	if (Device)
		Device->Release();

	if (Image)
		Image->drop();

	if (Texture)
		Texture->Release();

	if (RTTSurface)
		RTTSurface->Release();

	bbdxTextureAlloc -= ImageSize.Width * ImageSize.Height * 4;

}



////! lock function
//void* CD3D9Texture::lock()
//{
//	if (!Texture)
//		return 0;
//
//	HRESULT hr;
//	D3DLOCKED_RECT rect;
//	if(!IsRenderTarget)
//	{
//		hr = Texture->LockRect(0, &rect, 0, 0);
//	}
//	else
//	{
//		D3DSURFACE_DESC desc;
//		Texture->GetLevelDesc(0, &desc);
//		if (!RTTSurface)
//		{
//			hr = Device->CreateOffscreenPlainSurface(desc.Width, desc.Height, desc.Format, D3DPOOL_SYSTEMMEM, &RTTSurface, NULL);
//			if (FAILED(hr))
//			{
//				os::Printer::log("Could not lock DIRECT3D9 Texture.", ELL_ERROR);
//				return 0;
//			}
//		}
//
//		IDirect3DSurface9 *surface = NULL;
//		hr = Texture->GetSurfaceLevel(0, &surface);
//		if (FAILED(hr))
//		{
//			os::Printer::log("Could not lock DIRECT3D9 Texture.", ELL_ERROR);
//			return 0;
//		}
//		hr = Device->GetRenderTargetData(surface, RTTSurface);
//		if(FAILED(hr))
//		{
//			os::Printer::log("Could not lock DIRECT3D9 Texture.", ELL_ERROR);
//			return 0;
//		}
//		hr = RTTSurface->LockRect(&rect, NULL, 0);
//		if(FAILED(hr))
//		{
//			os::Printer::log("Could not lock DIRECT3D9 Texture.", ELL_ERROR);
//			return 0;
//		}
//		return rect.pBits;
//	}
//	if (FAILED(hr))
//	{
//		os::Printer::log("Could not lock DIRECT3D9 Texture.", ELL_ERROR);
//		return 0;
//	}
//
//	return rect.pBits;
//}
//
//
//
////! unlock function
//void CD3D9Texture::unlock()
//{
//	if (!Texture)
//		return;
//
//	if (!IsRenderTarget)
//		Texture->UnlockRect(0);
//	else if (RTTSurface)
//		RTTSurface->UnlockRect();
//}


//! Returns original size of the texture.
const core::dimension2d<s32>& CD3D9Texture::getOriginalSize()
{
	return ImageSize;
}


//! Returns (=size) of the texture.
const core::dimension2d<s32>& CD3D9Texture::getSize()
{
	return TextureSize;
}


//! returns the size of a texture which would be the optimize size for rendering it
inline s32 CD3D9Texture::getTextureSizeFromImageSize(s32 size)
{
	s32 ts = 0x01;

	while(ts < size)
		ts <<= 1;

	if (ts > size && ts > 64)
		ts >>= 1;

	return ts;
}



//! returns driver type of texture (=the driver, who created the texture)
E_DRIVER_TYPE CD3D9Texture::getDriverType()
{
	return EDT_DIRECT3D9;
}



//! returns color format of texture
ECOLOR_FORMAT CD3D9Texture::getColorFormat()
{
	return ColorFormat;
}



//! returns pitch of texture (in bytes)
s32 CD3D9Texture::getPitch()
{
	return Pitch;
}



//! returns the DIRECT3D9 Texture
IDirect3DBaseTexture9* CD3D9Texture::getDX9Texture()
{
	return Texture;
}


//! returns if texture has mipmap levels
bool CD3D9Texture::hasMipMaps()
{
	return HasMipMaps;
}


void CD3D9Texture::copy16BitMipMap(char* src, char* tgt,
				   s32 width, s32 height,
				   s32 pitchsrc, s32 pitchtgt)
{
	u16 c;

	for (int x=0; x<width; ++x)
		for (int y=0; y<height; ++y)
		{
			s32 a=0, r=0, g=0, b=0;

			for (int dx=0; dx<2; ++dx)
				for (int dy=0; dy<2; ++dy)
				{
					int tgx = (x*2)+dx;
					int tgy = (y*2)+dy;

					c = *(u16*)((void*)&src[(tgx*2)+(tgy*pitchsrc)]);

					a += getAlpha(c);
					r += getRed(c);
					g += getGreen(c);
					b += getBlue(c);
				}

			a /= 4;
			r /= 4;
			g /= 4;
			b /= 4;

			c = ((a & 0x1) <<15) | ((r & 0x1F)<<10) | ((g & 0x1F)<<5) | (b & 0x1F);
			*(u16*)((void*)&tgt[(x*2)+(y*pitchtgt)]) = c;
		}
}


void CD3D9Texture::copy32BitMipMap(char* src, char* tgt,
				   s32 width, s32 height,
				   s32 pitchsrc, s32 pitchtgt)
{
	SColor c;

	for (int x=0; x<width; ++x)
		for (int y=0; y<height; ++y)
		{
			s32 a=0, r=0, g=0, b=0;

			for (int dx=0; dx<2; ++dx)
				for (int dy=0; dy<2; ++dy)
				{
					int tgx = (x*2)+dx;
					int tgy = (y*2)+dy;

					c = *(u32*)((void*)&src[(tgx<<2)+(tgy*pitchsrc)]);

					a += c.getAlpha();
					r += c.getRed();
					g += c.getGreen();
					b += c.getBlue();
				}

			a >>= 2;
			r >>= 2;
			g >>= 2;
			b >>= 2;

			c = ((a & 0xff)<<24) | ((r & 0xff)<<16) | ((g & 0xff)<<8) | (b & 0xff);
			*(u32*)((void*)&tgt[(x*4)+(y*pitchtgt)]) = c.color;
		}
}


//! Regenerates the mip map levels of the texture. Useful after locking and
//! modifying the texture
//void CD3D9Texture::regenerateMipMapLevels()
//{
//	if (HasMipMaps)
//		createMipMaps();
//}


//! returns if it is a render target
bool CD3D9Texture::isRenderTarget()
{
	return IsRenderTarget;
}

//! Returns pointer to the render target surface
//IDirect3DSurface9* CD3D9Texture::getRenderTargetSurface()
//{
//	if (!IsRenderTarget)
//		return 0;
//
//	IDirect3DSurface9 *pRTTSurface = 0;
//	if (Texture)
//		Texture->GetSurfaceLevel(0, &pRTTSurface);
//
//	if (pRTTSurface)
//		pRTTSurface->Release();
//
//	return pRTTSurface;
//}



} // end namespace video
} // end namespace irr

#endif // _IRR_COMPILE_WITH_DIRECT3D_9_
