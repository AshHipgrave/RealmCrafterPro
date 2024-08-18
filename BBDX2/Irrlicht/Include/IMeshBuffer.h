// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#ifndef __I_MESH_BUFFER_H_INCLUDED__
#define __I_MESH_BUFFER_H_INCLUDED__

#include "IUnknown.h"
#include "SMaterial.h"
#include "irrArray.h"
#include "aabbox3d.h"
#include "S3DVertex.h"

#ifdef NDEBUG
#define D3D_DEBUG_INFO
#endif
#include <d3d9.h>
//#include <windows.h>

namespace irr
{
namespace scene
{
	//! Struct for holding a mesh with a single material
	/** SMeshBuffer is a simple implementation of a MeshBuffer. */
	class IMeshBuffer : public virtual IUnknown
	{
	public:

		// constructor!
		IMeshBuffer()
		{
			TextureAlpha = false;
			VertexAlpha = false;
			RenderTime = 3;
		}

		//! destructor
		virtual ~IMeshBuffer(){}; 

        //! returns the material of this meshbuffer
        virtual video::SMaterial& getMaterial() = 0;

		//! returns the material of this meshbuffer
        virtual const video::SMaterial& getMaterial() const = 0;

		//! returns which type of vertex data is stored.
		virtual video::E_VERTEX_TYPE getVertexType() const = 0;

		//! returns pointer to vertex data. The data is a array of vertices. Which vertex
		//! type is used can be determinated with getVertexType().
		virtual const void* getVertices() const = 0; 

		//! returns pointer to vertex data. The data is a array of vertices. Which vertex
		//! type is used can be determinated with getVertexType().
		virtual void* getVertices() = 0; 

		//! returns amount of vertices
		virtual s32 getVertexCount() const = 0;

		//! returns pointer to Indices
		virtual const u32* getIndices() const = 0;

		//! returns pointer to Indices
		virtual u32* getIndices() = 0;

		//! returns amount of indices
		virtual s32 getIndexCount() const = 0;

		//! returns an axis aligned bounding box
		virtual const core::aabbox3d<f32>& getBoundingBox() const = 0;

		//! returns an axis aligned bounding box
		virtual core::aabbox3d<f32>& getBoundingBox() = 0;


		void* vertexB;
		void* indexB; //It does not include the type of renderer that created the hardware buffer, that must be added if you plan to use multiple renderers
		int vcnt;
		int icnt;
		bool TextureAlpha;
		bool VertexAlpha;
		int RenderTime;


	};

} // end namespace scene
} // end namespace irr

#endif

