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

float Time : Time;
float Selected = 0.0f;

float ShowNormal = 0.0f;

// Vertex Shader Input
struct VSInput
{
	float3 Position : POSITION;
	half2 TexCoord : TEXCOORD0;
};

// Vertex Shader Output
struct VSOut
{
	float4 Position : POSITION;
	half2 TexCoord       : TEXCOORD0;
	half4 Color : COLOR0;
};

texture Texture0 : TextureStage0;
sampler Tex0 = sampler_state
{
    texture = <Texture0>;
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture Texture1 : TextureStage1;
sampler Tex1 = sampler_state
{
    texture = <Texture1>;
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};


// Vertex entrypoint
VSOut VS_Main(uniform half4 from, uniform half4 to, uniform float notLine, VSInput In)
{
	// Setup return value
	VSOut Out;

	// Copy input data to output
	Out.TexCoord = In.TexCoord;
	
	if(Selected > 0.0f)
	{
		Out.Color = saturate(lerp(from, to, sin(Time * 0.005f)));
	}else
	{
		Out.Color = to * notLine;
	}

	// Transform
	float4 WorldPosition = mul(float4(In.Position, 1.0f), World);
	Out.Position = mul(WorldPosition, ViewProjection);

	// Return output
	return Out;
}

// Pixel Entrypoint
half4 PS_Main(VSOut In) : COLOR0
{
	// Setup return value
	half4 Out;
	
	// Get pixel normal
	half3 Normal = tex2D(Tex1, In.TexCoord).xyz;

	// Get pixel color
	half3 Diffuse = tex2D(Tex0, In.TexCoord).xyz;

	Normal *= ShowNormal;
	//Diffuse *= (1.0f - ShowNormal);
	
	Out = half4((Normal + Diffuse) * In.Color, In.Color.w);
	//Out = half4(Diffuse, 1.0f);

	// Return
	return Out;
}

technique ProfileHigh
{
    pass p0 
    {		
		VertexShader = compile vs_2_0 VS_Main(half4(1, 0.0, 0.0, 0.4), half4(1, 1, 1, 0.4), 1.0f);
		PixelShader  = compile ps_2_0 PS_Main();
		zenable = true;
		zwriteenable = false;
		alphablendenable = true;
		srcblend = srcalpha;
		destblend = invsrcalpha;

    }
	pass p1
    {		
		VertexShader = compile vs_2_0 VS_Main(half4(1, 1, 0, 1), half4(1, 1, 0, 1), 0.0f);
		PixelShader  = compile ps_2_0 PS_Main();
		zenable = true;
		zwriteenable = false;
		alphablendenable = false;
		srcblend = srcalpha;
		destblend = invsrcalpha;
		fillmode = wireframe;

    }
}
