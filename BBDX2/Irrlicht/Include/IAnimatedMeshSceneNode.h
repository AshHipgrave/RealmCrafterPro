// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#ifndef __I_ANIMATED_MESH_SCENE_NODE_H_INCLUDED__
#define __I_ANIMATED_MESH_SCENE_NODE_H_INCLUDED__

#include "ISceneNode.h"
#include "IAnimatedMesh.h"

class ShaderLoader;

namespace irr
{
namespace scene
{
	class IAnimatedMeshSceneNode;

	//! Callback interface for catching events of ended animations.
	/** Implement this interface and use 
	 IAnimatedMeshSceneNode::setAnimationEndCallback to be able to
	 be notified if an animation playback has ended.
	**/
	class IAnimationEndCallBack : public virtual IUnknown
	{
	public:

		//! Will be called when the animation playback has ended.
		//! See IAnimatedMeshSceneNode::setAnimationEndCallback for 
		//! more informations.
		//! \param node: Node of which the animation has ended.
		virtual void OnAnimationEnd(IAnimatedMeshSceneNode* node) = 0;
	};

	//! Scene node capable of displaying an animated mesh and its shadow.
	/** The shadow is optional: If a shadow should be displayed too, just invoke
	the IAnimatedMeshSceneNode::createShadowVolumeSceneNode().*/
	class IAnimatedMeshSceneNode : public ISceneNode
	{
	public:
		

		//! Constructor
		IAnimatedMeshSceneNode(ISceneNode* parent, ISceneManager* mgr, s32 id,
			const core::vector3df& position = core::vector3df(0,0,0),
			const core::vector3df& rotation = core::vector3df(0,0,0),
			const core::vector3df& scale = core::vector3df(1.0f, 1.0f, 1.0f))
			: ISceneNode(parent, mgr, id, position, rotation, scale)
		{
			PublicName = "";
			CSeq = 0;
			CLen = 0;
			Anicnt = 0;
			for(int i = 0; i < 100; ++i)
			{
				AnimSeq[i][0] = 0;
				AnimSeq[i][1] = 0;
			}
			IsAnimated = false;
		}

		//! Destructor
		virtual ~IAnimatedMeshSceneNode() {};

		//! Sets the current frame number. 
		//! From now on the animation is played from this frame. 
		//! \param frame: Number of the frame to let the animation be started from.
		//! The frame number must be a valid frame number of the IMesh used by this 
		//! scene node. Set IAnimatedMesh::getMesh() for details.
		virtual void setCurrentFrame(s32 frame) = 0;

		//! Sets the frame numbers between the animation is looped.
		//! The default is 0 - MaximalFrameCount of the mesh.
		//! \param begin: Start frame number of the loop.
		//! \param end: End frame number of the loop.
		//! \return Returns true if successful, false if not.
		virtual bool setFrameLoop(s32 begin, s32 end) = 0;

		//! Sets the speed with witch the animation is played.
		//! \param framesPerSecond: Frames per second played.
		virtual void setAnimationSpeed(s32 framesPerSecond) = 0;

		//! Returns a pointer to a child node, wich has the same transformation as 
		//! the corrsesponding joint, if the mesh in this scene node is a x mesh.
		//! Otherwise 0 is returned. With this method it is possible to
		//! attach scene nodes to joints more easily. In this way, it is
		//! for example possible to attach a weapon to the left hand of an
		//! animated model. This example shows how:
		//! \code
		//! ISceneNode* hand = 
		//!		yourMS3DAnimatedMeshSceneNode->getXJointNode("LeftHand");
		//! hand->addChild(weaponSceneNode);
		//! \endcode
		//! Please note that the SceneNode returned by this method may not exist
		//! before this call and is created by it.
		//! \param jointName: Name of the joint.
		//! \return Returns a pointer to the scene node which represents the joint
		//! with the specified name. Returns 0 if the contained mesh is not an
		//! ms3d mesh or the name of the joint could not be found.
		virtual ISceneNode* getXJointNode(const c8* jointName) = 0;

			//! Returns a pointer to a child node, wich has the same transformation as
		//! the corrsesponding joint, if the mesh in this scene node is a b3d mesh.
		//! Otherwise 0 is returned. With this method it is possible to
		//! attach scene nodes to joints more easily. In this way, it is
		//! for example possible to attach a weapon to the left hand of an
		//! animated model. This example shows how:
		//! \code
		//! ISceneNode* hand =
		//!		yourB3DAnimatedMeshSceneNode->getB3DJointNode("LeftHand");
		//! hand->addChild(weaponSceneNode);
		//! \endcode
		//! Please note that the SceneNode returned by this method may not exist
		//! before this call and is created by it.
		//! \param jointName: Name of the joint.
		//! \return Returns a pointer to the scene node which represents the joint
		//! with the specified name. Returns 0 if the contained mesh is not an
		//! b3d mesh or the name of the joint could not be found.
		virtual ISceneNode* getB3DJointNode(const c8* jointName) = 0;
		virtual ISceneNode* getB3DJointNode(s32 number) = 0;
		virtual s32 getB3DJointCount() = 0;

		//! Returns the current displayed frame number.
		virtual s32 getFrameNr() = 0;

		//! Sets looping mode which is on by default. If set to false,
		//! animations will not be played looped.
		virtual void setLoopMode(bool playAnimationLooped) = 0;

		//! Sets a callback interface which will be called if an animation
		//! playback has ended. Set this to 0 to disable the callback again.
		//! Please note that this will only be called when in non looped mode,
		//! see IAnimatedMeshSceneNode::setLoopMode().
		virtual void setAnimationEndCallback(IAnimationEndCallBack* callback=0) = 0;

		//! Sets if the scene node should not copy the materials of the mesh but use them in a read only style.
		/* In this way it is possible to change the materials a mesh causing all mesh scene nodes 
		referencing this mesh to change too. */
		virtual void setReadOnlyMaterials(bool readonly) = 0;

		//! Returns if the scene node should not copy the materials of the mesh but use them in a read only style
		virtual bool isReadOnlyMaterials() = 0;

		//! Sets a new mesh
		virtual void setMesh(IAnimatedMesh* mesh) = 0;

		//! Sets a new mesh LOD
		virtual void setMeshLOD_LOW(IAnimatedMesh* mesh, float Distance ) = 0;
		virtual void setMeshLOD_MEDIUM(IAnimatedMesh* mesh, float Distance ) = 0;
		virtual void setMeshLOD_HIGH(IAnimatedMesh* mesh, float Distance ) = 0;

		virtual void setAnimationCPY() = 0;
		virtual s32 getAnimationSpeed() = 0;
		
		virtual void setInheritAnimation(bool inherit) = 0;
		virtual bool getInheritAnimation() = 0;

		//JA Gets the Mesh from the class
		virtual IAnimatedMesh* getLocalMesh() = 0;

		//JA Sets RCFX shader
		virtual void setRCFXShader(ShaderLoader* shd) = 0;
		virtual ShaderLoader* getRCFXShader() = 0;
		
		s32 BeginFrameTime;
		s32 StartFrame;
		s32 EndFrame;
		s32 FramesPerSecond;

		int CSeq;
		int CLen;
		int Anicnt;
		int AnimSeq[100][2];
		core::aabbox3d<f32> Box;
		bool IsAnimated;
		core::stringc PublicName;

		// Level of Detail
		IAnimatedMesh* pMeshLOD_HIGH, *pMeshLOD_MEDIUM, *pMeshLOD_LOW;
		float DistLOD_LOW, DistLOD_MEDIUM, DistLOD_HIGH;
	};

} // end namespace scene
} // end namespace irr

#endif

