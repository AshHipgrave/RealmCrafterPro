// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h


//B3D file loader by Luke Hoschke, File format by Mark Sibly



#ifndef __C_ANIMATED_MESH_B3D_H_INCLUDED__
#define __C_ANIMATED_MESH_B3D_H_INCLUDED__

#include "IAnimatedMeshB3d.h"
#include "IReadFile.h"
#include "CSceneManager.h"


#include "CEmptySceneNode.h"

#include "CAnimatedMeshSceneNode.h"



//#include "IAnimatedMeshSceneNode.h"

#include "S3DVertex.h"
#include "irrString.h"
#include "matrix4.h"
#include "SMeshBuffer.h"

#include <quaternion.h>

#include <windows.h>

namespace irr
{
namespace video
{
	class IVideoDriver;
}
namespace scene
{

	class CAnimatedMeshSceneNode;

	class CAnimatedMeshB3d : public IAnimatedMeshB3d, public IMesh
	{
	public:

		struct SB3dTexture
		{
			irr::video::ITexture* Texture;
			s32 Flags;
			s32 Blend;
			f32 Xpos;
			f32 Ypos;
			f32 Xscale;
			f32 Yscale;
			f32 Angle;
		};

		struct SB3dMaterial
		{
			irr::video::SMaterial* Material;
			f32 red, green, blue, alpha;
			f32 shininess;
			s32 blend,fx;
			SB3dTexture *Textures[4];

		};

		/*struct SB3dBone
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

			core::array<SB3dKey> Keys;

			core::array<SB3dBone> Bones;

			core::array<SB3dNode*> Nodes;

		};*/

		//! constructor
		CAnimatedMeshB3d(video::IVideoDriver* driver, bool IsAnimated);

		//! destructor
		virtual ~CAnimatedMeshB3d();

		//! loads an B3d file
		virtual bool loadFile(ISceneManager* smgr, io::IReadFile* file, bool threaded);

		//! returns the amount of frames in milliseconds. If the amount is 1, it is a static (=non animated) mesh.
		virtual s32 getFrameCount();

		//! returns the animated mesh based on a detail level. 0 is the lowest, 255 the highest detail. Note, that some Meshes will ignore the detail level.
		virtual IMesh* getMesh(s32 frame, s32 detailLevel=255, s32 startFrameLoop=-1, s32 endFrameLoop=-1);

		//! returns amount of mesh buffers.
		virtual s32 getMeshBufferCount();

		//! returns pointer to a mesh buffer
		virtual IMeshBuffer* getMeshBuffer(s32 nr);

		//! returns an axis aligned bounding box
		virtual const core::aabbox3d<f32>& getBoundingBox() const;

		//! returns an axis aligned bounding box
		virtual core::aabbox3d<f32>& getBoundingBox();

		//! sets a flag of all contained materials to a new value
		virtual void setMaterialFlag(video::E_MATERIAL_FLAG flag, bool newvalue);

		//! Returns the type of the animated mesh.
		virtual E_ANIMATED_MESH_TYPE getMeshType() const;

		//! Returns a pointer to a transformation matrix of a part of the
		//! mesh based on a frame time.
		virtual core::matrix4* getMatrixOfJoint(s32 jointNumber, s32 frame);

		//! Gets joint count.
		virtual s32 getJointCount() const;

		//! Gets the name of a joint.
		virtual const c8* getJointName(s32 number) const;

		//! Gets a joint number from its name
		virtual s32 getJointNumber(const c8* name) const;


		virtual core::matrix4* getLocalMatrixOfJoint(s32 jointNumber);

		virtual core::matrix4* getMatrixOfJointUnanimated(s32 jointNumber);

		virtual void setJointAnimation(s32 jointNumber, bool On);

		//!Update Normals when Animating
		//!False= Don't (default)
		//!True= Update normals, slower
		virtual void updateNormalsWhenAnimating(bool on);


		//!Sets Interpolation Mode
		//!0- Constant
		//!1- Linear (default)
		virtual void SetInterpolationMode(s32 mode);



		//!Want should happen on when animating
		//!0-Nothing
		//!1-Update nodes only
		//!2-Update skin only
		//!3-Update both nodes and skin (default)
		virtual void SetAnimateMode(s32 mode);

		virtual ISceneNode* getJoint(s32 number);


		virtual bool getEntityFXParam(irr::s32 param);


		core::aabbox3d<f32> BoundingBox;





		struct SB3DMeshBuffer : public IMeshBuffer
		{
			SB3DMeshBuffer()
			{
				#ifdef _DEBUG
				setDebugName("SB3DMeshBuffer");
				#endif
				icnt = 0;
				vcnt = 0;
				indexB=0;
				vertexB=0; 
			}

			~SB3DMeshBuffer() {};

			virtual const video::SMaterial& getMaterial() const
			{
				return Material;
			}

			video::SMaterial& getMaterial()
			{
				return Material;
			}

			virtual const void* getVertices() const
			{
				return Vertices.const_pointer();
			}

			virtual void* getVertices()
			{
				return Vertices.pointer();
			}

			virtual s32 getVertexCount() const
			{
				return Vertices.size();
			}

			virtual const u32* getIndices() const
			{
				return Indices.const_pointer();
			}

			virtual u32* getIndices()
			{
				return Indices.pointer();
			}

			virtual s32 getIndexCount() const
			{
				return Indices.size();
			}

			virtual const core::aabbox3d<f32>& getBoundingBox() const
			{
				return BoundingBox;
			}

			virtual core::aabbox3d<f32>& getBoundingBox()
			{
				return BoundingBox;
			}

			void recalculateBoundingBox()
			{
				if (Vertices.empty())
					BoundingBox.reset(0,0,0);
				else
				{
					BoundingBox.reset(Vertices[0].Pos);
					for (u32 i=1; i<Vertices.size(); ++i)
						BoundingBox.addInternalPoint(Vertices[i].Pos);
				}
			}

			virtual video::E_VERTEX_TYPE getVertexType() const
			{
				return video::EVT_2TCOORDS;
				//return video::EVT_STANDARD;
			}

			video::SMaterial Material;
			core::array<video::S3DVertex2TCoords> Vertices;

			//core::array<video::S3DVertex> Vertices;

			core::array<u32> Indices;
			core::aabbox3d<f32> BoundingBox;

		};

		virtual void recalculateBoundingBox()
		{
			bool rst = true;
			for(u32 i = 0; i < this->getMeshBufferCount(); ++i)
			{
				if(this->getMeshBuffer(i)->getVertexType() == video::EVT_2TCOORDS)
				{
				((SB3DMeshBuffer*)this->getMeshBuffer(i))->recalculateBoundingBox();
				if(rst)
				{
					this->BoundingBox.reset(((SB3DMeshBuffer*)this->getMeshBuffer(i))->BoundingBox.MinEdge);
					rst = false;
				}
				this->BoundingBox.addInternalPoint(((SB3DMeshBuffer*)this->getMeshBuffer(i))->BoundingBox.MinEdge);
				this->BoundingBox.addInternalPoint(((SB3DMeshBuffer*)this->getMeshBuffer(i))->BoundingBox.MaxEdge);
				}else if(this->getMeshBuffer(i)->getVertexType() == video::EVT_TANGENTS)
				{
					((SMeshBufferTangents*)this->getMeshBuffer(i))->recalculateBoundingBox();
					if(rst)
					{
						this->BoundingBox.reset(((SMeshBufferTangents*)this->getMeshBuffer(i))->BoundingBox.MinEdge);
						rst = false;
					}
					this->BoundingBox.addInternalPoint(((SMeshBufferTangents*)this->getMeshBuffer(i))->BoundingBox.MinEdge);
					this->BoundingBox.addInternalPoint(((SMeshBufferTangents*)this->getMeshBuffer(i))->BoundingBox.MaxEdge);
				}
			}
		}


		
		virtual core::array<SB3dNode*>* GetAnimationNodes(u32 Frame, core::array<s32> &PrevKeys)
		{
			animate(Frame, 0, 0, PrevKeys);
			return &NodesL;
			//return &ShaderBonesTrans;
			/*core::array<SB3dNode*> NodesList;
			NodesL.clear();


			for(int i = 0; i < this->Nodes.size(); ++i)
			{
				GetNodes(this->Nodes[i],(core::array<SB3dNode*>*)&NodesList);
			}


			for(int i = 0 ; i < NodesList.size(); ++i)
			{
				bool Found = false;
				for(int f = 0; f < NodesL.size(); ++f)
					if(NodesL[f] == NodesList[i])
						Found = true;
				if(!Found)
					NodesL.push_back(NodesList[i]);
			}
			//return &BindPoseNodes;
			return &NodesL;*/

			


		}
		virtual bool CopyBuffersToTangents();
		virtual bool IsAnimated();
		virtual void SetAnimated(bool A)
		{
			_IsAnimated = A;
		}

		virtual irr::core::aabbox3df GetAnimatedBoundingBox(bool &hasValue)
		{
			if(FrameBoxes.size() > 0)
			{
				hasValue = true;
				return FrameBoxes[0];
			}

			hasValue = false;
			return irr::core::aabbox3df();
		}

		virtual bool getIsLoaded()
		{
			return isLoaded_;
		}

		virtual void setIsLoaded(bool isLoaded)
		{
			isLoaded_ = isLoaded;
		}


private:

	bool isLoaded_;


		struct B3dChunk
		{
			c8 name[4];
			s32 length;
			s32 startposition;
		};

public:
		s32 AnimFlags;
		s32 AnimFrames; //how many frames in anim
		f32 AnimFPS;
private:

		bool _IsAnimated;
		bool Threaded;

	  	

		core::array<SB3dNode*> NodesL;

		core::array<SB3dNode*> Nodes;

		core::array<SB3dNode*> RootNodes;
		core::array<irr::core::aabbox3df> FrameBoxes;
		

		

		bool ReadChunkTEXS(io::IReadFile* file, B3dChunk *B3dStack, s16 &B3dStackSize);

		bool ReadChunkBRUS(io::IReadFile* file, B3dChunk *B3dStack, s16 &B3dStackSize);

		bool ReadChunkMESH(io::IReadFile* file, B3dChunk *B3dStack, s16 &B3dStackSize, SB3dNode *InNode);

		bool ReadChunkVRTS(io::IReadFile* file, B3dChunk *B3dStack, s16 &B3dStackSize, SB3dNode *InNode, SB3DMeshBuffer *MeshBuffer, s32 Vertices_Start);

		bool ReadChunkTRIS(io::IReadFile* file, B3dChunk *B3dStack, s16 &B3dStackSize, SB3dNode *InNode, SB3DMeshBuffer *MeshBuffer, s32 Vertices_Start);

		bool ReadChunkNODE(io::IReadFile* file, B3dChunk *B3dStack, s16 &B3dStackSize, SB3dNode *InNode);


		bool ReadChunkBONE(io::IReadFile* file, B3dChunk *B3dStack, s16 &B3dStackSize, SB3dNode *InNode);

		bool ReadChunkKEYS(io::IReadFile* file, B3dChunk *B3dStack, s16 &B3dStackSize, SB3dNode *InNode);

		bool ReadChunkANIM(io::IReadFile* file, B3dChunk *B3dStack, s16 &B3dStackSize, SB3dNode *InNode);

		void animate(s32 frame,s32 startFrameLoop, s32 endFrameLoop, core::array<s32> &PrevKeys);


		void resetSkin();

		void CalculateGlobalMatrixes(SB3dNode *Node,SB3dNode *ParentNode);

		void animateSkin(f32 frame,f32 startFrame, f32 endFrame,SB3dNode *InNode,SB3dNode *ParentNode, NGin::Math::AABB &boundingBox);
		void animateBox(s32 frame,s32 startFrameLoop, s32 endFrameLoop, core::array<s32> &PrevKeys, NGin::Math::AABB &boundingBox);

		void getNodeAnimation(f32 frame,SB3dNode *Node,core::vector3df &position, core::vector3df &scale, core::quaternion &rotation, int* LastKey);

		void animateNodes(f32 frame,f32 startFrame, f32 endFrame, core::array<s32> &PrevKeys);


		void slerp(core::quaternion A,core::quaternion B,core::quaternion &C,f32 t);

		void GetNodes(SB3dNode* N, core::array<SB3dNode*> *b);

		f32 totalTime;
		bool HasAnimation;
		s32 lastCalculatedFrame;

		bool AnimateNormals;


		//0- Constant
		//1- Linear
		s32 InterpolationMode;



		//0-None
		//1-Update nodes only
		//2-Update skin only
		//3-Update both nodes and skin
		s32 AnimateMode;



		bool NormalsInFile;


		core::stringc readString(io::IReadFile* file);
		core::stringc stripPathString(core::stringc oldstring, bool keepPath);

		void readFloats(io::IReadFile* file, f32* vec, u32 count);



		core::array<SB3dMaterial> Materials;


		core::array<SB3dTexture> Textures;

		core::array<video::S3DVertex2TCoords*> Vertices;
		//core::array<video::S3DVertex*> Vertices;


		core::array<core::matrix4> Vertices_GlobalMatrix;
		core::array<bool> Vertices_Moved;

		core::array<f32> Vertices_Alpha;

		core::array<s32> AnimatedVertices_VertexID;

		core::array<SB3DMeshBuffer*> AnimatedVertices_MeshBuffer;

		core::array<IMeshBuffer*> Buffers;

		video::IVideoDriver* Driver;






		//This stuff is WIP...


	virtual void CreateAnimationSkelton_Helper(ISceneManager* SceneManager ,core::array<ISceneNode*> &JointChildSceneNodes, CAnimatedMeshSceneNode *AnimatedMeshSceneNode, ISceneNode* ParentNode, SB3dNode *ParentB3dNode, SB3dNode *B3dNode);

	public:

		virtual void StoreAnimationSkelton(core::array<ISceneNode*> &JointChildSceneNodes);
		virtual void RecoverAnimationSkelton(core::array<ISceneNode*> &JointChildSceneNodes);

		virtual void StoreAnimationSkelton(core::array<core::matrix4> &Matrixs);
		virtual void RecoverAnimationSkelton(core::array<core::matrix4> &Matrixs);

		virtual void CreateAnimationSkelton(ISceneManager* SceneManager, core::array<ISceneNode*> &JointChildSceneNodes, CAnimatedMeshSceneNode *AnimatedMeshSceneNode);





	};

} // end namespace scene
} // end namespace irr

#endif

