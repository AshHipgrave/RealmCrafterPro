// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#ifndef __C_ANIMATED_MESH_SCENE_NODE_H_INCLUDED__
#define __C_ANIMATED_MESH_SCENE_NODE_H_INCLUDED__

#include "IAnimatedMeshSceneNode.h"
#include "IAnimatedMesh.h"
#include "CAnimatedMeshB3d.h"

namespace irr
{
namespace scene
{
	class IDummyTransformationSceneNode;

	class CAnimatedMeshSceneNode : public IAnimatedMeshSceneNode
	{
	public:

		//! constructor
		CAnimatedMeshSceneNode(IAnimatedMesh* mesh, ISceneNode* parent, ISceneManager* mgr,	s32 id,
			const core::vector3df& position = core::vector3df(0,0,0),
			const core::vector3df& rotation = core::vector3df(0,0,0),
			const core::vector3df& scale = core::vector3df(1.0f, 1.0f, 1.0f));

		//! destructor
		virtual ~CAnimatedMeshSceneNode();

		//! sets the current frame. from now on the animation is played from this frame.
		virtual void setCurrentFrame(s32 frame);

		//! OnPostRender() is called just after rendering the whole scene.
		virtual void OnPostRender(u32 timeMs);

		//! frame
		virtual void OnPreRender();

		//! renders the node.
		virtual void render();

		virtual void renderBuffer(int Index);

		//! returns the axis aligned bounding box of this node
		virtual const core::aabbox3d<f32>& getBoundingBox() const;

		//! sets the frames between the animation is looped.
		//! the default is 0 - MaximalFrameCount of the mesh.
		virtual bool setFrameLoop(s32 begin, s32 end);

		//! Sets looping mode which is on by default. If set to false,
		//! animations will not be looped.
		virtual void setLoopMode(bool playAnimationLooped);

		//! Sets a callback interface which will be called if an animation
		//! playback has ended. Set this to 0 to disable the callback again.
		virtual void setAnimationEndCallback(IAnimationEndCallBack* callback=0);

		//! sets the speed with witch the animation is played
		virtual void setAnimationSpeed(s32 framesPerSecond);

		//! returns the material based on the zero based index i. To get the amount
		//! of materials used by this scene node, use getMaterialCount().
		//! This function is needed for inserting the node into the scene hirachy on a
		//! optimal position for minimizing renderstate changes, but can also be used
		//! to directly modify the material of a scene node.
		virtual video::SMaterial& getMaterial(s32 i);
		
		//! returns amount of materials used by this scene node.
		virtual s32 getMaterialCount();

		//! Returns a pointer to a child node, wich has the same transformation as 
		//! the corrsesponding joint, if the mesh in this scene node is a x mesh.
		virtual ISceneNode* getXJointNode(const c8* jointName);

		//! Returns a pointer to a child node, which has the same transformation as
		//! the corresponding joint, if the mesh in this scene node is a b3d mesh.
		virtual ISceneNode* getB3DJointNode(const c8* jointName);
		virtual ISceneNode* getB3DJointNode(s32 number);
		virtual s32 getB3DJointCount();

		//! Removes a child from this scene node.
		//! Implemented here, to be able to remove the shadow properly, if there is one,
		//! or to remove attached childs.
		virtual bool removeChild(ISceneNode* child);

		//! Returns the current displayed frame number.
		virtual s32 getFrameNr();

		//! Sets if the scene node should not copy the materials of the mesh but use them in a read only style.
		/* In this way it is possible to change the materials a mesh causing all mesh scene nodes 
		referencing this mesh to change too. */
		virtual void setReadOnlyMaterials(bool readonly);

		//! Returns if the scene node should not copy the materials of the mesh but use them in a read only style
		virtual bool isReadOnlyMaterials();

		//! Sets a new mesh
		virtual void setMesh(IAnimatedMesh* mesh);

		//! Sets a new mesh LOD
		virtual void setMeshLOD_LOW(IAnimatedMesh* mesh, float Distance ) override;
		virtual void setMeshLOD_MEDIUM(IAnimatedMesh* mesh, float Distance ) override;
		virtual void setMeshLOD_HIGH(IAnimatedMesh* mesh, float Distance ) override;

		//! Writes attributes of the scene node.
		virtual void serializeAttributes(io::IAttributes* out, io::SAttributeReadWriteOptions* options=0);

		//! Reads attributes of the scene node.
		virtual void deserializeAttributes(io::IAttributes* in, io::SAttributeReadWriteOptions* options=0);

		//! Returns type of the scene node
		virtual ESCENE_NODE_TYPE getType() { return ESNT_ANIMATED_MESH; }

		virtual s32 getAnimationSpeed() { return OFPS; }

		virtual void setAnimationCPY();

		virtual void setInheritAnimation(bool inherit);
		virtual bool getInheritAnimation();

		//JA Gets the Mesh from the class
		virtual IAnimatedMesh* getLocalMesh();

		//JA Sets RXFX shader
		virtual void setRCFXShader(ShaderLoader* shd);
		virtual ShaderLoader* getRCFXShader() { return FXFile; }

		virtual void WriteSceneGraph(irr::io::IWriteFile* File, core::stringc Tabbage)
		{

			char o[128];
			sprintf(o, "%sCAnimatedMeshSceneNode(%x, %s, %s)\r\n", Tabbage.c_str(), this, Name.c_str(), "");//this->PublicName.c_str());
			core::stringc Write = o;

			File->write(o, Write.size());
			WriteDesc(File, Tabbage);
		}

		//int CSeq;
		//int CLen;
		//int Anicnt;
		//int AnimSeq[100][1];

		s32 BeginFrameTime;
		s32 StartFrame;
		s32 EndFrame;
		s32 FramesPerSecond;
		s32 OFPS;
core::array<IDummyTransformationSceneNode* > JointChildSceneNodes;
core::array<core::matrix4> MatNodes;
core::array<s32> PrevKeys;
		
int lastframe;

	private:

		core::array<video::SMaterial> Materials;
		IAnimatedMesh*	Mesh;
		IAnimatedMesh* unsetMesh_;
		bool meshSet_;

	private:
		ShaderLoader* FXFile;

		bool Looping;
		bool ReadOnlyMaterials;
		bool InheritAnimation;

		IAnimationEndCallBack* LoopCallBack;
		s32 PassCount;
	};

} // end namespace scene
} // end namespace irr

#endif

