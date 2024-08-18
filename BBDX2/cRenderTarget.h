#pragma once

namespace NGin
{
	#if !defined(NGIN_TYPE_UINT)
		typedef unsigned int uint;
		#define NGIN_TYPE_UINT
	#endif
}
using namespace NGin;

class cRenderTarget
{
	private:
		IDirect3DDevice9* pd3dDevice;
		bool _IsSet;

	public:
		cRenderTarget();
		cRenderTarget(IDirect3DDevice9* Device);
		cRenderTarget(IDirect3DDevice9* Device, D3DFORMAT Format);
		~cRenderTarget();

		bool Create(uint Width, uint Height, bool Pow2);
		void Set(uint Width = -1, uint Height = -1, bool Clear = true); // -1 is leave the viewport as in the RT, 0 use the window viewport and >0 use those dimensions
		void UnSet(bool ClearPrev = false, bool ClearZ = false);
		void ReSize(uint Width, uint Height);
		void GetDimension(float &Width, float &Height);

		uint RWidth, RHeight, Width, Height;

		D3DFORMAT TargetFormat;

		D3DVIEWPORT9 Viewport;
		D3DVIEWPORT9 previousViewport;

		IDirect3DTexture9* pTexture;
		IDirect3DSurface9* pTextureSurface; // reference to the surface of the texture
};