// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#ifndef __I_ANIMATED_MESH_B3D_H_INCLUDED__
#define __I_ANIMATED_MESH_B3D_H_INCLUDED__

#include "IAnimatedMesh.h"
#include "ISceneNode.h"
#include "Quaternion.h"
#include <vector>

namespace irr
{
namespace scene
{
	struct VertexBlends;
	//! Interface for using some special functions of B3d meshes
	/** Please note that the B3d Mesh's frame numbers are scaled by 100 */
	class IAnimatedMeshB3d : public IAnimatedMesh
	{
	public:


		struct SB3dBone
		{
			s32 vertex_id;
			f32 weight;
		};

		struct SB3dKey
		{
			s32 frame;
			s32 flags;
			core::vector3df position;
			core::vector3df scale;
			core::quaternion rotation;
		};

		struct SB3dNode
		{
			//SB3dNode()
			//{
			//	char OO[1024];
			//	sprintf(OO, "Node Created (%i): %x\n", sizeof(SB3dNode), this);
			//	OutputDebugString(OO);
			//}

			//~SB3dNode()
			//{
			//	char OO[1024];
			//	sprintf(OO, "Node Destroyed: %x\n", this);
			//	OutputDebugString(OO);
			//}

			core::stringc Name;

			core::vector3df position;
			core::vector3df scale;
			core::quaternion rotation;

			core::vector3df Animatedposition;
			core::vector3df Animatedscale;
			core::quaternion Animatedrotation;
			core::matrix4 GlobalAnimatedMatrix;
			core::matrix4 LocalAnimatedMatrix;

			core::matrix4 LocalMatrix;
			core::matrix4 GlobalMatrix;
			core::matrix4 GlobalInversedMatrix;

			bool Animate; //Move this nodes local matrix when animating?

			std::vector<SB3dKey> Keys;

			core::array<SB3dBone> Bones;

			core::array<SB3dNode*> Nodes;

		};

		//! Returns a pointer to a transformation matrix of a part of the
		//! mesh based on a frame time. This is used for being able to attach
		//! objects to parts of animated meshes. For example a weapon to an animated
		//! hand.
		//! \param jointNumber: Zero based index of joint. The last joint has the number
		//! IAnimatedMeshB3d::getJointCount()-1;
		//! \param frame: Frame of the animation.
		//! \return Returns a pointer to the matrix of the mesh part or
		//! null if an error occured.
		virtual core::matrix4* getMatrixOfJoint(s32 jointNumber, s32 frame) = 0;


		//! Returns a pointer to a local matrix of a Joint, can be used to control the animation
		virtual core::matrix4* getLocalMatrixOfJoint(s32 jointNumber) = 0;

		//! Returns a pointer to a matrix of a part of the mesh unanimated
		virtual core::matrix4* getMatrixOfJointUnanimated(s32 jointNumber) = 0;


		//! Move this Joint's local matrix when animating
		//! \param jointNumber: Zero based index of joint. The last joint has the number
		//! IAnimatedMeshB3d::getJointCount()-1;
		//! \param On: False= Leave joint's local matrix, True= Animate
		//! (not used yet)
		virtual void setJointAnimation(s32 jointNumber, bool On) = 0;


		//! Gets joint count.
		//! \return Returns amount of joints in the skeletal animated mesh.
		virtual s32 getJointCount() const = 0;

		//! Gets the name of a joint.
		//! \param number: Zero based index of joint. The last joint has the number
		//! IAnimatedMeshB3d::getJointCount()-1;
		//! \return Returns name of joint and null if an error happened.
		virtual const c8* getJointName(s32 number) const = 0;

		//! Gets a joint number from its name
		//! \param name: Name of the joint.
		//! \return Returns the number of the joint or -1 if not found.
		virtual s32 getJointNumber(const c8* name) const = 0;

		//!Update Normals when Animating
		//!False= Don't (default)
		//!True= Update normals, slower
		virtual void updateNormalsWhenAnimating(bool on) = 0;


		//!Sets Interpolation Mode
		//!0- Constant
		//!1- Linear (default)
		virtual void SetInterpolationMode(s32 mode) = 0;

		//!Want should happen on when animating
		//!0-Nothing
		//!1-Update nodes only
		//!2-Update skin only
		//!3-Update both nodes and skin (default)
		virtual void SetAnimateMode(s32 mode) = 0;

		virtual scene::ISceneNode* getJoint(s32 number) = 0;
		virtual bool getEntityFXParam(irr::s32 param) = 0;

		virtual void recalculateBoundingBox() = 0;


		virtual core::array<IAnimatedMeshB3d::SB3dNode*>* GetAnimationNodes(u32 Frame, core::array<s32> &PrevKeys) = 0;
		virtual bool CopyBuffersToTangents() = 0;
		virtual bool IsAnimated()=0;
		virtual void SetAnimated(bool A) = 0;

		virtual irr::core::aabbox3df GetAnimatedBoundingBox(bool &hasValue) = 0;


		core::aabbox3d<f32> BoundingBox;
		core::array<scene::VertexBlends> VBlends;
		core::array<core::matrix4> BindPoseNodes;
		bool IsTangentMesh;

	};

} // end namespace scene
} // end namespace irr

#endif

