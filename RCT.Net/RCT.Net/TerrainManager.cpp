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
// This is the main DLL file.

#include "stdafx.h"

//#include "NVector2.h"
#include "TerrainManager.h"
#include <nginstring.h>



namespace RealmCrafter
{
namespace RCT
{

	CollisionChangedEventArgs::CollisionChangedEventArgs(IntPtr triangleList, System::UInt32 vertexCount, NGUINet::NVector3^ position, NGUINet::NVector3^ terrainPosition, bool priority)
	{
		_TriangleList = triangleList;
		_VertexCount = vertexCount;
		_Position = position;
		_TPosition = terrainPosition;
		_Priority = priority;
	}

	bool CollisionChangedEventArgs::HighPriority::get()
	{
		return _Priority;
	}

	IntPtr CollisionChangedEventArgs::TriangeList::get()
	{
		return _TriangleList;
	}

	System::UInt32 CollisionChangedEventArgs::VertexCount::get()
	{
		return _VertexCount;
	}

	NGUINet::NVector3^ CollisionChangedEventArgs::Position::get()
	{
		return _Position;
	}

	NGUINet::NVector3^ CollisionChangedEventArgs::TerrainPosition::get()
	{
		return _TPosition;
	}


	RealmCrafter::RCT::ITerrainManager* LastManager = 0;

	void CollisionChangedCallback(RealmCrafter::RCT::ITerrain* Terrain, RealmCrafter::RCT::CollisionEventArgs* E)
	{
		if(TerrainManager::LastTerrainManager != nullptr)
			TerrainManager::LastTerrainManager->__CollisionChangedEvent(Terrain, E);
	}

	
	void RenderCallback(int rtindex)
	{
		if(TerrainManager::LastTerrainManager != nullptr)
			TerrainManager::LastTerrainManager->__TerrainRender(rtindex);
	}

	void RenderShadowDepthCallback()
	{
		if(TerrainManager::LastTerrainManager != nullptr)
			TerrainManager::LastTerrainManager->__TerrainRenderDepth();
	}

	void LostCallback()
	{
		if(LastManager != 0)
			LastManager->OnDeviceLost();
	}

	void ResetCallback(float X, float Y)
	{
		if(LastManager != 0)
			LastManager->OnDeviceReset();
	}

	IntPtr TerrainManager::GetRenderCallback()
	{
		return _Render;
	}

	IntPtr TerrainManager::GetRenderDepthCallback()
	{
		return _ShadowDepth;
	}


	IntPtr TerrainManager::GetLostCallback()
	{
		return _Lost;
	}

	IntPtr TerrainManager::GetResetCallback()
	{
		return _Reset;
	}

	int LastRTIndex = 0;
	void TerrainManager::__TerrainRender(int rtIndex)
	{
		LastRTIndex = rtIndex;
		TerrainRender(this, System::EventArgs::Empty);
	}

	void TerrainManager::__TerrainRenderDepth()
	{
		TerrainRenderDepth(this, System::EventArgs::Empty);
	}

	void TerrainManager::__CollisionChangedEvent(RealmCrafter::RCT::ITerrain* Terrain, RealmCrafter::RCT::CollisionEventArgs* E)
	{
		IntPtr TPtr = (IntPtr)(void*)Terrain;

		CollisionChangedEventArgs^ Args = gcnew CollisionChangedEventArgs((IntPtr)(void*)E->GetTriangleList(),
			E->GetVertexCount(),
			NGUINet::NVectorConverter::FromVector3(E->GetPosition()),
			NGUINet::NVectorConverter::FromVector3(E->GetTerrainPosition()),
			E->GetHighPriority());

		for each(RCTerrain^ T in _Terrains)
		{
			if(T->GetHandle() == TPtr)
			{
				CollisionChanged(T, Args);
			}
		}
	}

#pragma region Constructors
	TerrainManager::TerrainManager(IntPtr device, System::String^ t1EffectPath, System::String^ t1EditorPath, System::String^ grassEffectPath, System::String^ depthEffectPath)
	{
		pin_ptr<const wchar_t> EffectPinnedPath = PtrToStringChars(t1EffectPath);
		pin_ptr<const wchar_t> EditorPinnedPath = PtrToStringChars(t1EditorPath);
		pin_ptr<const wchar_t> GrassEffectPinnedPath = PtrToStringChars(grassEffectPath);
		pin_ptr<const wchar_t> DepthEffectPinnedPath = PtrToStringChars(depthEffectPath);
		NGin::WString wEffectPath(EffectPinnedPath);
		NGin::WString wEditorPath(EditorPinnedPath);
		NGin::WString wGrassEffectPath(GrassEffectPinnedPath);
		NGin::WString wDepthEffectPath(DepthEffectPinnedPath);
		
		_Manager = RealmCrafter::RCT::CreateTerrainManager((IDirect3DDevice9*)(void*)device, wEffectPath.AsCString().c_str(), wEditorPath.AsCString().c_str(), wGrassEffectPath.AsCString().c_str(), wDepthEffectPath.AsCString().c_str());
		if(_Manager == 0)
			throw gcnew Exception("Failed to create TerrainManager!");
//		_Manager = NGin::GUI::CreateGUIManager((IDirect3DDevice9*)(void*)D3DDevice, NVectorConverter::ToVector2(Resolution), wShaderPath);
		_Terrains = gcnew System::Collections::Generic::List<RCT::RCTerrain^>();

		TerrainManager::LastTerrainManager = this;
		LastManager = _Manager;
		_Render = (IntPtr)&RenderCallback;
		_ShadowDepth = (IntPtr)&RenderShadowDepthCallback;
		_Reset = (IntPtr)&ResetCallback;
		_Lost = (IntPtr)&LostCallback;

		_Manager->CollisionChanged()->AddEvent(RealmCrafter::RCT::CollisionChangedCallback);
	}

	TerrainManager::~TerrainManager()
	{
		delete _Manager;
		LastManager = nullptr;
	}

	//void TerrainManager::Register(NGUINet::NControl^ Control)
	//{
	//	_Controls->Add(Control);
	//}

	//void TerrainManager::UnRegister(NGUINet::NControl^ Control)
	//{
	//	_Controls->Remove(Control);
	//}

#pragma endregion

	IntPtr TerrainManager::GetDirectionBuffer()
	{
		float** Directions = new float*[3];
		for(int i = 0; i < 3; ++i)
		{
			Directions[i] = new float[3];

			Directions[i][0] = 0.0f;
			Directions[i][1] = 0.0f;
			Directions[i][2] = 0.0f;
		}

		return (IntPtr)(void*)Directions;
	}

	IntPtr TerrainManager::GetColorBuffer()
	{
		float** Colors = new float*[3];
		for(int i = 0; i < 3; ++i)
		{
			Colors[i] = new float[3];

			Colors[i][0] = 0.0f;
			Colors[i][1] = 0.0f;
			Colors[i][2] = 0.0f;
		}

		return (IntPtr)(void*)Colors;
	}

	void TerrainManager::SetDirectionBuffer(IntPtr Buffer)
	{
		float** Directions = (float**)Buffer.ToPointer();

		for(int i = 0; i < 3; ++i)
			_Manager->SetLightNormal(i, NGin::Math::Vector3(Directions[i][0], Directions[i][1], Directions[i][2]));

		for(int i = 0; i < 3; ++i)
			delete Directions[i];
		delete Directions;
	}

	void TerrainManager::SetColorBuffer(IntPtr Buffer)
	{
		float** Colors = (float**)Buffer.ToPointer();

		for(int i = 0; i < 3; ++i)
			_Manager->SetLightColor(i, NGin::Math::Color(Colors[i][0], Colors[i][1], Colors[i][2]));

		for(int i = 0; i < 3; ++i)
			delete Colors[i];
		delete Colors;
	}

	void TerrainManager::SetPointLights(IntPtr lightPtr, IntPtr count, IntPtr stride)
	{
		_Manager->SetPointLights((void**)(void*)lightPtr, *((int*)(void*)count), *((int*)(void*)stride));
	}

	RCTerrain^ TerrainManager::CreateT1(int size)
	{
		RealmCrafter::RCT::ITerrain* Terrain = _Manager->CreateT1(size);

		RCTerrain^ T = gcnew RCTerrain((IntPtr)(void*)Terrain);
		_Terrains->Add(T);
		return T;
	}

	RCTerrain^ TerrainManager::LoadT1(System::String^ path)
	{
		pin_ptr<const wchar_t> wPath = PtrToStringChars(path);

		RealmCrafter::RCT::ITerrain* Terrain = _Manager->LoadT1(NGin::WString(wPath).AsCString().c_str());
		if(Terrain == 0)
			return nullptr;
		else
		{
			RCTerrain^ T = gcnew RCTerrain((IntPtr)(void*)Terrain);
			_Terrains->Add(T);
			return T;
		}
	}

	bool TerrainManager::SaveT1(RCTerrain^ terrain, System::String^ path)
	{
		pin_ptr<const wchar_t> wPath = PtrToStringChars(path);

		return _Manager->SaveT1((RealmCrafter::RCT::ITerrain*)(void*)terrain->GetHandle(), NGin::WString(wPath).AsCString().c_str());
	}

	void TerrainManager::Destroy(RCTerrain^ terrain)
	{
		_Terrains->Remove(terrain);
		terrain->MgrDstr();
	}

	void TerrainManager::Update(NGUINet::NVector3^ position, bool updateCollisions)
	{
		_Manager->Update(NGUINet::NVectorConverter::ToVector3(position), updateCollisions);
	}

	void TerrainManager::Render(cli::array<float>^ viewProjection, cli::array<float>^ lightProjection, IntPtr ShadowMap)
	{
		if(viewProjection->Length != 16)
			throw gcnew System::Exception("viewProjection did not contain 16 floats!");

		float* VP = new float[16];
		for(int i = 0; i < 16; ++i)
			VP[i] = viewProjection[i];

		float* LP = new float[16];
		for(int i = 0; i < 16; ++i)
			LP[i] = lightProjection[i];

		_Manager->Render(LastRTIndex, VP, LP, ShadowMap.ToPointer());
		delete VP;
		delete LP;
	}

	void TerrainManager::RenderDepth(cli::array<float>^ viewProjection)
	{

		if(viewProjection->Length != 16)
			throw gcnew System::Exception("viewProjection did not contain 16 floats!");

		float* VP = new float[16];
		for(int i = 0; i < 16; ++i)
			VP[i] = viewProjection[i];

		_Manager->RenderDepth(VP);
		delete VP;
	}

	void TerrainManager::SetLightNormal(int index, NGUINet::NVector3^ normal)
	{
		_Manager->SetLightNormal(index, NGUINet::NVectorConverter::ToVector3(normal));
	}

	void TerrainManager::SetLightColor(int index, System::Drawing::Color^ color)
	{
		_Manager->SetLightColor(index, NGUINet::NVectorConverter::ToColor(color));
	}

	void TerrainManager::SetAmbientLight(System::Drawing::Color^ color)
	{
		_Manager->SetAmbientLight(NGUINet::NVectorConverter::ToColor(color));
	}


	struct UnManagedPointLight
	{
		NGin::Math::Vector3 Position;
		NGin::Math::Color Color;
		int Radius;
		bool Active;
		float Distance;
	};


	void TerrainManager::SetPointLights(cli::array<TerrainPointLight^>^ lights)
	{
		UnManagedPointLight** Lights = new UnManagedPointLight*[lights->Length];

		for(int i = 0; i < lights->Length; ++i)
		{
			Lights[i] = new UnManagedPointLight();
			Lights[i]->Position = NGUINet::NVectorConverter::ToVector3(lights[i]->Position);
			Lights[i]->Color = NGUINet::NVectorConverter::ToColor(lights[i]->Color);
			Lights[i]->Active = lights[i]->Active;
			Lights[i]->Radius = lights[i]->Radius;
			Lights[i]->Distance = lights[i]->Distance;
		}
		
		_Manager->SetPointLights((void**)Lights, lights->Length, sizeof(UnManagedPointLight));
		
		for(int i = 0; i < lights->Length; ++i)
			delete Lights[i];
		delete Lights;
	}

	void TerrainManager::SetFog(System::Drawing::Color^ color, NGUINet::NVector2^ data)
	{
		_Manager->SetFog(NGUINet::NVectorConverter::ToColor(color),
			NGUINet::NVectorConverter::ToVector2(data));
	}

	void TerrainManager::SetSecondBrush(float radius, float hardness, NGUINet::NVector2^ position, bool circle)
	{
		_Manager->SetSecondBrush(radius, hardness, NGUINet::NVectorConverter::ToVector2(position), circle);
	}
	
	bool TerrainManager::LoadGrassTypes(System::String^ path)
	{
		pin_ptr<const wchar_t> wPath = PtrToStringChars(path);

		return this->_Manager->LoadGrassTypes(WString(wPath).AsCString().c_str());
	}

	bool TerrainManager::SaveGrassTypes(System::String^ path)
	{
		pin_ptr<const wchar_t> wPath = PtrToStringChars(path);

		return this->_Manager->SaveGrassTypes(WString(wPath).AsCString().c_str());
	}

	GrassType^ TerrainManager::CreateGrassType()
	{
		return gcnew GrassType(_Manager->CreateGrassType());
	}

	GrassType^ TerrainManager::FindGrassType(System::String^ name)
	{
		pin_ptr<const wchar_t> wName = PtrToStringChars(name);

		IGrassType* T = _Manager->FindGrassType(WString(wName).AsCString().c_str());

		if(T == 0)
			return nullptr;
		else
			return gcnew GrassType(T);
	}

	cli::array<GrassType^>^ TerrainManager::FetchGrassTypes()
	{
		NGin::ArrayList<IGrassType*> InTypes;
		_Manager->FetchGrassTypes(InTypes);

		cli::array<GrassType^>^ Types = gcnew cli::array<GrassType^>(InTypes.Size());

		for(unsigned int i = 0; i < InTypes.Size(); ++i)
			Types[i] = gcnew GrassType(InTypes[i]);

		return Types;
	}

	void TerrainManager::RemoveGrassType(GrassType^ type)
	{
		_Manager->RemoveGrassType(type->Handle);
	}


	float TerrainManager::BrushRadius::get()
	{
		return _Manager->GetBrushRadius();
	}

	void TerrainManager::BrushRadius::set(float value)
	{
		_Manager->SetBrushRadius(value);
	}

	float TerrainManager::BrushHardness::get()
	{
		return _Manager->GetBrushHardness();
	}
	
	void TerrainManager::BrushHardness::set(float value)
	{
		_Manager->SetBrushHardness(value);
	}
	
	NGUINet::NVector2^ TerrainManager::BrushPosition::get()
	{
		return NGUINet::NVectorConverter::FromVector2(_Manager->GetBrushPosition());
	}
	
	void TerrainManager::BrushPosition::set(NGUINet::NVector2^ value)
	{
		_Manager->SetBrushPosition(NGUINet::NVectorConverter::ToVector2(value));
	}

	bool TerrainManager::BrushIsCircle::get()
	{
		return _Manager->GetBrushIsCircle();
	}
	
	void TerrainManager::BrushIsCircle::set(bool value)
	{
		_Manager->SetBrushIsCircle(value);
	}

	bool TerrainManager::EditorMode::get()
	{
		return _Manager->GetEditorMode();
	}
	
	void TerrainManager::EditorMode::set(bool value)
	{
		_Manager->SetEditorMode(value);
	}

	bool TerrainManager::EditorLocked::get()
	{
		return _Manager->GetEditorLock();
	}
	
	void TerrainManager::EditorLocked::set(bool value)
	{
		_Manager->SetEditorLock(value);
	}

	bool TerrainManager::RenderTerrain::get()
	{
		return _Manager->GetTerrainRender();
	}

	void TerrainManager::RenderTerrain::set(bool value)
	{
		_Manager->SetTerrainRender(value);
	}

	bool TerrainManager::RenderGrass::get()
	{
		return _Manager->GetGrassRender();
	}

	void TerrainManager::RenderGrass::set(bool value)
	{
		_Manager->SetGrassRender(value);
	}

	bool TerrainManager::RenderCollision::get()
	{
		return _Manager->GetCollisionRender();
	}

	void TerrainManager::RenderCollision::set(bool value)
	{
		_Manager->SetCollisionRender(value);
	}

// 
// 	bool TerrainManager::TerrainRender::get()
// 	{
// 		return _Manager->GetTerrainRender();
// 	}
// 
// 	void TerrainManager::TerrainRender::set(bool set)
// 	{
// 		_Manager->SetTerrainRender(set);
// 	}
// 
// 	bool TerrainManager::GrassRender::get()
// 	{
// 		return _Manager->GetGrassRender();
// 	}
// 
// 	void TerrainManager::GrassRender::set(bool set)
// 	{
// 		_Manager->SetGrassRender(set);
// 	}
}
}