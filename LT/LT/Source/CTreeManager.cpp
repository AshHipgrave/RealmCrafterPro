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

#include <algorithm>

#include "CTreeManager.h"
#include "CTreeInstance.h"
#include "CTreeType.h"

namespace LT
{
	CTreeManager::CTreeManager(ITreeRenderer* renderer)
		: Renderer(renderer), TreesDrawn(0), LODsDrawn(0),
		LODDistance(50.0f), RenderDistance(1024.0f), PointLightDistance(50.0f)
	{
		CollisionChangedEvent = new CollisionEventHandler();
	}

	CTreeManager::~CTreeManager()
	{

	}

	CollisionEventHandler* CTreeManager::CollisionChanged()
	{
		return CollisionChangedEvent;
	}

	ITreeRenderer* CTreeManager::GetRenderer() const
	{
		return Renderer;
	}

	bool CTreeManager::RenderDepth(NGin::Math::Matrix view, NGin::Math::Matrix projection, NGin::Math::Vector3 cameraPosition)
	{
		return RenderDepth(view, projection, cameraPosition, NGin::Math::Vector3(0, 0, 0));
	}

	bool CTreeManager::RenderDepth(NGin::Math::Matrix view, NGin::Math::Matrix projection, NGin::Math::Vector3 cameraPosition, NGin::Math::Vector3 &offset)
	{
		NGin::Math::Matrix LightProjection = projection * view;
		RealmCrafter::RCT::Frustum ViewFrustum;
		ViewFrustum.FromViewProjection(*(reinterpret_cast<D3DXMATRIX*>(&LightProjection)));

		Renderer->PreRender();

		TParameter<NGin::Math::Matrix> LightProjectionParam;
		LightProjectionParam.SetData(LightProjection);
		Renderer->SetParameter("LightViewProjection", LightProjectionParam, true);

		TParameter<NGin::Math::Matrix> ViewParam;
		ViewParam.SetData(view);
		Renderer->SetParameter("LightView", ViewParam, true);

		TParameter<NGin::Math::Matrix> ProjectionParam;
		ProjectionParam.SetData(projection);
		Renderer->SetParameter("LightProjection", ProjectionParam, true);

		TParameter<NGin::Math::Vector3> CameraPositionParam;
		CameraPositionParam.SetData(cameraPosition);
		Renderer->SetParameter("CameraPosition", CameraPositionParam, true);

 		for(int z = 0; z < LoadedZones.size(); ++z)
		{
 			LoadedZones[z]->RenderDepth(ViewFrustum, cameraPosition, TreesDrawn, LODsDrawn, offset);
		}

		Renderer->PostRender();

		return true;
	}

	bool CTreeManager::Render(int rtIndex, NGin::Math::Matrix view, NGin::Math::Matrix projection, NGin::Math::Vector3 cameraPosition, NGin::Math::Matrix lightProj)
	{
		return Render(rtIndex, view, projection, cameraPosition, lightProj, NGin::Math::Vector3(0, 0, 0));
	}
	
	bool CTreeManager::Render(int rtIndex, NGin::Math::Matrix view, NGin::Math::Matrix projection, NGin::Math::Vector3 cameraPosition, NGin::Math::Matrix lightProj, NGin::Math::Vector3 &offset)
	{
		if(rtIndex != 0)
			return true;

		TreesDrawn = 0;
		LODsDrawn = 0;

		NGin::Math::Matrix ViewProjection = projection * view;

		RealmCrafter::RCT::Frustum ViewFrustum;
		ViewFrustum.FromViewProjection(*(reinterpret_cast<D3DXMATRIX*>(&ViewProjection)));

		Renderer->PreRender();

		TParameter<NGin::Math::Matrix> ViewProjectionParam;
		ViewProjectionParam.SetData(ViewProjection);
		Renderer->SetParameter("ViewProjection", ViewProjectionParam, true);

		TParameter<NGin::Math::Matrix> ViewParam;
		ViewParam.SetData(view);
		Renderer->SetParameter("View", ViewParam, true);

		TParameter<NGin::Math::Matrix> ProjectionParam;
		ProjectionParam.SetData(projection);
		Renderer->SetParameter("Projection", ProjectionParam, true);

		NGin::Math::Matrix ViewInverse = view;
		//ViewInverse.Invert();
		D3DXMatrixInverse(reinterpret_cast<D3DXMATRIX*>(&ViewInverse), NULL, reinterpret_cast<D3DXMATRIX*>(&view));
		TParameter<NGin::Math::Matrix> ViewInverseParam;
		ViewInverseParam.SetData(view);
		Renderer->SetParameter("ViewInverse", ViewInverseParam, true);

		TParameter<NGin::Math::Vector3> CameraPositionParam;
		CameraPositionParam.SetData(cameraPosition);
		Renderer->SetParameter("CameraPosition", CameraPositionParam, true);

		TParameter<NGin::Math::Matrix> LightProjectionParam;
		LightProjectionParam.SetData(lightProj);
		Renderer->SetParameter("LightProjection", LightProjectionParam, true);

		for(int z = 0; z < LoadedZones.size(); ++z)
			LoadedZones[z]->Render(rtIndex, ViewFrustum, cameraPosition, TreesDrawn, LODsDrawn, offset);

		Renderer->PostRender();

		return true;
	}

	unsigned int CTreeManager::GetTreesDrawn() const
	{
		return TreesDrawn;
	}

	unsigned int CTreeManager::GetLODsDrawn() const
	{
		return LODsDrawn;
	}

	bool CTreeManager::Update(const float* cameraPosition)
	{
		return false;
	}

	ITreeType* CTreeManager::LoadTreeType(const char* path)
	{
		FILE* File = fopen(path, "rb");
		if(File == 0)
			return 0;

		ITreeType* LoadedType = CTreeType::LoadTreeType(this, File, path, 0);
		
		fclose(File);
		if(LoadedType != 0)
			LoadedTypes.push_back(LoadedType);

		return LoadedType;
	}

	bool CTreeManager::ReloadTreeType(ITreeType* oldType, const char* path)
	{
		FILE* File = fopen(path, "rb");
		if(File == 0)
			return false;

		ITreeType* type = CTreeType::LoadTreeType(this, File, path, dynamic_cast<CTreeType*>(oldType));

		fclose(File);

		if(type == 0)
			return false;

		return true;
	}

	ITreeType* CTreeManager::FindTreeType(const char* name) const
	{
		std::string Needle = name;
		std::transform(Needle.begin(), Needle.end(), Needle.begin(), ::tolower);

		for(int i = 0; i < LoadedTypes.size(); ++i)
			if(LoadedTypes[i]->IsType(Needle.c_str()))
				return LoadedTypes[i];

		return 0;
	}

	ITreeZone* CTreeManager::LoadZone(const char* path)
	{
		CTreeZone* Zone = new CTreeZone(this);
		
		if(!Zone->Load(path))
		{
			delete Zone;
			return 0;
		}

		LoadedZones.push_back(Zone);
		return Zone;
	}

	void CTreeManager::UnloadZone(ITreeZone* zone)
	{
		if(zone == 0)
			return;

		int Found = -1;
		for(int i = 0; i < LoadedZones.size(); ++i)
		{
			if(LoadedZones[i] == zone)
			{
				Found = i;
				break;
			}
		}

		// Not in the list, so invalid
		if(Found == -1)
			return;

		LoadedZones.erase(LoadedZones.begin() + Found);

		delete zone;
	}

	bool CTreeManager::SaveZone(const ITreeZone* zone, const char* path) const
	{
		if(zone != 0)
			return dynamic_cast<const CTreeZone*>(zone)->Save(path);
		return false;
	}

	ITreeZone* CTreeManager::CreateZone()
	{
		CTreeZone* Zone = new CTreeZone(this);
		LoadedZones.push_back(Zone);
		return Zone;
	}

	void CTreeManager::SetLODDistance(float distance)
	{
		LODDistance = distance;
	}

	void CTreeManager::SetRenderDistance(float distance)
	{
		RenderDistance = distance;
	}

	void CTreeManager::SetPointLightDistance(float distance)
	{
		PointLightDistance = distance;
	}

	float CTreeManager::GetLODDistance() const
	{
		return LODDistance;
	}

	float CTreeManager::GetRenderDistance() const
	{
		return RenderDistance;
	}

	float CTreeManager::GetPointLightDistance() const
	{
		return PointLightDistance;
	}
	
#ifdef EXPORTS
	__declspec(dllexport)
#else
	__declspec(dllimport)
#endif
		ITreeManager* ITreeManager::CreateTreeManager(ITreeRenderer* renderer)
	{
		return new CTreeManager(renderer);
	}
}
