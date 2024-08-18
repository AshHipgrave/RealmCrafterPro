// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#ifndef __S_VIEW_FRUSTRUM_H_INCLUDED__
#define __S_VIEW_FRUSTRUM_H_INCLUDED__

#include "plane3d.h"
#include "vector3d.h"
#include "aabbox3d.h"
#include "matrix4.h"

namespace irr
{
namespace scene
{

	//! Defines the view frustrum. Thats the space viewed by the camera.
	/** The view frustrum is enclosed by 6 planes. These six planes share
	four points. A bounding box around these four points is also stored in
	this structure.
	*/
	struct SViewFrustrum
	{
		enum VFPLANES
		{
			//! Far plane of the frustrum. Thats the plane farest away from the eye.
			VF_FAR_PLANE = 0,
			//! Near plane of the frustrum. Thats the plane nearest to the eye.
			VF_NEAR_PLANE,
			//! Left plane of the frustrum.
			VF_LEFT_PLANE,
			//! Right plane of the frustrum.
			VF_RIGHT_PLANE,
			//! Bottom plane of the frustrum.
			VF_BOTTOM_PLANE,
			//! Top plane of the frustrum.
			VF_TOP_PLANE,

			//! Amount of planes enclosing the view frustum. Should be 6.
			VF_PLANE_COUNT
		};

		//! Default Constructor
		SViewFrustrum() {};

		//! This constructor creates a view frustrum based on a projection and/or
		//! view matrix.
		SViewFrustrum(const core::matrix4& mat);

		//! the position of the camera
		core::vector3df cameraPosition;

		//! all planes enclosing the view frustrum.
		core::plane3d<f32> planes[VF_PLANE_COUNT];

		//! bouding box around the view frustrum
		core::aabbox3d<f32> boundingBox;

		//! transforms the frustrum by the matrix
		//! \param mat: Matrix by which the view frustrum is transformed.
		void transform(const core::matrix4 &mat);

		//! returns the point which is on the far left upper corner inside the
		//! the view frustrum.
		core::vector3df getFarLeftUp() const;

		//! returns the point which is on the far left bottom corner inside the
		//! the view frustrum.
		core::vector3df getFarLeftDown() const;

		//! returns the point which is on the far right top corner inside the
		//! the view frustrum.
		core::vector3df getFarRightUp() const;

		//! returns the point which is on the far right bottom corner inside the
		//! the view frustrum.
		core::vector3df getFarRightDown() const;

		//! returns a bounding box enclosing the whole view frustrum
		core::aabbox3d<f32> getBoundingBox() const;

		//! recalculates the bounding box member based on the planes
		inline void recalculateBoundingBox();
	};


	//! transforms the furstum by the matrix
	//! \param Matrix by which the view frustrum is transformed.
	inline void SViewFrustrum::transform(const core::matrix4 &mat)
	{
		for (int i=0; i<VF_PLANE_COUNT; ++i)
			mat.transformPlane(planes[i]);

		mat.transformVect(cameraPosition);

		recalculateBoundingBox();
	}


	//! returns the point which is on the far left upper corner inside the
	//! the view frustrum.
	inline core::vector3df SViewFrustrum::getFarLeftUp() const
	{
		core::vector3df p;
		planes[scene::SViewFrustrum::VF_FAR_PLANE].getIntersectionWithPlanes(
			planes[scene::SViewFrustrum::VF_TOP_PLANE],
			planes[scene::SViewFrustrum::VF_LEFT_PLANE], p);

		return p;
	}

	//! returns the point which is on the far left bottom corner inside the
	//! the view frustrum.
	inline core::vector3df SViewFrustrum::getFarLeftDown() const
	{
		core::vector3df p;
		planes[scene::SViewFrustrum::VF_FAR_PLANE].getIntersectionWithPlanes(
			planes[scene::SViewFrustrum::VF_BOTTOM_PLANE],
			planes[scene::SViewFrustrum::VF_LEFT_PLANE], p);

		return p;
	}

	//! returns the point which is on the far right top corner inside the
	//! the view frustrum.
	inline core::vector3df SViewFrustrum::getFarRightUp() const
	{
		core::vector3df p;
		planes[scene::SViewFrustrum::VF_FAR_PLANE].getIntersectionWithPlanes(
			planes[scene::SViewFrustrum::VF_TOP_PLANE],
			planes[scene::SViewFrustrum::VF_RIGHT_PLANE], p);

		return p;
	}

	//! returns the point which is on the far right bottom corner inside the
	//! the view frustrum.
	inline core::vector3df SViewFrustrum::getFarRightDown() const
	{
		core::vector3df p;
		planes[scene::SViewFrustrum::VF_FAR_PLANE].getIntersectionWithPlanes(
			planes[scene::SViewFrustrum::VF_BOTTOM_PLANE],
			planes[scene::SViewFrustrum::VF_RIGHT_PLANE], p);

		return p;
	}


	//! returns a bounding box encosing the whole view frustrum
	inline core::aabbox3d<f32> SViewFrustrum::getBoundingBox() const
	{
		return boundingBox;
	}


	//! recalculates the bounding box member based on the planes
	inline void SViewFrustrum::recalculateBoundingBox()
	{
		core::aabbox3d<f32> box(cameraPosition);

		box.addInternalPoint(getFarLeftUp());
		box.addInternalPoint(getFarRightUp());
		box.addInternalPoint(getFarLeftDown());
		box.addInternalPoint(getFarRightDown());

		boundingBox = box;
	}


	//! This constructor creates a view frustrum based on a projection and/or
	//! view matrix.
	inline SViewFrustrum::SViewFrustrum(const core::matrix4& mat)
	{
		#define sw(a,b)		(mat((b),(a)))

		// left clipping plane
		planes[SViewFrustrum::VF_LEFT_PLANE].Normal.X = -(sw(0,3) + sw(0,0));
		planes[SViewFrustrum::VF_LEFT_PLANE].Normal.Y = -(sw(1,3) + sw(1,0));
		planes[SViewFrustrum::VF_LEFT_PLANE].Normal.Z = -(sw(2,3) + sw(2,0));
		planes[SViewFrustrum::VF_LEFT_PLANE].D =		  -(sw(3,3) + sw(3,0));

		// right clipping plane
		planes[SViewFrustrum::VF_RIGHT_PLANE].Normal.X = -(sw(0,3) - sw(0,0));
		planes[SViewFrustrum::VF_RIGHT_PLANE].Normal.Y = -(sw(1,3) - sw(1,0));
		planes[SViewFrustrum::VF_RIGHT_PLANE].Normal.Z = -(sw(2,3) - sw(2,0));
		planes[SViewFrustrum::VF_RIGHT_PLANE].D =        -(sw(3,3) - sw(3,0));

		// top clipping plane
		planes[SViewFrustrum::VF_TOP_PLANE].Normal.X = -(sw(0,3) - sw(0,1));
		planes[SViewFrustrum::VF_TOP_PLANE].Normal.Y = -(sw(1,3) - sw(1,1));
		planes[SViewFrustrum::VF_TOP_PLANE].Normal.Z = -(sw(2,3) - sw(2,1));
		planes[SViewFrustrum::VF_TOP_PLANE].D =        -(sw(3,3) - sw(3,1));

		// bottom clipping plane
		planes[SViewFrustrum::VF_BOTTOM_PLANE].Normal.X = -(sw(0,3) + sw(0,1));
		planes[SViewFrustrum::VF_BOTTOM_PLANE].Normal.Y = -(sw(1,3) + sw(1,1));
		planes[SViewFrustrum::VF_BOTTOM_PLANE].Normal.Z = -(sw(2,3) + sw(2,1));
		planes[SViewFrustrum::VF_BOTTOM_PLANE].D =        -(sw(3,3) + sw(3,1));

		// near clipping plane
		planes[SViewFrustrum::VF_NEAR_PLANE].Normal.X = -sw(0,2);
		planes[SViewFrustrum::VF_NEAR_PLANE].Normal.Y = -sw(1,2);
		planes[SViewFrustrum::VF_NEAR_PLANE].Normal.Z = -sw(2,2);
		planes[SViewFrustrum::VF_NEAR_PLANE].D =        -sw(3,2);

		// far clipping plane
		planes[SViewFrustrum::VF_FAR_PLANE].Normal.X = -(sw(0,3) - sw(0,2));
		planes[SViewFrustrum::VF_FAR_PLANE].Normal.Y = -(sw(1,3) - sw(1,2));
		planes[SViewFrustrum::VF_FAR_PLANE].Normal.Z = -(sw(2,3) - sw(2,2));
		planes[SViewFrustrum::VF_FAR_PLANE].D =        -(sw(3,3) - sw(3,2));

		// normalize normals

		for (s32 i=0; i<6; ++i)
		{
			f32 len = (f32)(1.0f / planes[i].Normal.getLength());
			planes[i].Normal *= len;
			planes[i].D *= len;
		}

		// make bounding box

		recalculateBoundingBox();
	}



} // end namespace scene
} // end namespace irr


#include <AABB.h>
#include <d3dx9.h>

namespace RealmCrafter
{
	namespace RCT
	{

		// View Frustum class
		class Frustum
		{
		public:

			// Planes that make frustum
			D3DXPLANE Planes[6];

			// Set this Frustums' values using the combined VP
			void FromViewProjection(D3DXMATRIX &viewProjection)
			{
				// Left plane
				Planes[0].a = viewProjection._14 + viewProjection._11;
				Planes[0].b = viewProjection._24 + viewProjection._21;
				Planes[0].c = viewProjection._34 + viewProjection._31;
				Planes[0].d = viewProjection._44 + viewProjection._41;

				// Right plane
				Planes[1].a = viewProjection._14 - viewProjection._11;  
				Planes[1].b = viewProjection._24 - viewProjection._21;
				Planes[1].c = viewProjection._34 - viewProjection._31;
				Planes[1].d = viewProjection._44 - viewProjection._41;

				// Top plane
				Planes[2].a = viewProjection._14 - viewProjection._12;  
				Planes[2].b = viewProjection._24 - viewProjection._22;
				Planes[2].c = viewProjection._34 - viewProjection._32;
				Planes[2].d = viewProjection._44 - viewProjection._42;

				// Bottom plane
				Planes[3].a = viewProjection._14 + viewProjection._12;  
				Planes[3].b = viewProjection._24 + viewProjection._22;
				Planes[3].c = viewProjection._34 + viewProjection._32;
				Planes[3].d = viewProjection._44 + viewProjection._42;

				// Near plane
				Planes[4].a = viewProjection._13;  
				Planes[4].b = viewProjection._23;
				Planes[4].c = viewProjection._33;
				Planes[4].d = viewProjection._43;

				// Far plane
				Planes[5].a = viewProjection._14 - viewProjection._13;  
				Planes[5].b = viewProjection._24 - viewProjection._23;
				Planes[5].c = viewProjection._34 - viewProjection._33;
				Planes[5].d = viewProjection._44 - viewProjection._43; 

				for(int i = 0; i < 6; ++i)
					D3DXPlaneNormalize(&Planes[i], &Planes[i]);
			}

			// Check if a sphere is in the frustum
			bool SphereInFrustum(NGin::Math::Vector3 &Point, float Radius)
			{
				D3DXVECTOR4 P(Point.X, Point.Y, Point.Z, 1.0f);
				for(int i = 0; i < 6; ++i)
					if(D3DXPlaneDot(&Planes[i], &P) + Radius < 0.0f)
						return false;
				return true;
			}

			// Check if a Box is in the Frustum
			int BoxInFrustum(NGin::Math::AABB &Box)
			{
				bool Intersect = false;
				int Result = 0;
				NGin::Math::Vector3 Min, Max;

				for(int i = 0; i < 6; ++i)
				{
					if(Planes[i].a > 0)
					{
						Min.X = Box._Min.X;
						Max.X = Box._Max.X;
					}
					else
					{
						Min.X = Box._Max.X;
						Max.X = Box._Min.X;
					}

					if(Planes[i].b > 0)
					{
						Min.Y = Box._Min.Y;
						Max.Y = Box._Max.Y;
					}
					else
					{
						Min.Y = Box._Max.Y;
						Max.Y = Box._Min.Y;
					}

					if(Planes[i].c > 0)
					{
						Min.Z = Box._Min.Z;
						Max.Z = Box._Max.Z;
					}
					else
					{
						Min.Z = Box._Max.Z;
						Max.Z = Box._Min.Z;
					}

					D3DXVECTOR3 pMin(Min.X, Min.Y, Min.Z);
					D3DXVECTOR3 pMax(Max.X, Max.Y, Max.Z);

					D3DXVECTOR4 p4Min(Min.X, Min.Y, Min.Z, 1);
					D3DXVECTOR4 p4Max(Max.X, Max.Y, Max.Z, 1);

					D3DXVECTOR3 pNorm(Planes[i].a, Planes[i].b, Planes[i].c);

					if(D3DXPlaneDotCoord(&Planes[i], &pMax) < 0)
					{
						Result = 0;
						return Result;
					}

					if(D3DXPlaneDotCoord(&Planes[i], &pMin) < 0)
						Intersect = true;
				}

				if(Intersect)
					Result = 1;
				else
					Result = 2;

				return Result;
			}
		};
	}
}


#endif

