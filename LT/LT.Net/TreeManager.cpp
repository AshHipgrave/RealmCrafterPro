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
#include "Stdafx.h"
#include <d3dx9.h>
#include "..\LT\Source\CD3D9TreeRenderer.h"
#include "TreeManager.h"
#include <NGinString.h>
#include <vcclr.h>



namespace LTNet
{
	ITreeManager* LastManager = 0;
	int LastRTIndex = 0;
	void __cdecl CollisionChangedCallback(ITreeZone* TManager, LT::CollisionEventArgs* E);
	void RenderCallback(int rtIndex);
	void LostCallback();
	void ResetCallback(float X, float Y);

	TreeManager^ TreeManager::CreateTreeManager(IntPtr device, System::String^ trunkPath, System::String^ leafPath, System::String^ lodPath)
	{
		pin_ptr<const wchar_t> TrunkPinnedPath = PtrToStringChars(trunkPath);
		pin_ptr<const wchar_t> LeafPinnedPath = PtrToStringChars(leafPath);
		pin_ptr<const wchar_t> LODPinnedPath = PtrToStringChars(lodPath);
		NGin::WString TrunkPath(TrunkPinnedPath);
		NGin::WString LeafPath(LeafPinnedPath);
		NGin::WString LODPath(LODPinnedPath);

		ITreeRenderer* Renderer = LT::CreateDirect3D9TreeRenderer((IDirect3DDevice9*)(void*)device, TrunkPath.AsCString().c_str(), LeafPath.AsCString().c_str(), LODPath.AsCString().c_str());
		if(Renderer == 0)
			throw gcnew Exception("CreateDirect3D9TreeRenderer Failed!");

		ITreeManager* TManager = ITreeManager::CreateTreeManager(Renderer);
		if(TManager == 0)
			throw gcnew Exception("CreateTreeManager Failed!");

		TreeManager^ Manager = gcnew TreeManager(TManager);
		TreeManager::LastTreeManager = Manager;

		return Manager;
	}

	TreeManager::TreeManager(ITreeManager* handle)
		: _TreeManager(handle)
	{
		LastManager = _TreeManager;
		_Render = (IntPtr)&RenderCallback;
		_Reset = (IntPtr)&ResetCallback;
		_Lost = (IntPtr)&LostCallback;

		_TreeManager->CollisionChanged()->AddEvent(LTNet::CollisionChangedCallback);
	}

	unsigned int TreeManager::GetTreesDrawn()
	{
		return _TreeManager->GetTreesDrawn();
	}

	unsigned int TreeManager::GetLODsDrawn()
	{
		return _TreeManager->GetLODsDrawn();
	}

// 	bool TreeManager::Render(cli::array<float>^ view, cli::array<float>^ projection, NGUINet::NVector3^ cameraPosition)
// 	{
// 		if(view->Length != 16)
// 			throw gcnew Exception("view did not contain 16 floats!");
// 		if(projection->Length != 16)
// 			throw gcnew Exception("projection did not contain 16 floats!");
// 
// 		NGin::Math::Matrix View, Projection;
// 
// 		for(int i = 0; i < 16; ++i)
// 		{
// 			View.M[i] = view[i];
// 			Projection.M[i] = projection[i];
// 		}
// 
// 		return _TreeManager->Render(View, Projection, NGUINet::NVectorConverter::ToVector3(cameraPosition));
// 	}

	bool TreeManager::Render(cli::array<float>^ view, cli::array<float>^ projection, cli::array<float>^ lightProjection, float x, float y, float z)
	{
		if(view->Length != 16)
			throw gcnew Exception("view did not contain 16 floats!");
		if(projection->Length != 16)
			throw gcnew Exception("projection did not contain 16 floats!");

		NGin::Math::Matrix View, Projection, LightProjection;

		for(int i = 0; i < 16; ++i)
		{
			View.M[i] = view[i];
			Projection.M[i] = projection[i];
			LightProjection.M[i] = lightProjection[i];
		}

		return _TreeManager->Render(LastRTIndex, View, Projection, NGin::Math::Vector3(x, y, z), LightProjection);
	}

	bool TreeManager::Update(float x, float y, float z)
	{
		NGin::Math::Vector3 Pos(x, y, z);
		return _TreeManager->Update(reinterpret_cast<const float*>(&Pos));
	}

	TreeType^ TreeManager::LoadTreeType(System::String^ path)
	{
		pin_ptr<const wchar_t> PinnedPath = PtrToStringChars(path);
		NGin::WString Path(PinnedPath);

		ITreeType* TType = _TreeManager->LoadTreeType(Path.AsCString().c_str());
		if(TType == 0)
			return nullptr;

		return gcnew TreeType(TType);
	}

	bool TreeManager::ReloadTreeType(TreeType^ oldType, System::String^ path)
	{
		pin_ptr<const wchar_t> PinnedPath = PtrToStringChars(path);
		NGin::WString Path(PinnedPath);

		return _TreeManager->ReloadTreeType(oldType->GetHandle(), Path.AsCString().c_str());
	}

	TreeType^ TreeManager::FindTreeType(System::String^ name)
	{
		pin_ptr<const wchar_t> PinnedPath = PtrToStringChars(name);
		NGin::WString Name(PinnedPath);

		ITreeType* TType = _TreeManager->FindTreeType(Name.AsCString().c_str());
		if(TType == 0)
			return nullptr;

		return gcnew TreeType(TType);
	}

	TreeZone^ TreeManager::CreateZone()
	{
		return gcnew TreeZone(_TreeManager->CreateZone());
	}

	TreeZone^ TreeManager::LoadZone(System::String^ path)
	{
		pin_ptr<const wchar_t> PinnedPath = PtrToStringChars(path);
		NGin::WString Path(PinnedPath);

		ITreeZone* Zone = _TreeManager->LoadZone(Path.AsCString().c_str());
		if(Zone == 0)
			return nullptr;

		return gcnew TreeZone(Zone);
	}

	bool TreeManager::SaveZone(TreeZone^ zone, System::String^ path)
	{
		pin_ptr<const wchar_t> PinnedPath = PtrToStringChars(path);
		NGin::WString Path(PinnedPath);

		return _TreeManager->SaveZone(zone->GetHandle(), Path.AsCString().c_str());
	}

	void TreeManager::UnloadZone(TreeZone^ zone)
	{
		if(zone == nullptr)
			return;

		_TreeManager->UnloadZone(zone->GetHandle());
	}

	float TreeManager::LODDistance::get()
	{
		return _TreeManager->GetLODDistance();
	}

	void TreeManager::LODDistance::set(float value)
	{
		_TreeManager->SetLODDistance(value);
	}

	float TreeManager::RenderDistance::get()
	{
		return _TreeManager->GetRenderDistance();
	}

	void TreeManager::RenderDistance::set(float value)
	{
		_TreeManager->SetRenderDistance(value);
	}

	float TreeManager::PointLightDistance::get()
	{
		return _TreeManager->GetPointLightDistance();
	}

	void TreeManager::PointLightDistance::set(float value)
	{
		_TreeManager->SetPointLightDistance(value);
	}

	void TreeManager::SetShadowMap(System::IntPtr handle)
	{
		_TreeManager->GetRenderer()->SetShadowMap(handle.ToPointer());
	}

	IntPtr TreeManager::GetDirectionBuffer()
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

	IntPtr TreeManager::GetColorBuffer()
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

	void TreeManager::SetDirectionBuffer(IntPtr Buffer)
	{
		float** Directions = (float**)Buffer.ToPointer();

		for(int i = 0; i < 3; ++i)
			_TreeManager->GetRenderer()->SetLightNormal(i, NGin::Math::Vector3(Directions[i][0], Directions[i][1], Directions[i][2]));

		for(int i = 0; i < 3; ++i)
			delete Directions[i];
		delete Directions;
	}

	void TreeManager::SetColorBuffer(IntPtr Buffer)
	{
		float** Colors = (float**)Buffer.ToPointer();

		for(int i = 0; i < 3; ++i)
			_TreeManager->GetRenderer()->SetLightColor(i, NGin::Math::Color(Colors[i][0], Colors[i][1], Colors[i][2]));

		for(int i = 0; i < 3; ++i)
			delete Colors[i];
		delete Colors;
	}

	void TreeManager::SetPointLights(IntPtr lightPtr, IntPtr count, IntPtr stride)
	{
		_TreeManager->GetRenderer()->SetPointLights((void**)(void*)lightPtr, *((int*)(void*)count), *((int*)(void*)stride));
	}

	void TreeManager::SetLightNormal(int index, float x, float y, float z)
	{
		_TreeManager->GetRenderer()->SetLightNormal(index, NGin::Math::Vector3(x, y, z));
	}

	void TreeManager::SetLightColor(int index, System::Drawing::Color^ color)
	{
		_TreeManager->GetRenderer()->SetLightColor(index, NGUINet::NVectorConverter::ToColor(color));
	}

	void TreeManager::SetAmbientLight(System::Drawing::Color^ color)
	{
		_TreeManager->GetRenderer()->SetAmbientLight(NGUINet::NVectorConverter::ToColor(color));
	}

	void TreeManager::SetFog(System::Drawing::Color^ color, float data1, float data2)
	{
		_TreeManager->GetRenderer()->SetFog(NGUINet::NVectorConverter::ToColor(color),
			NGin::Math::Vector2(data1, data2));
	}

	struct UnManagedPointLight
	{
		NGin::Math::Vector3 Position;
		NGin::Math::Color Color;
		int Radius;
		bool Active;
		float Distance;
	};


	void TreeManager::SetPointLights(cli::array<TreePointLight^>^ lights)
	{
		UnManagedPointLight** Lights = new UnManagedPointLight*[lights->Length];

		for(int i = 0; i < lights->Length; ++i)
		{
			Lights[i] = new UnManagedPointLight();
			Lights[i]->Position.Reset(lights[i]->PositionX, lights[i]->PositionY, lights[i]->PositionZ);
			Lights[i]->Color = NGUINet::NVectorConverter::ToColor(lights[i]->Color);
			Lights[i]->Active = lights[i]->Active;
			Lights[i]->Radius = lights[i]->Radius;
			Lights[i]->Distance = lights[i]->Distance;
		}

		_TreeManager->GetRenderer()->SetPointLights((void**)Lights, lights->Length, sizeof(UnManagedPointLight));

		for(int i = 0; i < lights->Length; ++i)
			delete Lights[i];
		delete Lights;
	}

	void TreeManager::SetSwayDirection(float directionx, float directionz)
	{
		_TreeManager->GetRenderer()->SetSwayDirection(NGin::Math::Vector2(directionx, directionz));
	}

	void TreeManager::SetSwayPower(float power)
	{
		_TreeManager->GetRenderer()->SetSwayPower(power);
	}

	void TreeManager::SetQualityLevel(int qualityLevel)
	{
		_TreeManager->GetRenderer()->SetQualityLevel(qualityLevel);
	}

	
	

	void __cdecl CollisionChangedCallback(ITreeZone* TManager, LT::CollisionEventArgs* E)
	{
		if(TreeManager::LastTreeManager != nullptr)
			TreeManager::LastTreeManager->__CollisionChangedEvent(TManager, E);
	}

	void RenderCallback(int rtIndex)
	{
		if(TreeManager::LastTreeManager != nullptr)
			TreeManager::LastTreeManager->__TreeRender(rtIndex);
	}

	void LostCallback()
	{
		if(LastManager != 0)
			LastManager->GetRenderer()->OnDeviceLost();
	}

	void ResetCallback(float X, float Y)
	{
		if(LastManager != 0)
			LastManager->GetRenderer()->OnDeviceReset();
	}

	IntPtr TreeManager::GetRenderCallback()
	{
		return _Render;
	}

	IntPtr TreeManager::GetLostCallback()
	{
		return _Lost;
	}

	IntPtr TreeManager::GetResetCallback()
	{
		return _Reset;
	}

	void TreeManager::__TreeRender(int rtIndex)
	{
		LastRTIndex = rtIndex;
		TreeRender(this, System::EventArgs::Empty);
	}

	void TreeManager::__CollisionChangedEvent(ITreeZone* treeZone, LT::CollisionEventArgs* E)
	{
		const STreeCollisionData* Data = E->GetCollisionData();
		ITreeInstance* Instance = (ITreeInstance*)E->GetTreeInstance();
		TreeCollisionData^ CollisionData = nullptr;

		if(Data != 0)
		{
			CollisionData = gcnew TreeCollisionData();
			CollisionData->InstanceCount = Data->InstanceCount;
			CollisionData->Instances = gcnew cli::array<TreeCollisionData::DataInstance^>(Data->InstanceCount);

			for(int i = 0; i < (int)Data->InstanceCount; ++i)
			{
				TreeCollisionData::DataInstance^ DI = gcnew TreeCollisionData::DataInstance();
				DI->DimensionsX = Data->Instances[i].Dimensions.X;
				DI->DimensionsY = Data->Instances[i].Dimensions.Y;
				DI->DimensionsZ = Data->Instances[i].Dimensions.Z;
				DI->OffsetX = Data->Instances[i].Offset.X;
				DI->OffsetY = Data->Instances[i].Offset.Y;
				DI->OffsetZ = Data->Instances[i].Offset.Z;
				DI->Type = (LTNet::ETreeCollisionType)Data->Instances[i].Type;

				CollisionData->Instances[i] = DI;
			}
	
		}

		CollisionChangedEventArgs^ Args = gcnew CollisionChangedEventArgs(CollisionData, gcnew TreeInstance(Instance));
		CollisionChanged(gcnew TreeZone(treeZone), Args);
	}

}