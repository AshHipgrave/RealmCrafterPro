#pragma once

#include <Vector2.h>
#include <Vector3.h>
#include <Color.h>
#include "NVector2.h"
#include "NVector3.h"

using namespace System;

namespace NGUINet
{
	class NVectorConverter
	{
	public:
		static NGin::Math::Vector3 ToVector3(NGUINet::NVector3^ In);
		static NGUINet::NVector3^ FromVector3(NGin::Math::Vector3 In);

		static NGin::Math::Vector2 ToVector2(NGUINet::NVector2^ In);
		static NGUINet::NVector2^ FromVector2(NGin::Math::Vector2 In);

		static NGin::Math::Color ToColor(System::Drawing::Color^ In);
		static System::Drawing::Color^ FromColor(NGin::Math::Color In);
	};
}
