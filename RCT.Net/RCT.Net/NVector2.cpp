#include "stdafx.h"

#include "NVector2.h"

namespace NGUINet
{
	NVector2::NVector2()
	{
		_X = 0.0f;
		_Y = 0.0f;
	}

	NVector2::NVector2(float x, float y)
	{
		_X = x;
		_Y = y;
	}

	float NVector2::X::get()
	{
		return _X;
	}

	void NVector2::X::set(float value)
	{
		_X = value;
	}

	float NVector2::Y::get()
	{
		return _Y;
	}

	void NVector2::Y::set(float value)
	{
		_Y = value;
	}
}