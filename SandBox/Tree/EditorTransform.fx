//======================================================================================//
// Filename: .fx                                                                        //
//                                                                                      //
// Author:   Jared Belkus (jared.belkus@solstargames.com)                               //
//                                                                                      //
// Description:                                                                         //
//                                                                                      //
//======================================================================================//
//   (C) 2009                                                                           //
//======================================================================================//

// Transform constants
float4x4 ViewProjection : ViewProjection;
float4x4 World : World;
float4x4 WorldViewProjection;

float3 CameraPosition : CameraPosition;

// Vertex Shader Input
struct VSInput
{
	float3 Position : POSITION;
	float4 Color : COLOR0;
};

// Vertex Shader Output
struct VSOut
{
	float4 Position : POSITION;
	float4 Color : COLOR0;
};

// Vertex entrypoint
VSOut VS_Main(VSInput In)
{
	// Setup return value
	VSOut Out;
	
	Out.Color = In.Color;
	
	float4x4 TWorld = World;
	
	float FOV = 1.5708f;
	float Scale = 2.0f * sin(FOV * 0.5f) * length(TWorld[3].xyz - CameraPosition);
	Scale *= 0.05f;
	
	TWorld[0][0] *= Scale;
	TWorld[1][1] *= Scale;
	TWorld[2][2] *= Scale;
	

	// Transform
	float4 WorldPosition = mul(float4(In.Position, 1.0f), TWorld);
	Out.Position = mul(WorldPosition, ViewProjection);

	// Return output
	return Out;
}

// Pixel Entrypoint
half4 PS_Main(VSOut In) : COLOR0
{
	// Setup return value
	half4 Out;
	
	Out = In.Color;
	
	// Return
	return Out;
}

technique ProfileHigh
{
    pass p0 
    {		
		VertexShader = compile vs_2_0 VS_Main();
		PixelShader  = compile ps_2_0 PS_Main();
		zenable = false;
		zwriteenable = false;
		alphablendenable = true;
		srcblend = srcalpha;
		destblend = invsrcalpha;
		cullmode = none;
    }
}
