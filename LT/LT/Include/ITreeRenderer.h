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
//COPYRIGHTNOTICE

#pragma once

#include <string>

#include "IParameter.h"
#include "ITreeTexture.h"
#include "ITreeMeshBuffer.h"
#include "STreeTrunkVertex.h"
#include "STreeLeafVertex.h"
#include "STreeLODVertex.h"
#include <Color.h>

namespace LT
{
	//! Tree Renderer Interface
	class ITreeRenderer
	{
	public:

		virtual ~ITreeRenderer() {}

		//! Prepare for rendering, called before all trees are drawn.
		virtual bool PreRender() = 0;

		//! Finnish rendering, called after trees are drawn.
		virtual bool PostRender() = 0;

		//! Get the number of triangles drawn in the previous frame.
		virtual unsigned int GetTrianglesDrawn() const = 0;
		
		//! Create a buffer to hold trunk data
		virtual ITreeMeshBuffer* CreateTrunkBuffer(const STreeTrunkVertex* vertices, unsigned int vertexCount, const unsigned short* indices, unsigned int indexCount) = 0;

		//! Create a buffer to hold leaves
		virtual ITreeMeshBuffer* CreateLeafBuffer(const STreeLeafVertex* vertices, unsigned int vertexCount, const unsigned short* indices, unsigned int indexCount) = 0;

		//! Create a buffer to hold LOD billboards
		virtual ITreeMeshBuffer* CreateLODBuffer(const STreeLODVertex* vertices, unsigned int vertexCount, const unsigned short* indices, unsigned int indexCount) = 0;

		//! Free a mesh buffer
		virtual void FreeMeshBuffer(ITreeMeshBuffer* buffer) = 0;

		//! Get/Load a texture (useful for cache implementations)
		virtual ITreeTexture* GetTexture(const char* path, bool ismask = false) = 0;

		//! Free/Drop a texture
		virtual void FreeTexture(ITreeTexture* texture) = 0;

		//! Prepare to render trunk meshes
		virtual bool PreRenderTrunk(int rtIndex) = 0;

		//! Prepare to render leaves
		virtual bool PreRenderLeaf(int rtIndex) = 0;

		//! Prepare to render trunk meshes for depth rendering
		virtual bool PreRenderTrunkDepth() = 0;

		//! Prepare to render leaves meshed for depth rendering
		virtual bool PreRenderLeafDepth() = 0;

		//! Prepare to render LOD billboards
		virtual bool PreRenderLOD(int rtIndex) = 0;

		//! Finnish rendering trunks
		virtual void PostRenderTrunk() = 0;

		//! Finnish rendering leaves
		virtual void PostRenderLeaf() = 0;

		//! Finnish rendering trunks (depth)
		virtual void PostRenderTrunkDepth() = 0;

		//! Finnish rendering leaves (depth)
		virtual void PostRenderLeafDepth() = 0;
		
		//! Finnish rendering LOD Billboards
		virtual void PostRenderLOD() = 0;

		//! Commit buffer data (usually to a GPU)
		virtual bool SetMeshBuffer(const ITreeMeshBuffer* buffer) = 0;

		//! Set a specific texture for the next rendered object
		virtual bool SetTexture(const char* name, const ITreeTexture* texture) = 0;

		//! Set a paramter for the next rendered object
		virtual bool SetParameter(const char* name, const IParameter &parameter, bool applyToAll) = 0;
		virtual bool SetParameter(const char* name, const IParameter &parameter) = 0;

		//! Draw a mesh buffer
		virtual void DrawMeshBuffer(const ITreeMeshBuffer* buffer) = 0;

		//! Set the texture for shadow mapping
		virtual void SetShadowMap(void* handle) = 0;

		virtual void SetLightNormal(int index, NGin::Math::Vector3 &normal) = 0;
		virtual void SetLightColor(int index, NGin::Math::Color &color) = 0;
		virtual void SetAmbientLight(NGin::Math::Color &color) = 0;
		virtual void SetPointLights(void** lights, int count, int stride) = 0;
		virtual void SetFog(NGin::Math::Color& color, NGin::Math::Vector2& data) = 0;
		virtual void SetPointLightParameters(NGin::Math::Vector3 &position) = 0;
		virtual void SetSwayDirection(NGin::Math::Vector2 &direction) = 0;
		virtual void SetSwayPower(float power) = 0;

		virtual void SetQualityLevel(int qualityLevel) = 0;

		virtual void OnDeviceLost() = 0;
		virtual void OnDeviceReset() = 0;

	};
}
