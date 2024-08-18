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
#include "stdafx.h"

#include "RCTerrain.h"
#include <nginstring.h>

namespace RealmCrafter
{
namespace RCT
{
	RCTerrain::RCTerrain(IntPtr handle)
	{
		Handle = (RealmCrafter::RCT::ITerrain*)(void*)handle;
		_Tag = nullptr;
	}

	RCTerrain::~RCTerrain()
	{
		System::Diagnostics::Trace::WriteLine("Delete");
		if(Handle != 0)
			delete Handle;
	}

	void RCTerrain::MgrDstr()
	{
		if(Handle != 0)
			delete Handle;
		Handle = 0;
	}

	IntPtr RCTerrain::GetHandle()
	{
		return (IntPtr)(void*)Handle;
	}

	int RCTerrain::GetSize()
	{
		return Handle->GetSize();
	}

	void RCTerrain::SetTextureScale(int index, float scale)
	{
		Handle->SetTextureScale(index, scale);
	}

	float RCTerrain::GetTextureScale(int index)
	{
		return Handle->GetTextureScale(index);
	}

	void RCTerrain::SetTexture(int index, System::String^ path)
	{
		pin_ptr<const wchar_t> PinnedPath = PtrToStringChars(path);
		NGin::WString wPath(PinnedPath);

		Handle->SetTexture(index, std::string(wPath.AsCString().c_str()));
	}

	System::String^ RCTerrain::GetTexture(int index)
	{
		NGin::WString wPath = Handle->GetTexture(index).c_str();

		return gcnew System::String((const wchar_t*)wPath.w_str());
	}

	void RCTerrain::SetGrassType(int index, System::String^ type)
	{
		pin_ptr<const wchar_t> PinnedType = PtrToStringChars(type);
		NGin::WString wType(PinnedType);

		Handle->SetGrassType(index, wType.AsCString().c_str());
	}

	System::String^ RCTerrain::GetGrassType(int index)
	{
		return gcnew System::String((const wchar_t*)NGin::WString(Handle->GetGrassType(index).c_str()).w_str());
	}
	
	void RCTerrain::SetHeight(int x, int y, float height)
	{
		Handle->SetHeight(x, y, height);
	}

	void RCTerrain::SetHeight(int chunkX, int chunkY, int x, int y, float height)
	{
		Handle->SetHeight(chunkX, chunkY, x, y, height);
	}

	float RCTerrain::GetHeight(int x, int y)
	{
		return Handle->GetHeight(x, y);
	}

	float RCTerrain::GetHeight(int chunkX, int chunkY, int x, int y)
	{
		return Handle->GetHeight(chunkX, chunkY, x, y);
	}

	void RCTerrain::SetGrass(int x, int y, System::Byte gr)
	{
		Handle->SetGrass(x, y, Convert::ToUInt32(gr));
	}

	System::Byte RCTerrain::GetGrass(int x, int y)
	{
		return Convert::ToByte(Handle->GetGrass(x, y));
	}

	void RCTerrain::SetExclusion(int x, int y, bool exclude)
	{
		Handle->SetExclusion(x, y, exclude);
	}

	bool RCTerrain::GetExclusion(int x, int y)
	{
		return Handle->GetExclusion(x, y);
	}
	
	RCSplat^ RCTerrain::GetColorChunk(NGUINet::NVector2^ position, int x, int y)
	{
		T1TexCol Col = Handle->GetColorChunk(NGUINet::NVectorConverter::ToVector2(position), x, y);

		RCSplat^ Out = gcnew RCSplat();
		Out->Set(Col.S0, Col.S1, Col.S2, Col.S3);

		return Out;
	}

	void RCTerrain::SetColorChunk(NGUINet::NVector2^ position, int x, int y, RCSplat^ color)
	{
		T1TexCol Col;
		Col.Set(color->S0, color->S1, color->S2, color->S3);

		Handle->SetColorChunk(NGUINet::NVectorConverter::ToVector2(position), x, y, Col);
	}

	NGUINet::NVector3^ RCTerrain::GetPosition()
	{
		return NGUINet::NVectorConverter::FromVector3(Handle->GetPosition());
	}

	void RCTerrain::SetPosition(NGUINet::NVector3^ position)
	{
		Handle->SetPosition(NGUINet::NVectorConverter::ToVector3(position));
	}

	void RCTerrain::Tag::set(System::Object^ value)
	{
		_Tag = value;
	}

	System::Object^ RCTerrain::Tag::get()
	{
		return _Tag;
	}

	void RCTerrain::Raise(float radius, float hardness, NGUINet::NVector2^ position, bool circular, float change)
	{
		Transform_Raise(Handle, radius, hardness, NGUINet::NVectorConverter::ToVector2(position), circular, change);
	}

	void RCTerrain::Smooth(float radius, float hardness, NGUINet::NVector2^ position, bool circular, float change)
	{
		Transform_Smooth(Handle, radius, hardness, NGUINet::NVectorConverter::ToVector2(position), circular, change);
	}

	void RCTerrain::SetHeight(float radius, float hardness, NGUINet::NVector2^ position, bool circular, float height)
	{
		Transform_SetHeight(Handle, radius, hardness, NGUINet::NVectorConverter::ToVector2(position), circular, height);
	}

	void RCTerrain::Paint(float radius, float hardness, NGUINet::NVector2^ position, bool circular, float strength, int texture, float min, float max, float minHeight, float maxHeight)
	{
		Transform_Paint(Handle, radius, hardness, NGUINet::NVectorConverter::ToVector2(position), circular, strength, texture, min, max, minHeight, maxHeight);
	}

	void RCTerrain::PaintHole(float radius, NGUINet::NVector2^ position, bool circular, bool erase)
	{
		Transform_PaintHole(Handle, radius, NGUINet::NVectorConverter::ToVector2(position), circular, erase);
	}

	void RCTerrain::PaintGrass(float radius, NGUINet::NVector2^ position, bool circular, System::Byte grassMask, bool additive)
	{
		Transform_PaintGrass(Handle, radius, NGUINet::NVectorConverter::ToVector2(position), circular, (unsigned char)grassMask, additive);
	}

	void RCTerrain::Ramp(NGUINet::NVector2^ startPosition, NGUINet::NVector2^ endPosition, float startWidth, float endWidth)
	{
		Transform_Ramp(Handle, NGUINet::NVectorConverter::ToVector2(startPosition),
			NGUINet::NVectorConverter::ToVector2(endPosition),
			startWidth, endWidth);
	}

}
}