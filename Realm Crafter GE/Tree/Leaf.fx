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
float4x4 ViewInverse : ViewInverse;
float4x4 World : World;
float4x4 LightView : LightView;
float4x4 LightProjection : LightProjection;
float BillboardScale : BillboardScale;

float3 CameraPosition : CameraPosition;

float Time : Time;
float2 SwayDirection : SwayDirection;
float SwayAmount : SwayAmount;
float3 SwayCenter : SwayCenter;

#include "..\Include\BBDXInclude.fxh"
ImportFog
ImportLights

// Vertex Shader Input
struct VSInput
{
	float3 Position : POSITION;
	half2 TexCoord : TEXCOORD0;
};

// Vertex Shader Output
struct VSOut_High
{
	float4 Position : POSITION;
	half2 TexCoord : TEXCOORD0;
	half4 Fog : TEXCOORD1;
	half3 Normal : TEXCOORD2;
	half3 BiNormal : TEXCOORD3;
	half3 Tangent : TEXCOORD4;
	half4 Lights : COLOR0;
};

struct VSOut
{
	float4 Position : POSITION;
	half2 TexCoord : TEXCOORD0;
	half4 Fog : TEXCOORD1;
	half4 Lights : COLOR0;
};

struct VSOut_Depth
{
	float4 Position : POSITION;
	half2 TexCoord : TEXCOORD0;
	float2 Depths : TEXCOORD1;
};

texture Texture0 : DiffuseMap;
sampler DiffuseTex = sampler_state
{
    texture = <Texture0>;
    MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
};

sampler MaskTex = sampler_state
{
    texture = <Texture0>;
    MIPFILTER = Point;
    MINFILTER = Point;
    MAGFILTER = Point;
};

texture Texture1 : NormalMap;
sampler NormalTex = sampler_state
{
    texture = <Texture1>;
    MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
};

float4 DiffuseDirectionalLightTree(float3 VertexNormal, float3 LightColor, float3 LightDirection)
{
	float DiffuseAmount1 = saturate(dot(-LightDirection, VertexNormal));
	//float DiffuseAmount2 = saturate(dot(-LightDirection, -VertexNormal) * 1.3f);

	return float4( (LightColor * DiffuseAmount1) + (LightColor * 0.3f), 1.0f);
}



// Vertex entrypoint
VSOut_High VS_Main_High(VSInput In)
{
	// Setup return value
	VSOut_High Out;
	
	// Copy
	Out.TexCoord = In.TexCoord;
	Out.TexCoord.y = 1.0f - Out.TexCoord.y;

	// Transform
	float4 WorldPosition = mul(float4(In.Position, 1.0f), World);
	float3 WorldBase = mul(float4(SwayCenter.xyz, 1.0f), World);
	float3 Influence = (WorldPosition.xyz - WorldBase) / 30.0f;
	Influence.y = max(Influence.y, 0.0f);
	float TimeSine = sin(Time * 0.001f) * SwayAmount;
	WorldPosition += float4(
		((Influence.z * TimeSine) + (Influence.y * TimeSine)) * SwayDirection.x,
		0,
		((Influence.x * TimeSine) + (Influence.y * TimeSine)) * SwayDirection.y,
		0);
	Out.Position = mul(WorldPosition, View);
	
	
	float3 Normal = normalize(CameraPosition.xyz - WorldPosition.xyz);
	float3 Tangent = cross(Normal, float3(1, 0, 0));
	float3 BiNormal = cross(Normal, float3(0, 1, 0));
	Out.Normal = Normal;
	Out.BiNormal = BiNormal;
	Out.Tangent = Tangent;
	
	Out.Position.xy += (In.TexCoord - float2(0.5f, 0.5f)) * (BillboardScale * float2(length(World[0].xyz), length(World[1].xyz)));
	
	Out.Position = mul(Out.Position, Projection);
	
	// Lighting
	Out.Lights = AmbientLight;
	Out.Lights = saturate(Out.Lights);
	
	// Fog
	Out.Fog = VertexFog(FogData, FogColor, Out.Position.w);


	// Return output
	return Out;
}

// Pixel Entrypoint
half4 PS_Main_High(VSOut_High In) : COLOR0
{
	half4 Diffuse = tex2D(DiffuseTex, In.TexCoord);
	half4 Mask = tex2D(MaskTex, In.TexCoord);
	half3 Normal = tex2D(NormalTex, In.TexCoord).xyz * 2 - 1;
	
	// Create tangent matrix
	half3 TNormal = normalize(In.Normal);
	half3 Tangent = normalize(In.Tangent);
	half3 BiNormal = normalize(In.BiNormal);
	float3x3 WorldToTangent = {Tangent, BiNormal, TNormal};
	Normal = normalize(mul(Normal, WorldToTangent));
	
	half4 Lights = In.Lights;
	Lights.xyz += saturate(DirectionalColor[0] * half3(0.5f, 0.5f, 0.5f));
	Lights.xyz += saturate(DirectionalColor[1] * half3(0.5f, 0.5f, 0.5f));
	Lights.xyz += saturate(DirectionalColor[2] * half3(0.5f, 0.5f, 0.5f));
	//Lights += DiffuseDirectionalLightTree(Normal, DirectionalColor[0], DirectionalNormal[0]);
	//Lights += DiffuseDirectionalLightTree(Normal, DirectionalColor[1], DirectionalNormal[1]);
	//Lights += DiffuseDirectionalLightTree(Normal, DirectionalColor[2], DirectionalNormal[2]);
	Lights = saturate(Lights);
	
	
	// Generate final output including fog
	half4 Out = Diffuse * Lights;
	Out = lerp(Out, float4(In.Fog.rgb, 1.0f), In.Fog.w);
	//Out.xyz = (Normal + 1) * 0.5f;
	Out.w = Mask.w; 
	
	return Out;
}

// Vertex entrypoint
VSOut VS_Main_Low(VSInput In)
{
	// Setup return value
	VSOut Out;
	
	// Copy
	Out.TexCoord = In.TexCoord;
	Out.TexCoord.y = 1.0f - Out.TexCoord.y;

	// Transform
	float4 WorldPosition = mul(float4(In.Position, 1.0f), World);
	Out.Position = mul(WorldPosition, View);
	
	float3 Normal = normalize(CameraPosition.xyz - WorldPosition.xyz);
	
	Out.Position.xy += (In.TexCoord - float2(0.5f, 0.5f)) * (BillboardScale * float2(length(World[0].xyz), length(World[1].xyz)));
	
	Out.Position = mul(Out.Position, Projection);
	
	// Lighting
	Out.Lights = AmbientLight;
	Out.Lights.xyz += saturate(DirectionalColor[0] * half3(0.5f, 0.5f, 0.5f));
	Out.Lights.xyz += saturate(DirectionalColor[1] * half3(0.5f, 0.5f, 0.5f));
	Out.Lights.xyz += saturate(DirectionalColor[2] * half3(0.5f, 0.5f, 0.5f));
	Out.Lights = saturate(Out.Lights);
	
	// Fog
	Out.Fog = VertexFog(FogData, FogColor, Out.Position.w);


	// Return output
	return Out;
}

half4 PS_Main_Low(VSOut In) : COLOR0
{
	half4 Diffuse = tex2D(DiffuseTex, In.TexCoord);
	half4 Mask = tex2D(MaskTex, In.TexCoord);

	// Generate final output including fog
	half4 Out = Diffuse * In.Lights;
	Out = lerp(Out, float4(In.Fog.rgb, 1.0f), In.Fog.w);
	Out.w = Mask.w; 
	
	return Out;
}

// Vertex entrypoint
VSOut_Depth VS_Main_Depth(VSInput In)
{
	// Setup return value
	VSOut_Depth Out;
	
	// Copy
	Out.TexCoord = In.TexCoord;
	Out.TexCoord.y = 1.0f - Out.TexCoord.y;

	// Transform
	float4 WorldPosition = mul(float4(In.Position, 1.0f), World);
	Out.Position = mul(WorldPosition, LightView);
	
	Out.Position.xy += (In.TexCoord - float2(0.5f, 0.5f)) * (BillboardScale * float2(length(World[0].xyz), length(World[1].xyz)));
	
	Out.Position = mul(Out.Position, LightProjection);
	
	Out.Depths = Out.Position.zw;

	// Return output
	return Out;
}

// Pixel Entrypoint
float4 PS_Main_Depth(VSOut_Depth In) : COLOR0
{
	half4 Mask = tex2D(MaskTex, In.TexCoord);
	
	float Depth = In.Depths.x / In.Depths.y;
    float4 Out = float4(Depth, Depth * Depth, 0, 1);
	Out.w = Mask.w; 
	
	return Out;
}

technique ProfileLow
{
    pass p0 
    {		
		VertexShader = compile vs_2_0 VS_Main_Low();
		PixelShader  = compile ps_2_0 PS_Main_Low();
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


technique ProfileDepth
{
    pass p0 
    {		
		VertexShader = compile vs_2_0 VS_Main_Depth();
		PixelShader  = compile ps_2_0 PS_Main_Depth();
		alphablendenable = false;
		alphatestenable = true;
		alpharef = 40;
		alphafunc = greater;
		zenable = true;
		zwriteenable = true;
    }
}
