// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

class ShaderLoader;

#include "CAnimatedMeshSceneNode.h"
#include "IVideoDriver.h"
#include "ISceneManager.h"
#include "S3DVertex.h"
#include "os.h"
#include "SViewFrustrum.h"
#include "ICameraSceneNode.h"
#include "IAnimatedMeshX.h"
#include "IDummyTransformationSceneNode.h"
#include "IMaterialRenderer.h"
#include "IMeshCache.h"
#include "IAnimatedMesh.h"
#include "IAnimatedMeshB3d.h"
#include "CD3D9Driver.h"

namespace irr
{
namespace scene
{



//! constructor
CAnimatedMeshSceneNode::CAnimatedMeshSceneNode(IAnimatedMesh* mesh, ISceneNode* parent, ISceneManager* mgr, s32 id,
			const core::vector3df& position, const core::vector3df& rotation,	const core::vector3df& scale)
: IAnimatedMeshSceneNode(parent, mgr, id, position, rotation, scale), Mesh(0), 
	BeginFrameTime(0), StartFrame(0), EndFrame(0), FramesPerSecond(100),
	Looping(true), LoopCallBack(0), ReadOnlyMaterials(false), InheritAnimation(false)
{
	#ifdef _DEBUG
	setDebugName("CAnimatedMeshSceneNode");
	#endif

	FXFile = 0;
	BeginFrameTime = os::Timer::getTime();

	Mesh = pMeshLOD_HIGH = pMeshLOD_MEDIUM = pMeshLOD_LOW = NULL;
	DistLOD_LOW = DistLOD_MEDIUM = DistLOD_HIGH = 9e10;

	meshSet_ = false;
	unsetMesh_ = NULL;
	setMesh(mesh);
}

#define DropMesh(x) {	if (x && x->ReferenceCounter == 1)				{					\
						irr::video::IVideoDriver* driver = SceneManager->getVideoDriver();	\
						IMesh* tmpMesh = x->getMesh(0);										\
						for(int f = 0; f < tmpMesh->getMeshBufferCount(); ++f)				\
							driver->deleteHardwareBuffer(tmpMesh->getMeshBuffer(f));		\
						SceneManager->getMeshCache()->removeMesh(x);	}					\
					}	\
					if(x) x->drop();

//! destructor
CAnimatedMeshSceneNode::~CAnimatedMeshSceneNode()
{

	//char OO[1024];
	//sprintf(OO, "~CAnimatedMeshSceneNode(%s):%x\nMesh Ref: %i\n", this->Name.c_str(), this, Mesh->ReferenceCounter);
	//OutputDebugString(OO);

	//IMesh* m = Mesh->getMesh(0, 0);
	//for (s32 i=0; i< m->getMeshBufferCount(); ++i)
	//{
	//	IMeshBuffer* mb = m->getMeshBuffer(i);
	//	if (mb)
	//		for(int i = 0; i < MATERIAL_MAX_TEXTURES; ++i)
	//			if(mb->getMaterial(f).Textures[i] != 0)
	//				mb->getMaterial(f).Textures[i]->drop();
	//}

	for(s32 f = 0; f < Materials.size(); ++f)
	{
		video::SMaterial M = Materials[f];

		for(s32 i = 0; i < video::MATERIAL_MAX_TEXTURES; ++i)
		{
			if(M.Textures[i] != 0)
			{
				//sprintf(OO, "    TexDrop: %s, %i\n", M.Textures[i]->getName().c_str(), M.Textures[i]->ReferenceCounter);
				//OutputDebugString(OO);
				
				SceneManager->getVideoDriver()->setTexture(i, 0);
				if(M.Textures[i]->ReferenceCounter <= 2)
					SceneManager->getVideoDriver()->removeTexture(M.Textures[i]);
				M.Textures[i]->drop();
				
				
			}
		}
	}

	DropMesh(pMeshLOD_HIGH);
	DropMesh(pMeshLOD_MEDIUM);
	DropMesh(pMeshLOD_LOW);

	//for (s32 i=0; i<(s32)JointChildSceneNodes.size(); ++i)
	//	if (JointChildSceneNodes[i])
	//		JointChildSceneNodes[i]->drop();

	if (LoopCallBack)
		LoopCallBack->drop();


}



//! sets the current frame. from now on the animation is played from this frame.
void CAnimatedMeshSceneNode::setCurrentFrame(s32 frame)
{
}

void CAnimatedMeshSceneNode::setInheritAnimation(bool inherit)
{
	InheritAnimation = inherit;
}

bool CAnimatedMeshSceneNode::getInheritAnimation()
{
	return InheritAnimation;
}

//! frame
void CAnimatedMeshSceneNode::OnPreRender()
{
	if( !meshSet_ && unsetMesh_ != NULL )
	{
		if( unsetMesh_->getMesh(0) != NULL && unsetMesh_->getMesh(0)->getIsLoaded() )
		{
			setMesh( unsetMesh_ );
			unsetMesh_ = NULL;
		}
	}

	//stringc N = stringc(this->getDebugName());
		//N.make_lower();
		//printf("Render: %x - %s\n", this, IsVisible ? "true" : "false");		
		//if(N == stringc("data\\meshes\\ground.eb3d"))
		//printf("PreRender: %x: %s, %s\n", this, this->getName(), N.c_str());

	if ( IsVisible)
	{
		// Change LOD Mesh
		float distance = RelativeTranslation.getDistanceFrom( SceneManager->getActiveCamera()->getTarget() );

 		if ( distance > DistLOD_LOW * DistLOD_LOW )
		{
			lastLOD = -1;
			Mesh = NULL;
		}
 		else if ( distance > DistLOD_MEDIUM * DistLOD_MEDIUM )
		{
			lastLOD = 2;
			Mesh = pMeshLOD_LOW;
		}
 		else if ( distance > DistLOD_HIGH * DistLOD_HIGH )
		{
			lastLOD = 1;
			Mesh = pMeshLOD_MEDIUM;
		}
 		else 
		{
			lastLOD = 0;
			Mesh = pMeshLOD_HIGH;
		}

		if ( !Mesh ) return;

		if(RenderTimes.size() != Mesh->getMesh(0)->getMeshBufferCount())
		{
			RenderTimes.set_used(Mesh->getMesh(0)->getMeshBufferCount());
			for(int i = 0; i < Mesh->getMesh(0)->getMeshBufferCount(); ++i)
				RenderTimes[i] = 3;
		}

		// because this node supports rendering of mixed mode meshes consisting of 
		// transparent and solid material at the same time, we need to go through all 
		// materials, check of what type they are and register this node for the right
		// render pass according to that.

		s32 frameNr = getFrameNr();



		// update absolute position
		updateAbsolutePosition();

		if(!JointChildSceneNodes.empty() && Mesh && Mesh->getMeshType() == scene::EAMT_B3D)
		{
			//this->getLocalMesh()->getMesh(this->getFrameNr());
			scene::IAnimatedMeshB3d* amm = (IAnimatedMeshB3d*)Mesh;
			core::matrix4* mat;// = new core::matrix4();

			core::array<IAnimatedMeshB3d::SB3dNode*>* Nodes = ((IAnimatedMeshB3d*)Mesh)->GetAnimationNodes(this->getFrameNr(), PrevKeys);
			MatNodes.clear();

			if(!InheritAnimation)
			{
				for(int i = 0; i < Nodes->size(); ++i)
				{
					MatNodes.push_back(((*Nodes)[i]->GlobalAnimatedMatrix * (*Nodes)[i]->GlobalInversedMatrix).getTransposed());
				}

			}else // Is inherited animation?
			{
				if(Parent != NULL && this->Parent->getType() == ESNT_ANIMATED_MESH
					&& ((CAnimatedMeshSceneNode*)Parent)->getLocalMesh()->getMeshType() == EAMT_B3D)
				{
					CAnimatedMeshB3d* ParentMesh = (CAnimatedMeshB3d*)((CAnimatedMeshSceneNode*)Parent)->getLocalMesh();

					for(int i = 0; i < Nodes->size(); ++i)
					{
						IAnimatedMeshB3d::SB3dNode* Node = (*Nodes)[i];
						bool Found = false;

						for(int t = 0; t < ParentMesh->getJointCount(); ++t)
						{
							if(Node->Name.equals_ignore_case(irr::core::stringc(ParentMesh->getJointName(t))))
							{
								MatNodes.push_back(((CAnimatedMeshSceneNode*)Parent)->MatNodes[t]);
								Found = true;
								break;
							}
						}

						if(!Found)
						{
							MatNodes.push_back(((*Nodes)[i]->GlobalAnimatedMatrix * (*Nodes)[i]->GlobalInversedMatrix).getTransposed());
						}
					}
				}

			}

			for(s32 i=0; i<(s32)JointChildSceneNodes.size(); ++i)
				if (JointChildSceneNodes[i])
				{
					mat = amm->getMatrixOfJoint(i, frameNr);
					//mat = amm->getLocalMatrixOfJoint(i);
					//*mat = (Nodes[0][i]->GlobalAnimatedMatrix * Nodes[0][i]->GlobalInversedMatrix).getTransposed();
					//*mat = Nodes[0][i]->GlobalMatrix;

					//char out[128];
					//sprintf(out,"%i %i %f, %f, %f",i,frameNr,mat->getTranslation().X,mat->getTranslation().Y,mat->getTranslation().Z);
					//MessageBox(NULL,out,"",MB_OK);


					//if(mat)
					//	JointChildSceneNodes[i]->getRelativeTransformationMatrix() = *mat;
					//JointChildSceneNodes[i]->updateAbsolutePosition();

					if(mat)
					{
						//JointChildSceneNodes[i]->getRelativeTransformation() = *mat;
						//JointChildSceneNodes[i]->updateAbsolutePosition();

						//JointChildSceneNodes[i]->getAbsoluteTransformation() = *mat;

						//JointChildSceneNodes[i]->setScale(vector3df(1, 1, 1));
						
						//core::matrix4 p = JointChildSceneNodes[i]->getParent()->getAbsoluteTransformation();
						//p.makeInverse();

						//core::matrix4 t = (*mat) * p;

						//JointChildSceneNodes[i]->setRotation(Nodes[0][i]->Animatedrotation.toEuler());
						//JointChildSceneNodes[i]->setPosition(Nodes[0][i]->Animatedposition);
						//JointChildSceneNodes[i]->setScale(Nodes[0][i]->Animatedscale);

						//core::matrix4 m;
						//m.setTranslation(Nodes[0][i]->position);

						//vector3df r;
						//Nodes[0][i]->rotation.toEuler(r);

						//m.setRotationDegrees(r);
						//m.setScale(Nodes[0][i]->scale);

						//m *= (*mat);
						//*mat = m;

						JointChildSceneNodes[i]->setRotation(mat->getRotationDegrees());
						JointChildSceneNodes[i]->setPosition(mat->getTranslation());
						//JointChildSceneNodes[i]->setScale(mat->getScale());
						JointChildSceneNodes[i]->setScale(vector3df(1, 1, 1));
						
						
						//JointChildSceneNodes[i]->setScale(vector3df(1, 1, 1));
						//JointChildSceneNodes[i]->setRotation(vector3df(0, 0, 0));
						JointChildSceneNodes[i]->updateAbsolutePosition();
						
						//char OO[1024];
						//vector3df V = JointChildSceneNodes[i]->getAbsolutePosition();
						////vector3df V = JointChildSceneNodes[i]->getPosition();
						//sprintf(OO, "D: %i: %x: %f, %f, %f\n", i, JointChildSceneNodes[i], V.X, V.Y, V.Z);
						//OutputDebugString(OO);

						//JointChildSceneNodes[i]->setPosition(mat->getTranslation() + vector3df(0, 100, 0));
						//JointChildSceneNodes[i]->setScale(mat->getScale());
						//JointChildSceneNodes[i]->setRotation(mat->getRotationDegrees());
						//JointChildSceneNodes[i]->updateAbsolutePosition();
					}
				}else{
					JointChildSceneNodes[i] =
						SceneManager->addDummyTransformationSceneNode(this);
					JointChildSceneNodes[i]->grab();

					mat = amm->getMatrixOfJoint(i, frameNr);

					//if(mat)
					//	JointChildSceneNodes[i]->getRelativeTransformationMatrix() = *mat;

// 					char OO[1024];
// 					sprintf(OO, "%i: %x\n", i, JointChildSceneNodes);
// 					OutputDebugString(OO);
		
				}
		}


		video::IVideoDriver* driver = SceneManager->getVideoDriver();

		PassCount = 0;
		int transparentCount = 0;
		int solidCount = 0;

		// count transparent and solid materials in this scene node
		for (u32 i=0; i<Materials.size(); ++i)
		{
			video::IMaterialRenderer* rnd = 
				driver->getMaterialRenderer(Materials[i].MaterialType);

			if (rnd && rnd->isTransparent())
				++transparentCount;
			else
				++solidCount;

			if (solidCount && transparentCount)
				break;
		}	

		// register according to material types counted
		if(Order == 0)
		{

//irr::core::stringc N = this->getTag();// != 0 ? this->TransparentNodes[i].node->getTag() : "";
//irr::core::stringc Emi = "Weapon";
//irr::core::stringc E = N.subString(0, 6);
//if(N.size() > 5 && E == Emi)
//{
//	printf("%x: %s(%i, %i)\n", this,this->getTag(), Materials.size(), transparentCount);
//	D3DPERF_SetMarker(D3DCOLOR_ARGB(255, 0, 255, 0), irr::core::stringw(this->getTag()).c_str());
//}
			//if(N == M)
			//{
			//	char O[1024];
			//	sprintf(O, "V: %i; S: %i; T: %i; A: %f; F: %i\n", IsVisible ? 1 : 0, solidCount, transparentCount, Alpha, ForceAlpha ? 1 : 0);
			//	OutputDebugString(O);
			//}
			//if(strcmp(this->getTag(), "WATCHME") == 0)
			//	OutputDebugString("You're not wrong\n");

			if(this->ParticleAlpha)
			{
				//if(strcmp(this->getTag(), "WATCHME") == 0)
				//{
				//	char OO[1024];
				//	sprintf(OO, "%x: particleAlpha()\n", this);
				//	OutputDebugString(OO);
				//}
				SceneManager->registerNodeForRendering(this, scene::ESNRP_TRANSPARENT);
			}else
			{

				bool founds = false;
				if (solidCount >0 && transparentCount == 0 && Alpha > 0.999f)
				{
					//if(strcmp(this->getTag(), "WATCHME") == 0)
					//{
					//	char OO[1024];
					//	sprintf(OO, "%x: Solid()\n", this);
					//	OutputDebugString(OO);
					//}
					SceneManager->registerNodeForRendering(this, scene::ESNRP_SOLID);
					founds = true;
				}
			
				if (transparentCount || Alpha < 0.999f || ForceAlpha == true)
				{
					//if(strcmp(this->getTag(), "WATCHME") == 0)
					//{
					//	char OO[1024];
					//	sprintf(OO, "%x: Trans()\n", this);
					//	OutputDebugString(OO);
					//}
					SceneManager->registerNodeForRendering(this, scene::ESNRP_TRANSPARENT);
					founds = true;
				}

			}
			
			// This is left in for debugging purposes. It was removed becuase "QuestionEmitters" doesn't have a mesh.
			/*if(!founds)
			{
				char out[128];
				sprintf(out,"Solid Count: %i\nTransparent Count: %i\n Alpha: %i\n ForceAlpha: %i",solidCount,transparentCount,Alpha,ForceAlpha);
				MessageBox(NULL,out,"Couldn't find a home in the renderer",MB_OK);
			}*/
			
		}
		//ISceneNode::OnPreRender();

		for (s32 j=0; j<(s32)JointChildSceneNodes.size(); ++j)
			if (JointChildSceneNodes[j])
				JointChildSceneNodes[j]->OnPreRender();
	}
	
	ISceneNode::OnPreRender();
}


inline s32 CAnimatedMeshSceneNode::getFrameNr()
{
	s32 frame = 0;

	s32 len = EndFrame - StartFrame;

	if (!len)
		return StartFrame;

	if(this->getLocalMesh()->getMeshType() == EAMT_B3D)
		this->FramesPerSecond = ((CAnimatedMeshB3d*)this->getLocalMesh())->AnimFPS * 100;

	

	if (Looping)
	{
		// play animation looped
		frame = StartFrame + ((s32)((((float)os::Timer::getTime()) - ((float)BeginFrameTime))
			* (FramesPerSecond/1000.0f)) % len);
	}
	else
	{
		// play animation non looped
		//frame = StartFrame + ((s32)((os::Timer::getTime() - BeginFrameTime)
			//* (FramesPerSecond/1000.0f)));

		frame = StartFrame + ((s32)((((float)os::Timer::getTime()) - ((float)BeginFrameTime))
			* (FramesPerSecond/1000.0f)));

		//Aif(frame < lastframe)
			//Aprintf("Frame Out(%i, %i)\n", frame, lastframe);
		//Aprintf("%i\n", frame);
		lastframe = frame;

		if (frame > EndFrame)
		{
			//Aprintf("Anim Done\n");
			frame = EndFrame;
			StartFrame = EndFrame;
			if (LoopCallBack)
				LoopCallBack->OnAnimationEnd(this);
		}
	}

	return frame;
}


void CAnimatedMeshSceneNode::setAnimationCPY()
{
	OFPS = FramesPerSecond;
}

//! OnPostRender() is called just after rendering the whole scene.
void CAnimatedMeshSceneNode::OnPostRender(u32 timeMs)
{
	if(pMeshLOD_HIGH != NULL)
		Mesh = pMeshLOD_HIGH;

	if (IsVisible)
	{
		core::list<ISceneNode*>::Iterator it = Children.begin();
		for (; it != Children.end(); ++it)
			(*it)->OnPostRender(timeMs);
	}
}

//! renders the node.
void CAnimatedMeshSceneNode::renderBuffer(int Index)
{
	video::IVideoDriver* driver = SceneManager->getVideoDriver();

	if (!Mesh || !driver)
		return;

	bool isTransparentPass = 
		SceneManager->getSceneNodeRenderPass() == scene::ESNRP_TRANSPARENT;

	++PassCount;

	s32 frame = getFrameNr();

	scene::IMesh* m = Mesh->getMesh(frame, 255, StartFrame, EndFrame);

	if (m)
	{

		if(Index < 0 || Index > m->getMeshBufferCount() - 1)
			return;

		Box = m->getBoundingBox();

		// for debug purposes only:
		if (DebugDataVisible && PassCount==1)
		{
			video::SMaterial mat;
			mat.Lighting = false;
			driver->setMaterial(mat);
			driver->draw3DBox(Box, video::SColor(0,255,255,255));
			
			if (Mesh->getMeshType() == EAMT_X)
			{
				// draw skeleton
				const core::array<core::vector3df>* ds = 
					((IAnimatedMeshX*)Mesh)->getDrawableSkeleton(frame);

				for (s32 s=0; s<(s32)ds->size(); s+=2)
					driver->draw3DLine((*ds)[s], (*ds)[s+1],  video::SColor(0,255,255,255));
			}

			#if 0
			// draw normals
			for (s32 g=0; g<m->getMeshBufferCount(); ++g)
			{
				scene::IMeshBuffer* mb = m->getMeshBuffer(g);

				u32 vSize;
				u32 i;
				vSize = 0;
				switch( mb->getVertexType() )
				{
					case video::EVT_STANDARD:
						vSize = sizeof ( video::S3DVertex );
						break;
					case video::EVT_2TCOORDS:
						vSize = sizeof ( video::S3DVertex2TCoords );
						break;
					case video::EVT_TANGENTS:
						vSize = sizeof ( video::S3DVertexTangents );
						break;
				}

				const video::S3DVertex* v = ( const video::S3DVertex*)mb->getVertices();
				video::SColor c ( 255, 128 ,0, 0 );
				video::SColor c1 ( 255, 255 ,0, 0 );
				for ( i = 0; i != mb->getVertexCount(); ++i )
				{
					core::vector3df h = v->Normal * 5.f;
					core::vector3df h1 = h.crossProduct ( core::vector3df ( 0.f, 1.f, 0.f ) );

					driver->draw3DLine ( v->Pos, v->Pos + h, c );
					driver->draw3DLine ( v->Pos + h, v->Pos + h + h1, c );
					v = (const video::S3DVertex*) ( (u8*) v + vSize );
				}
			}
			#endif

		}

		//if (Shadow && PassCount==1)
		//	Shadow->setMeshToRenderFrom(m);


		bool DrawCustom = false;

		if(this->getLocalMesh()->getMeshType() == scene::EAMT_B3D)
		{
			if(((IAnimatedMeshB3d*)Mesh)->IsAnimated())
				DrawCustom = true;
		}

		irr::core::stringw Str;
		Str.append(this->getTag());
		if(Str != stringw(""));
			D3DPERF_SetMarker(D3DCOLOR_ARGB(255, 0, 255, 0), Str.c_str());
		



		scene::IMeshBuffer* mb = m->getMeshBuffer(Index);
		if(RenderTimes[Index] == (int)SceneManager->getSceneNodeRenderPass() || Order != 0)
		{
			driver->setMaterial(Materials[Index]);
			driver->drawMeshBuffer(mb, DrawCustom);
		}
			
	}

}

//extern char St[4096];
//extern s32 FStart;

//! renders the node.
void CAnimatedMeshSceneNode::render()
{

	//this->setName(St);
	//FStart = atoi(this->getTag());

	video::IVideoDriver* driver = SceneManager->getVideoDriver();

	if (!Mesh || !driver)
		return;

	bool isTransparentPass = 
		SceneManager->getSceneNodeRenderPass() == scene::ESNRP_TRANSPARENT;

	++PassCount;

	//((irr::video::CD3D9Driver*)driver)->setTransformN(video::ETS_WORLD, AbsoluteTransformation);

	s32 frame = getFrameNr();

	scene::IMesh* m = Mesh->getMesh(frame, 255, StartFrame, EndFrame);

	if (m && m->getIsLoaded())
	{
#if 0
		Box = m->getBoundingBox();

		// for debug purposes only:
		if (DebugDataVisible && PassCount==1)
		{
			video::SMaterial mat;
			mat.Lighting = false;
			driver->setMaterial(mat);
			driver->draw3DBox(Box, video::SColor(0,255,255,255));
			
			if (Mesh->getMeshType() == EAMT_X)
			{
				// draw skeleton
				const core::array<core::vector3df>* ds = 
					((IAnimatedMeshX*)Mesh)->getDrawableSkeleton(frame);

				for (s32 s=0; s<(s32)ds->size(); s+=2)
					driver->draw3DLine((*ds)[s], (*ds)[s+1],  video::SColor(0,255,255,255));
			}

			#if 0
			// draw normals
			for (s32 g=0; g<m->getMeshBufferCount(); ++g)
			{
				scene::IMeshBuffer* mb = m->getMeshBuffer(g);

				u32 vSize;
				u32 i;
				vSize = 0;
				switch( mb->getVertexType() )
				{
					case video::EVT_STANDARD:
						vSize = sizeof ( video::S3DVertex );
						break;
					case video::EVT_2TCOORDS:
						vSize = sizeof ( video::S3DVertex2TCoords );
						break;
					case video::EVT_TANGENTS:
						vSize = sizeof ( video::S3DVertexTangents );
						break;
				}

				const video::S3DVertex* v = ( const video::S3DVertex*)mb->getVertices();
				video::SColor c ( 255, 128 ,0, 0 );
				video::SColor c1 ( 255, 255 ,0, 0 );
				for ( i = 0; i != mb->getVertexCount(); ++i )
				{
					core::vector3df h = v->Normal * 5.f;
					core::vector3df h1 = h.crossProduct ( core::vector3df ( 0.f, 1.f, 0.f ) );

					driver->draw3DLine ( v->Pos, v->Pos + h, c );
					driver->draw3DLine ( v->Pos + h, v->Pos + h + h1, c );
					v = (const video::S3DVertex*) ( (u8*) v + vSize );
				}
			}
			#endif

		}
#endif

		bool DrawCustom = false;

		if(this->getLocalMesh()->getMeshType() == scene::EAMT_B3D)
		{
			if(((IAnimatedMeshB3d*)Mesh)->IsAnimated())
				DrawCustom = true;
		}

// 		char OOO[128];
// 		sprintf(OOO, "%i", m->getMeshBufferCount());
//
// 		irr::core::stringw Str;
// 		Str.append(this->getTag());
// 		Str.append(" (Sc: ");
// 		Str.append(OOO);
// 		Str.append(")");
// 		if(Str != stringw(""));
// 			D3DPERF_SetMarker(D3DCOLOR_ARGB(255, 0, 255, 0), Str.c_str());
		

		//for (s32 i=0; i<m->getMeshBufferCount(); ++i)
		for (s32 i=m->getMeshBufferCount()-1; i>-1; --i)
		{

			//video::IMaterialRenderer* rnd = driver->getMaterialRenderer(Materials[i].MaterialType);
			//bool transparent = (rnd && rnd->isTransparent());

			// only render transparent buffer if this is the transparent render pass
			// and solid only in solid pass
			//if (transparent == isTransparentPass) 
			//{

				scene::IMeshBuffer* mb = m->getMeshBuffer(i);

				if(mb == 0)
					continue;

				if(RenderTimes[i] == (int)SceneManager->getSceneNodeRenderPass() || Order != 0)
				{
					if( Materials.size() > i )
						driver->setMaterial(Materials[i]);

					driver->drawMeshBuffer(mb, DrawCustom);
				}else
				{
					char OO[1024];
					sprintf(OO, "Rejected Mesh: %s (RenderTime: %i != %i; Order: %i);\n", this->Tag.c_str(), RenderTimes[i], (int)SceneManager->getSceneNodeRenderPass(), Order);
					OutputDebugString(OO);
				}

		}			
	}
}



//! sets the frames between the animation is looped.
//! the default is 0 - MaximalFrameCount of the mesh.
bool CAnimatedMeshSceneNode::setFrameLoop(s32 begin, s32 end)
{
	//if(end == 5500)
	//{
	//	begin = 28300;
	//	end = 32400;
	//}

	//printf("AnimCall(%i, %i)\n", begin, end);
	if (!Mesh)
		return false;

	s32 frameCount = Mesh->getFrameCount();

	if (!(begin <= end && begin < frameCount && end < frameCount))
		return false;

	StartFrame = begin;
	EndFrame = end;
	BeginFrameTime = os::Timer::getTime();

	return true;
}



//! sets the speed with witch the animation is played
void CAnimatedMeshSceneNode::setAnimationSpeed(s32 framesPerSecond)
{
	FramesPerSecond = framesPerSecond;
}




//! returns the axis aligned bounding box of this node
const core::aabbox3d<f32>& CAnimatedMeshSceneNode::getBoundingBox() const
{
	return Box;//Mesh ? Mesh->getBoundingBox() : Box;
}


//! returns the material based on the zero based index i. To get the amount
//! of materials used by this scene node, use getMaterialCount().
//! This function is needed for inserting the node into the scene hirachy on a
//! optimal position for minimizing renderstate changes, but can also be used
//! to directly modify the material of a scene node.
video::SMaterial& CAnimatedMeshSceneNode::getMaterial(s32 i)
{
	if (i < 0 || i >= (s32)Materials.size())
		return ISceneNode::getMaterial(i);

	return Materials[i];
}



//! returns amount of materials used by this scene node.
s32 CAnimatedMeshSceneNode::getMaterialCount()
{
	return Materials.size();
}

//! Returns a pointer to a child node, wich has the same transformation as 
//! the corrsesponding joint, if the mesh in this scene node is a ms3d mesh.
ISceneNode* CAnimatedMeshSceneNode::getXJointNode(const c8* jointName)
{
	if (!Mesh || Mesh->getMeshType() != EAMT_X)
		return 0;

	IAnimatedMeshX* amm = (IAnimatedMeshX*)Mesh;
	s32 jointCount = amm->getJointCount();
	s32 number = amm->getJointNumber(jointName);

	if (number == -1)
	{
		os::Printer::log("Joint with specified name not found in x mesh.", jointName, ELL_WARNING);
		return 0;
	}

	if (JointChildSceneNodes.empty())
	{
		// allocate joints for the first time.
		JointChildSceneNodes.set_used(jointCount);
		for (s32 i=0; i<jointCount; ++i)
			JointChildSceneNodes[i] = 0;
	}

	if (JointChildSceneNodes[number] == 0)
	{
		JointChildSceneNodes[number] = 
			SceneManager->addDummyTransformationSceneNode(this);
		JointChildSceneNodes[number]->grab();
	}

	return JointChildSceneNodes[number];
}

//! Returns a pointer to a child node, which has the same transformation as
//! the corrsesponding joint, if the mesh in this scene node is a b3d mesh.
ISceneNode* CAnimatedMeshSceneNode::getB3DJointNode(const c8* jointName)
{
	if (!Mesh || Mesh->getMeshType() != EAMT_B3D)
		return 0;

	IAnimatedMeshB3d* amm = (IAnimatedMeshB3d*)Mesh;
	s32 number = amm->getJointNumber(jointName);
	

	//return amm->getJoint(number);
	return getB3DJointNode(number);
	
}

ISceneNode* CAnimatedMeshSceneNode::getB3DJointNode(s32 number)
{

	IAnimatedMeshB3d* amm = (IAnimatedMeshB3d*)Mesh;
	s32 jointCount = amm->getJointCount();

	if (number == -1)
	{
		return 0;
	}

	if (JointChildSceneNodes.empty())
	{
		// allocate joints for the first time.
		JointChildSceneNodes.set_used(jointCount);
		for (s32 i=0; i<jointCount; ++i)
			JointChildSceneNodes[i] = 0;
	}

	if (JointChildSceneNodes[number] == 0)
	{
		JointChildSceneNodes[number] =
			SceneManager->addDummyTransformationSceneNode(this);
		JointChildSceneNodes[number]->grab();
		JointChildSceneNodes[number]->setName(amm->getJointName(number));
		JointChildSceneNodes[number]->IsJoint = true;
	}

	return JointChildSceneNodes[number];
}

s32 CAnimatedMeshSceneNode::getB3DJointCount()
{
	IAnimatedMeshB3d* amm = (IAnimatedMeshB3d*)Mesh;
	return amm->getJointCount();
}


//! Removes a child from this scene node.
//! Implemented here, to be able to remove the shadow properly, if there is one,
//! or to remove attached childs.
bool CAnimatedMeshSceneNode::removeChild(ISceneNode* child)
{
	if (ISceneNode::removeChild(child))
	{
		for (s32 i=0; i<(s32)JointChildSceneNodes.size(); ++i)
		if (JointChildSceneNodes[i] == child)
		{
			JointChildSceneNodes[i]->drop();
			JointChildSceneNodes[i] = 0;
			return true;
		}

		return true;
	}

	return false;
}

//! Sets looping mode which is on by default. If set to false,
//! animations will not be looped.
void CAnimatedMeshSceneNode::setLoopMode(bool playAnimationLooped)
{
	Looping = playAnimationLooped;
}


//! Sets a callback interface which will be called if an animation
//! playback has ended. Set this to 0 to disable the callback again.
void CAnimatedMeshSceneNode::setAnimationEndCallback(IAnimationEndCallBack* callback)
{
	if (LoopCallBack)
		LoopCallBack->drop();

	LoopCallBack = callback;

	if (LoopCallBack)
		LoopCallBack->grab();
}


//! Sets if the scene node should not copy the materials of the mesh but use them in a read only style.
void CAnimatedMeshSceneNode::setReadOnlyMaterials(bool readonly)
{
	ReadOnlyMaterials = readonly;
}


//! Returns if the scene node should not copy the materials of the mesh but use them in a read only style
bool CAnimatedMeshSceneNode::isReadOnlyMaterials()
{
	return ReadOnlyMaterials;
}


//! Writes attributes of the scene node.
void CAnimatedMeshSceneNode::serializeAttributes(io::IAttributes* out, io::SAttributeReadWriteOptions* options)
{
	IAnimatedMeshSceneNode::serializeAttributes(out, options);

	out->addString("Mesh", SceneManager->getMeshCache()->getMeshFilename(Mesh));
	out->addBool("Looping", Looping);
	out->addBool("ReadOnlyMaterials", ReadOnlyMaterials);
	out->addFloat("FramesPerSecond", (f32)FramesPerSecond);

	// TODO: write animation names instead of frame begin and ends
}


//! Reads attributes of the scene node.
void CAnimatedMeshSceneNode::deserializeAttributes(io::IAttributes* in, io::SAttributeReadWriteOptions* options)
{
	IAnimatedMeshSceneNode::deserializeAttributes(in, options);

	core::stringc oldMeshStr = SceneManager->getMeshCache()->getMeshFilename(Mesh);
	core::stringc newMeshStr = in->getAttributeAsString("Mesh");

	Looping = in->getAttributeAsBool("Looping");
	ReadOnlyMaterials = in->getAttributeAsBool("ReadOnlyMaterials");
	FramesPerSecond = (s32)in->getAttributeAsFloat("FramesPerSecond");

	if (newMeshStr != "" && oldMeshStr != newMeshStr)
	{
		IAnimatedMesh* newAnimatedMesh = SceneManager->getMesh(newMeshStr.c_str(), false, NULL, NULL);

		if (newAnimatedMesh)
			setMesh(newAnimatedMesh);
	}

	// TODO: read animation names instead of frame begin and ends
}


//JA Gets Mesh
IAnimatedMesh* CAnimatedMeshSceneNode::getLocalMesh()
{
	return Mesh;
}

// JA sets shader
void CAnimatedMeshSceneNode::setRCFXShader(ShaderLoader* shd)
{
	// Deprecated
}

//! Sets a new mesh
void CAnimatedMeshSceneNode::setMesh(IAnimatedMesh* mesh)
{
	if (!mesh)
		return; // won't set null mesh

	if (Mesh)
		Mesh->drop();

	// New mesh isn't loaded yet, keep it as unset
	if( mesh->getMesh(0) != NULL && !mesh->getMesh(0)->getIsLoaded() )
	{
		unsetMesh_ = mesh;
		return;
	}

	meshSet_ = true;
	Mesh = mesh;

	// get materials and bounding box
	Box = Mesh->getBoundingBox();

	IMesh* m = Mesh->getMesh(0,0);
	if (m)
	{
		Materials.clear();

		video::SMaterial mat;
		for (s32 f=0; f<m->getMeshBufferCount(); ++f)
		{
			IMeshBuffer* mb = m->getMeshBuffer(f);
			if (mb)
				mat = mb->getMaterial();

			for(int i = 0; i < MATERIAL_MAX_TEXTURES; ++i)
				if(mat.Textures[i] != 0)
					mat.Textures[i]->grab();

			Materials.push_back(mat);
		}
	}

	// get start and begin time

	StartFrame = 0;
	EndFrame = Mesh->getFrameCount();

	// grab the mesh
	//MC mod here
	if (Mesh) Mesh->grab();
	pMeshLOD_HIGH = Mesh;

	// Create all joint kiddies
	if(mesh->getMeshType() == irr::scene::EAMT_B3D)
		for(int i = 0; i < getB3DJointCount(); ++i)
			getB3DJointNode(i);
}

void CAnimatedMeshSceneNode::setMeshLOD_HIGH( IAnimatedMesh* mesh, float Distance )
{
	DistLOD_HIGH = Distance;
// 	if (!mesh) return; // won't set null mesh
// 	if (pMeshLOD_HIGH) pMeshLOD_HIGH->drop();
// 
// 	pMeshLOD_HIGH = mesh;
// 
// 	// grab the mesh
// 	//MC mod here
// 	if (pMeshLOD_HIGH) pMeshLOD_HIGH->grab();
// 	Mesh = pMeshLOD_HIGH;
}

void CAnimatedMeshSceneNode::setMeshLOD_MEDIUM( IAnimatedMesh* mesh, float Distance )
{
	DistLOD_MEDIUM = Distance;
	if (!mesh) return; // won't set null mesh
	if (pMeshLOD_MEDIUM) pMeshLOD_MEDIUM->drop();

	pMeshLOD_MEDIUM = mesh;

	// grab the mesh
	//MC mod here
	if (pMeshLOD_MEDIUM) pMeshLOD_MEDIUM->grab();
}


void CAnimatedMeshSceneNode::setMeshLOD_LOW( IAnimatedMesh* mesh, float Distance )
{
	DistLOD_LOW = Distance;
	if (!mesh) return; // won't set null mesh
	if (pMeshLOD_LOW) pMeshLOD_LOW->drop();

	pMeshLOD_LOW = mesh;

	// grab the mesh
	//MC mod here
	if (pMeshLOD_LOW) pMeshLOD_LOW->grab();
}

} // end namespace scene
} // end namespace irr

