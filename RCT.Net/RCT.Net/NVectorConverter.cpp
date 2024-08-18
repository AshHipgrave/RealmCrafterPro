#include "stdafx.h"

#include "NVectorConverter.h"

namespace NGUINet
{
	NGin::Math::Vector3 NVectorConverter::ToVector3(NGUINet::NVector3^ In)
	{
		return NGin::Math::Vector3(In->X, In->Y, In->Z);
	}

	NGUINet::NVector3^ NVectorConverter::FromVector3(NGin::Math::Vector3 In)
	{
		return gcnew NGUINet::NVector3(In.X, In.Y, In.Z);
	}

	NGin::Math::Vector2 NVectorConverter::ToVector2(NGUINet::NVector2^ In)
	{
		return NGin::Math::Vector2(In->X, In->Y);
	}

	NGUINet::NVector2^ NVectorConverter::FromVector2(NGin::Math::Vector2 In)
	{
		return gcnew NGUINet::NVector2(In.X, In.Y);
	}

	NGin::Math::Color NVectorConverter::ToColor(System::Drawing::Color^ In)
	{
		NGin::Math::Color cColor;
		cColor.R = Convert::ToSingle(In->R) / 255.0f;
		cColor.G = Convert::ToSingle(In->G) / 255.0f;
		cColor.B = Convert::ToSingle(In->B) / 255.0f;
		cColor.A = Convert::ToSingle(In->A) / 255.0f;

		return cColor;
	}

	System::Drawing::Color^ NVectorConverter::FromColor(NGin::Math::Color In)
	{
		return System::Drawing::Color::FromArgb(In.ToARGB());
	}
}