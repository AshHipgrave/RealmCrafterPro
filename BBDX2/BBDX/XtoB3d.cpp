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

#pragma comment(lib, "D3d9.lib")     //DirectX 9 Lib Files
#pragma comment(lib, "D3dx9.lib")
#include <d3dx9.h>                   //DirectX 9 Header Files

#include "ExportB3D.h"

PDIRECT3D9  _XtoB3D_pD3D(NULL);
LPDIRECT3DDEVICE9 _XtoB3D_pD3DDevice(NULL);
bool _XtoB3D_bManageDX(false);

	void CopyString(const char* input, char** output)
	{
		if( input )
		{
			UINT length = (UINT)::strlen(input) + 1; // add 1 for terminating null charater.
			*output = new char[length];
			::strcpy_s(*output, length+1, input);
		}
		else
		{
			*output = 0;
		}
	}

	
	struct FrameEx : public D3DXFRAME
	{
		D3DXFRAME* pParent;
	};

	class cFrameAllocator  : public ID3DXAllocateHierarchy 
	{
		public:
			HRESULT STDMETHODCALLTYPE CreateFrame( PCSTR Name, D3DXFRAME** ppNewFrame)
			{
				D3DXFRAME* frameEx = new FrameEx();
				((FrameEx*)frameEx)->pParent = NULL;

				if( Name )	CopyString(Name, &frameEx->Name);
				else		CopyString("<no name>", &frameEx->Name);

				frameEx->pMeshContainer = 0;
				frameEx->pFrameSibling = 0;
				frameEx->pFrameFirstChild = 0;
				D3DXMatrixIdentity(&frameEx->TransformationMatrix);
				*ppNewFrame = frameEx;
				return D3D_OK;
			}

			HRESULT STDMETHODCALLTYPE CreateMeshContainer( PCSTR Name, const D3DXMESHDATA* pMeshData, const D3DXMATERIAL* pMaterials, const D3DXEFFECTINSTANCE* pEffectInstances, DWORD NumMaterials, const DWORD *pAdjacency, ID3DXSkinInfo* pSkinInfo, D3DXMESHCONTAINER** ppNewMeshContainer )
			{	
				D3DXMESHCONTAINER* meshContainer = new D3DXMESHCONTAINER;
				::ZeroMemory(meshContainer, sizeof(D3DXMESHCONTAINER));	
				if( Name )	CopyString(Name, &meshContainer->Name);
				else        CopyString("<no name>", &meshContainer->Name);


				/////////////////////////////////////////////////////////////////////
				// Save our created mesh container now because we might return early,
				// and we must _always_ return an allocated container.

				*ppNewMeshContainer = meshContainer;


				//////////////////////////////////////////////////////////////////
				// Copy material data, and allocate memory for texture file names.

				meshContainer->NumMaterials = NumMaterials;
				meshContainer->pMaterials   = new D3DXMATERIAL[NumMaterials];
				for(unsigned long i = 0; i < NumMaterials; ++i)
				{
					D3DXMATERIAL* mtrls = meshContainer->pMaterials;
					mtrls[i].MatD3D = pMaterials[i].MatD3D;
					mtrls[i].MatD3D.Ambient = pMaterials[i].MatD3D.Diffuse;

					CopyString(pMaterials[i].pTextureFilename, 
						&mtrls[i].pTextureFilename);
				}


				////////////////////////////////////////////////////////////
				// Ignore effect instances and adjacency info for this demo.

				meshContainer->pEffects   = 0;
				meshContainer->pAdjacency = 0;


				//////////////////////////
				// Save mesh and skininfo.

				meshContainer->MeshData.Type  = D3DXMESHTYPE_MESH;
				meshContainer->MeshData.pMesh = pMeshData->pMesh; 
				meshContainer->pSkinInfo      = pSkinInfo;
				pMeshData->pMesh->AddRef();
				if (pSkinInfo) pSkinInfo->AddRef();

				return D3D_OK;
			}


			HRESULT STDMETHODCALLTYPE DestroyFrame( D3DXFRAME* pFrameToFree )
			{
				delete pFrameToFree->Name;
				delete pFrameToFree;

				return D3D_OK; 
			}

			HRESULT STDMETHODCALLTYPE DestroyMeshContainer( D3DXMESHCONTAINER* pMeshContainerBase)
			{
				delete pMeshContainerBase->Name;
				delete [] pMeshContainerBase->pAdjacency;
				delete [] pMeshContainerBase->pEffects;

				for(unsigned long i = 0; i < pMeshContainerBase->NumMaterials; ++i)
					delete pMeshContainerBase->pMaterials[i].pTextureFilename;

				delete [] pMeshContainerBase->pMaterials;

				if (pMeshContainerBase->MeshData.pMesh) pMeshContainerBase->MeshData.pMesh->Release();
				if (pMeshContainerBase->pSkinInfo) pMeshContainerBase->pSkinInfo->Release();

				delete pMeshContainerBase;

				return D3D_OK;
			}

	};

	

///////////////////////////////////////////////////////////////////////////
///
///////////////////////////////////////////////////////////////////////////
	D3DXFRAME* FindFrameWithMesh(D3DXFRAME* frame)
	{
		// the input .X file should contain only one mesh.

		if( frame->pMeshContainer )
			if( frame->pMeshContainer->MeshData.pMesh != 0 ) return frame;

		D3DXFRAME* f = 0;
		if(frame->pFrameSibling) {
			((FrameEx*)frame->pFrameSibling)->pParent = ((FrameEx*)frame)->pParent;
			if( f = FindFrameWithMesh(frame->pFrameSibling) )	return f;
		}
		if(frame->pFrameFirstChild) 
		{
			((FrameEx*)frame->pFrameFirstChild)->pParent = frame;
			if( f = FindFrameWithMesh(frame->pFrameFirstChild) )	return f;
		}

		return NULL;
	}

	LPD3DXFRAME FindFirstSkinInfo(D3DXFRAME* frame )
	{
		if( frame->pMeshContainer && frame->pMeshContainer->pSkinInfo) 
			return frame;

		LPD3DXFRAME pFrame(NULL);
		if(frame->pFrameSibling) 
		{
			((FrameEx*)frame->pFrameSibling)->pParent = ((FrameEx*)frame)->pParent;
			pFrame = FindFirstSkinInfo( frame->pFrameSibling );
		}
		if (pFrame) return pFrame;

		if(frame->pFrameFirstChild) 
		{
			((FrameEx*)frame->pFrameFirstChild)->pParent = frame;
			pFrame = FindFirstSkinInfo( frame->pFrameFirstChild );
		}
		if (pFrame) return pFrame;

		return NULL;
	}

///////////////////////////////////////////////////////////////////////////
///
///////////////////////////////////////////////////////////////////////////

void XtoB3D_SetDX( PDIRECT3D9  pD3D, LPDIRECT3DDEVICE9 pD3DDevice )
{
	_XtoB3D_pD3D = pD3D;
	_XtoB3D_pD3DDevice = pD3DDevice;
	_XtoB3D_bManageDX = false;
}

bool XtoB3D_CreateDX()
{
	// Create the D3D object, which is needed to create the D3DDevice.
	if ( NULL == ( _XtoB3D_pD3D = Direct3DCreate9( D3D_SDK_VERSION ) ) ) return false;

	D3DPRESENT_PARAMETERS d3dpp; 
	ZeroMemory( &d3dpp, sizeof(d3dpp) );
	d3dpp.Windowed = true;
	d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;

	// Create the Direct3D device
	if ( FAILED( _XtoB3D_pD3D->CreateDevice(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, GetDesktopWindow(),
		D3DCREATE_SOFTWARE_VERTEXPROCESSING, &d3dpp, &_XtoB3D_pD3DDevice ) ) ) 
		return false;

	_XtoB3D_bManageDX = true;
	return true;
}

void XtoB3D_ShutdownDX()
{
	_XtoB3D_pD3DDevice->Release();
	_XtoB3D_pD3D->Release();
	_XtoB3D_pD3DDevice = NULL;
	_XtoB3D_pD3D = NULL;
}

void CreateMaterials( DWORD nMaterials, LPD3DXMATERIAL aMaterials, cB3DChunk_BB3D* bb3d, vector<cB3DChunk_BRUS*> &apMaterials )
{
	// Copy each material and create its texture
	apMaterials.resize( nMaterials );
	cB3DChunk_TEXS* texs;
	for( DWORD i=0; i<nMaterials; i++ )
	{
		texs = NULL;
		if (aMaterials[i].pTextureFilename!=NULL)
		{
			texs = new cB3DChunk_TEXS;
			bb3d->InsertTexture(texs);
			texs->FileName = aMaterials[i].pTextureFilename;
		}

		cB3DChunk_BRUS* mat1 = new cB3DChunk_BRUS;
		bb3d->InsertMaterial(mat1);
			if (aMaterials[i].pTextureFilename!=NULL) mat1->MaterialName = aMaterials[i].pTextureFilename;
			if (texs) mat1->lpTextures.push_back(texs);
			mat1->shininess = aMaterials[i].MatD3D.Power;
			D3DXCOLOR color = D3DXCOLOR(aMaterials[i].MatD3D.Diffuse);
			mat1->red = color.r;	mat1->green = color.g;	mat1->blue = color.b;	mat1->alpha = color.a;
			apMaterials[i] = mat1;
	}
}

void CreateMesh( ID3DXMesh* pMesh, cB3DChunk_NODE* pNode, vector<cB3DChunk_BRUS*> &apMaterials, D3DXMATRIX* pTransform)
{
	cB3DChunk_MESH* mesh = new cB3DChunk_MESH;
		pNode->pTypeChunk = mesh;

	/////////////////////////////////////
	// V E R T I C E S
	//////////////////////////////////////////////

	char* aMeshVertices;
	int VertexOffset = pMesh->GetNumBytesPerVertex();
	int iPosition(-1), iNormal(-1), iColor(-1);
	vector<int> aiTexcoords;
	cB3DChunk_VRTS* vertices = &mesh->Vertices;
	vertices->aVertices.resize(pMesh->GetNumVertices());
	pMesh->LockVertexBuffer( D3DLOCK_READONLY, (LPVOID*)&aMeshVertices );

	D3DVERTEXELEMENT9 aDeclaration[MAX_FVF_DECL_SIZE];

	pMesh->GetDeclaration( aDeclaration );
	// Find Vertex Position
	for (int i(0); aDeclaration[i].Type != D3DDECLTYPE_UNUSED; ++i )
		if (aDeclaration[i].Usage == D3DDECLUSAGE_POSITION) {iPosition = aDeclaration[i].Offset; break; }
		// Find Vertex Normal
		for (int i(0); aDeclaration[i].Type != D3DDECLTYPE_UNUSED; ++i )
			if (aDeclaration[i].Usage == D3DDECLUSAGE_NORMAL) {iNormal = aDeclaration[i].Offset; break; }
			// Find Vertex Color
			for (int i(0); aDeclaration[i].Type != D3DDECLTYPE_UNUSED; ++i )
				if (aDeclaration[i].Usage == D3DDECLUSAGE_COLOR) {iColor = aDeclaration[i].Offset; break; }
				// Find Texcoords
				for (int i(0); aDeclaration[i].Type != D3DDECLTYPE_UNUSED; ++i )
					if (aDeclaration[i].Usage == D3DDECLUSAGE_TEXCOORD) aiTexcoords.push_back(aDeclaration[i].Offset);

				for (size_t i(0); i<vertices->aVertices.size(); ++i)
				{
					if (iPosition>=0) {
						D3DXVec3TransformCoord( (D3DXVECTOR3*)&vertices->aVertices[i].x, (D3DXVECTOR3*)&aMeshVertices[iPosition], pTransform);
						iPosition += VertexOffset;
					}
					if (iNormal>=0) {
						D3DXVec3TransformNormal( (D3DXVECTOR3*)&vertices->aVertices[i].nx, (D3DXVECTOR3*)&aMeshVertices[iNormal], pTransform);
						iNormal += VertexOffset;
					}
					if (iColor>=0) {
						D3DXCOLOR color = D3DXCOLOR(((D3DCOLOR*)&aMeshVertices[iColor])[0]);
						vertices->aVertices[i].red = color.r;
						vertices->aVertices[i].green = color.g;
						vertices->aVertices[i].blue = color.b;
						vertices->aVertices[i].alpha = color.a;
						iColor += VertexOffset;
					}
					for (size_t j(0); j<aiTexcoords.size(); ++j){
						vertices->aVertices[i].aTexcoordsU.push_back( ((float*)&aMeshVertices[aiTexcoords[j]])[0] );
						vertices->aVertices[i].aTexcoordsV.push_back( ((float*)&aMeshVertices[aiTexcoords[j]])[1] );
						aiTexcoords[j] += VertexOffset;
					}
				}
		pMesh->UnlockVertexBuffer();

				/////////////////////////////////////
				// F A C E S
				//////////////////////////////////////////////
				D3DXATTRIBUTERANGE* aAttribRanges = new D3DXATTRIBUTERANGE[apMaterials.size()+1];
				DWORD nAttribRanges;
				pMesh->GetAttributeTable( aAttribRanges, &nAttribRanges );

				unsigned short* aIndexBuffer;
				pMesh->LockIndexBuffer(D3DLOCK_READONLY, (LPVOID*)&aIndexBuffer);

				cB3DChunk_TRIS* pMeshGroup;
				mesh->aTrianglesGroup.resize(nAttribRanges);
				for (DWORD r(0); r<nAttribRanges; ++r)
				{
					pMeshGroup = &mesh->aTrianglesGroup[r];
					pMeshGroup->pMaterial = apMaterials[aAttribRanges[r].AttribId];
					pMeshGroup->aTriangles.resize(aAttribRanges[r].FaceCount);
					for (DWORD f(0); f<aAttribRanges[r].FaceCount; ++f)
					{
						pMeshGroup->aTriangles[f].aIndices[0] = aIndexBuffer[ aAttribRanges[r].FaceStart*3 + f*3+0 ];
						pMeshGroup->aTriangles[f].aIndices[1] = aIndexBuffer[ aAttribRanges[r].FaceStart*3 + f*3+1 ];
						pMeshGroup->aTriangles[f].aIndices[2] = aIndexBuffer[ aAttribRanges[r].FaceStart*3 + f*3+2 ];
					}
				}
				delete aAttribRanges;
				pMesh->UnlockAttributeBuffer();
}

void SetTranslationKeys( cB3DChunk_NODE* node, ID3DXKeyframedAnimationSet* pKeyFrameAnimationSet, UINT iBone)
{
	D3DXKEY_VECTOR3 key;
	UINT nKeyFrames = pKeyFrameAnimationSet->GetNumTranslationKeys(iBone);
	if (nKeyFrames == 0) return;

	cB3DChunk_KEYS* pKeys = new cB3DChunk_KEYS;
		node->lpKeys.push_back(pKeys);
		pKeys->Flags = 1;

	pKeys->aKeyFrames.resize( nKeyFrames );
	float StartingTime(0);

	for (UINT i(0); i<nKeyFrames; ++i)
	{
		pKeyFrameAnimationSet->GetTranslationKey(iBone, i, &key);
		if (i==0) { 
			StartingTime = key.Time; 
			key.Time = 0; 
		}
		else 
			key.Time -= StartingTime;

		pKeys->aKeyFrames[i].position[0] = key.Value.x;
		pKeys->aKeyFrames[i].position[1] = key.Value.y;
		pKeys->aKeyFrames[i].position[2] = key.Value.z;
		pKeys->aKeyFrames[i].frame = (int)key.Time;
	}
}

void SetScaleKeys( cB3DChunk_NODE* node, ID3DXKeyframedAnimationSet* pKeyFrameAnimationSet, UINT iBone)
{
	D3DXKEY_VECTOR3 key;
	UINT nKeyFrames = pKeyFrameAnimationSet->GetNumScaleKeys(iBone);
	if (nKeyFrames == 0) return;

	cB3DChunk_KEYS* pKeys = new cB3DChunk_KEYS;
	node->lpKeys.push_back(pKeys);
	pKeys->Flags = 2;

	pKeys->aKeyFrames.resize( nKeyFrames );
	float StartingTime(0);

	for (UINT i(0); i<nKeyFrames; ++i)
	{
		pKeyFrameAnimationSet->GetScaleKey(iBone, i, &key);
		if (i==0) { 
			StartingTime = key.Time; 
			key.Time = 0; 
		}
		else 
			key.Time -= StartingTime;

		pKeys->aKeyFrames[i].scale[0] = key.Value.x;
		pKeys->aKeyFrames[i].scale[1] = key.Value.y;
		pKeys->aKeyFrames[i].scale[2] = key.Value.z;
		pKeys->aKeyFrames[i].frame = (int)key.Time;
	}
}

void SetRotationKeys( cB3DChunk_NODE* node, ID3DXKeyframedAnimationSet* pKeyFrameAnimationSet, UINT iBone)
{
	D3DXKEY_QUATERNION key;
	UINT nKeyFrames = pKeyFrameAnimationSet->GetNumRotationKeys(iBone);
	if (nKeyFrames == 0) return;

	cB3DChunk_KEYS* pKeys = new cB3DChunk_KEYS;
	node->lpKeys.push_back(pKeys);
	pKeys->Flags = 4;

	pKeys->aKeyFrames.resize( nKeyFrames );
	float StartingTime(0);

	for (UINT i(0); i<nKeyFrames; ++i)
	{
		pKeyFrameAnimationSet->GetRotationKey(iBone, i, &key);
		if (i==0) { 
			StartingTime = key.Time; 
			key.Time = 0; 
		}
		else 
			key.Time -= StartingTime;

		pKeys->aKeyFrames[i].rotation[0] = key.Value.w;
		pKeys->aKeyFrames[i].rotation[1] = key.Value.x;
		pKeys->aKeyFrames[i].rotation[2] = key.Value.y;
		pKeys->aKeyFrames[i].rotation[3] = key.Value.z;
		pKeys->aKeyFrames[i].frame = (int)key.Time;
	}
}


void GetMeshTransformation( FrameEx* frame, D3DXMATRIX *m )
{
	D3DXMATRIX mp;
	D3DXVECTOR3 Scale, Position;
	D3DXQUATERNION Quat;
	D3DXMatrixDecompose(&Scale, &Quat, &Position, &frame->TransformationMatrix );
	D3DXQuaternionInverse(&Quat, &Quat);
	D3DXMatrixTransformation(&mp, NULL, NULL, &Scale, NULL, &Quat, &Position );

	*m = *m * mp;

	if (frame->pParent) GetMeshTransformation( (FrameEx*)frame->pParent, m );
}

void FillMeshContainerData( D3DXFRAME* pFrame, cB3DChunk_NODE* pRoot, cB3DChunk_BB3D* bb3d, bool bApplyMatrixToMesh )
{
	D3DXMESHCONTAINER* pMeshContainer = pFrame->pMeshContainer;
	if (!pMeshContainer) return;

	D3DXMATRIX mm;
	D3DXMatrixIdentity( &mm );
	if (bApplyMatrixToMesh) GetMeshTransformation( (FrameEx*)pFrame, &mm );

	vector<cB3DChunk_BRUS*> apMaterials;

	CreateMaterials(pMeshContainer->NumMaterials, pMeshContainer->pMaterials, bb3d, apMaterials);
	CreateMesh(pMeshContainer->MeshData.pMesh, pRoot, apMaterials, &mm);
}

void FillNode( cB3DChunk_NODE* node, D3DXFRAME* pSceneRoot, cB3DChunk_BB3D* bb3d, LPD3DXSKININFO pSkinInfo, bool bApplyMatrixToMesh )
{
	node->Name = pSceneRoot->Name;

	D3DXVECTOR3 Scale, Position;
	D3DXQUATERNION Quat;
	D3DXMatrixDecompose(&Scale, &Quat, &Position, &pSceneRoot->TransformationMatrix );
	D3DXQuaternionInverse(&Quat, &Quat);
	memcpy( node->position, &Position, sizeof(float)*3 );
	memcpy( node->scale, &Scale, sizeof(float)*3 );
	node->rotation[0] = Quat.w;
	node->rotation[1] = Quat.x;
	node->rotation[2] = Quat.y;
	node->rotation[3] = Quat.z;	

	if (pSceneRoot->pMeshContainer && pSceneRoot->pMeshContainer->MeshData.pMesh)
	{
		FillMeshContainerData( pSceneRoot, node, bb3d, bApplyMatrixToMesh );
	}
	else
	{
		cB3DChunk_BONE* bone = new cB3DChunk_BONE;
		node->pTypeChunk = bone;
		if (pSkinInfo)
		{
			DWORD nBones = pSkinInfo->GetNumBones();
			for (DWORD ibone(0); ibone < nBones; ++ibone)
				if (strcmp(pSkinInfo->GetBoneName(ibone), pSceneRoot->Name)==0)
				{
					DWORD nVertices = pSkinInfo->GetNumBoneInfluences(ibone);
					DWORD* aVertex = new DWORD[nVertices];
					float* aInfluence = new float[nVertices];
					pSkinInfo->GetBoneInfluence(ibone, aVertex, aInfluence);
					bone->aSkinInfo.resize(nVertices);
					for (DWORD i(0); i<nVertices; ++i)
					{
						bone->aSkinInfo[i].iVertex = aVertex[i];
						bone->aSkinInfo[i].weight = aInfluence[i];
					}
					delete aVertex;
					delete aInfluence;
					break;
				}
		}
	}

}
void CreateHierarchy( cB3DChunk_NODE* pNodeRoot, D3DXFRAME*	pSceneRoot, LPD3DXSKININFO pSkinInfo, ID3DXAnimationController* pAnimCtrl, cB3DChunk_BB3D* bb3d, D3DXFRAME*	pSkipFrame )
{
	cB3DChunk_NODE* node = pNodeRoot;
	if ( !(pSceneRoot->pMeshContainer && pSkinInfo) )
	{
		if (pSceneRoot != pSkipFrame)
		{
			node = new cB3DChunk_NODE;
			FillNode( node, pSceneRoot, bb3d, pSkinInfo, false);

			pNodeRoot->lpNodes.push_back(node);

			if (pAnimCtrl)
			{
				int nAnimationSets = pAnimCtrl->GetNumAnimationSets();
				if (nAnimationSets)
				{
					ID3DXKeyframedAnimationSet* pKeyFrameAnimationSet;
					pAnimCtrl->GetAnimationSet(0, (LPD3DXANIMATIONSET*)&pKeyFrameAnimationSet);

					UINT iBone = -1;
					if SUCCEEDED(pKeyFrameAnimationSet->GetAnimationIndexByName(pSceneRoot->Name, &iBone))
					{
						SetScaleKeys( node, pKeyFrameAnimationSet, iBone );
						SetTranslationKeys( node, pKeyFrameAnimationSet, iBone );
						SetRotationKeys( node, pKeyFrameAnimationSet, iBone );
					}
				}
			}
		}
	}

	if(pSceneRoot->pFrameSibling) 
	    CreateHierarchy( pNodeRoot, pSceneRoot->pFrameSibling, pSkinInfo, pAnimCtrl, bb3d, pSkipFrame );
	if(pSceneRoot->pFrameFirstChild) 
		CreateHierarchy( node, pSceneRoot->pFrameFirstChild,  pSkinInfo, pAnimCtrl, bb3d, pSkipFrame );
}

bool ParseXFile(const char* filename, cB3DChunk_BB3D* bb3d, int OverrideFPS )
{
	cFrameAllocator FrameAllocator;

	D3DXFRAME*		pSceneRoot;
	ID3DXAnimationController*	pAnimCtrl(NULL);

	if ( FAILED( D3DXLoadMeshHierarchyFromXA(filename, D3DXMESH_MANAGED,
		_XtoB3D_pD3DDevice, &FrameAllocator, 0, // ignore user data  
		&pSceneRoot,	&pAnimCtrl) ) ) return false;
	
	LPD3DXFRAME pFirstFrameWithSkin = FindFirstSkinInfo( pSceneRoot );
	LPD3DXSKININFO pSkinInfo(NULL);

	if (pFirstFrameWithSkin)
	{
		pSkinInfo = pFirstFrameWithSkin->pMeshContainer->pSkinInfo;
	}
	else pFirstFrameWithSkin = pSceneRoot;

	cB3DChunk_NODE* pNode = new cB3DChunk_NODE;
	bb3d->lpNodes.push_back(pNode);
	FillNode( pNode, pFirstFrameWithSkin, bb3d, pSkinInfo, true );
	pNode->position[0] = pNode->position[1] = pNode->position[2] = 0; 
	pNode->scale[0] = pNode->scale[1] = pNode->scale[2] = 1; 
	pNode->rotation[0]=1;	pNode->rotation[1] = pNode->rotation[2] = pNode->rotation[3] = 0; 

		if (pAnimCtrl)
		{
			int nAnimationSets = pAnimCtrl->GetNumAnimationSets();
			if (nAnimationSets)
			{
				ID3DXKeyframedAnimationSet* pKeyFrameAnimationSet;
				pAnimCtrl->GetAnimationSet(0, (LPD3DXANIMATIONSET*)&pKeyFrameAnimationSet);

				cB3DChunk_ANIM* pAnimChunk = new cB3DChunk_ANIM;
				if (OverrideFPS == 0) pAnimChunk->fps		= (float)pKeyFrameAnimationSet->GetSourceTicksPerSecond();
								 else pAnimChunk->fps		= (float)OverrideFPS;
				pAnimChunk->nFrames = (int)(pKeyFrameAnimationSet->GetPeriod() * pKeyFrameAnimationSet->GetSourceTicksPerSecond());
				pNode->pAnim = pAnimChunk;
			}
		}

	CreateHierarchy( pNode, pSceneRoot, pSkinInfo, pAnimCtrl, bb3d, pFirstFrameWithSkin );

	D3DXFrameDestroy(pSceneRoot, &FrameAllocator);
	if (pAnimCtrl) pAnimCtrl->Release();

	return true;
}



bool ConvertXtoB3D_Custom( irr::core::stringc XFileName, irr::core::stringc B3DFileName, int OverrideFPS = 0 )
{
//	freopen("salida.txt", "w", stdout);
	if (_XtoB3D_pD3D == NULL) XtoB3D_CreateDX();
	cB3DChunk_BB3D bb3d;
	if ( ParseXFile(XFileName.c_str(), &bb3d, OverrideFPS) )
	{
		bb3d.ExportToFile((char*)B3DFileName.c_str());
		if (_XtoB3D_bManageDX) XtoB3D_ShutdownDX();
		return true;
	}

	if (_XtoB3D_bManageDX) XtoB3D_ShutdownDX();
	return false;
}

irr::core::stringc ToB3D_FindDestinationName(irr::core::stringc Source);

irr::core::stringc ConvertXtoB3D(irr::core::stringc Source, int OverrideFPS = 0)
{
	irr::core::stringc Dest = ToB3D_FindDestinationName(Source);

	if (Dest.size() == 0)
		return "";

	return (ConvertXtoB3D_Custom(Source, Dest, OverrideFPS))?Dest:"";
}