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
float4x4 Projection : Projection;
float4x4 View : View;
float4x4 World : World;
float4x4 ViewInverse : ViewInverse;

#include "..\Include\BBDXInclude.fxh"
ImportFog
ImportLights

// Vertex Shader Input
struct VSInput
{
	float3 Position : POSITION;
	half2 TexCoord : TEXCOORD0;
	half2 BillScale : TEXCOORD1;
};

// Vertex Shader Output
struct VSOut
{
	float4 Position : POSITION;
	half4 Lights : COLOR0;
	half2 TexCoord : TEXCOORD0;
	half4 Fog : TEXCOORD1;
};



// Vertex entrypoint
VSOut VS_Main_High(VSInput In)
{
	// Setup return value
	VSOut Out;
	
	// Copy
	Out.TexCoord = In.TexCoord;
	Out.TexCoord.y = 1.0f - Out.TexCoord.y;

	// Transform
	float4 WorldPosition = mul(float4(In.Position, 1), World);
	Out.Position = mul(WorldPosition, View);
	
	Out.Position.xy += (In.BillScale * float2(World[0][0], World[1][1]));
	
	Out.Position = mul(Out.Position, Projection);
	
	
	// Lighting
	float3 Normal = float3(0, 1, 0);//normalize(WorldPosition.xyz - (ViewInverse[3].xyz));
	Out.Lights = AmbientLight;
	Out.Lights += DiffuseDirectionalLight(Normal, DirectionalColor[0], DirectionalNormal[0]);
	Out.Lights += DiffuseDirectionalLight(Normal, DirectionalColor[1], DirectionalNormal[1]);
	Out.Lights += DiffuseDirectionalLight(Normal, DirectionalColor[2], DirectionalNormal[2]);
	Out.Lights = saturate(Out.Lights);
	
	// Fog
	Out.Fog = VertexFog(FogData, FogColor, Out.Position.w);

	// Return output
	return Out;
}

texture Texture0 : DiffuseMap;
sampler DiffuseTex = sampler_state
{
    texture = <Texture0>;
    MIPFILTER = Linear;
    MINFILTER = Linear;
    MAGFILTER = Linear;
};

sampler MaskTex = sampler_state
{
    texture = <Texture0>;
    MIPFILTER = Point;
    MINFILTER = Point;
    MAGFILTER = Point;
};

// Pixel Entrypoint
half4 PS_Main_High(VSOut In) : COLOR0
{
	half4 Diffuse = tex2D(DiffuseTex, In.TexCoord);
	
	// Generate final output including fog
	half4 Out = Diffuse * In.Lights;
	Out = lerp(Out, float4(In.Fog.rgb, 1.0f), In.Fog.w);
	Out.w = tex2D(MaskTex, In.TexCoord).w; 
	
	return Out;
}

technique ProfileLow
{
    pass p0 
    {		
		VertexShader = compile vs_2_0 VS_Main_High();
		PixelShader  = compile ps_2_0 PS_Main_High();
		alphablendenable = false;
		alphatestenable = true;
		alpharef = 40;
		alphafunc = greater;
		zenable = true;
		zwriteenable = true;
    }
}

technique ProfileHigh
{
    pass p0 
    {		
		VertexShader = compile vs_2_0 VS_Main_High();
		PixelShader  = compile ps_2_0 PS_Main_High();
		alphablendenable = false;
		alphatestenable = true;
		alpharef = 40;
		alphafunc = greater;
		zenable = true;
		zwriteenable = true;
    }
}
