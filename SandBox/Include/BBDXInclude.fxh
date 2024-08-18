//======================================================================================//
// Filename: BBDXInclude.fxh                                                            //
//                                                                                      //
// Author:   Frank Puig Placeres                                                        //
//           Jared Belkus (jared.belkus@solstargames.com)                               //
//                                                                                      //
// Description: Include for for BBDX2 Shaders. Handles default textures, lighting and   //
//              vertex declarations.                                                    //
//                                                                                      //
//======================================================================================//
//   (C) 2008 Solstar Games.                                                            //
//======================================================================================//

#define MAXPOINTLIGHTS 5
#define MAXDIRECTIONALLIGHTS 3
#define MAXBONES 50

#ifdef _Anisotropy
#define _FilterType Anisotropic
#else
#define _FilterType Linear
#define _MaxAnisotropy 
#endif

#define ImportLights \
float3 PointLightColor[MAXPOINTLIGHTS] : PointColor; \
float4 PointLightPosition[MAXPOINTLIGHTS] : PointPosition; \
float3 DirectionalNormal[MAXDIRECTIONALLIGHTS] : DirectionalNormal; \
float3 DirectionalColor[MAXDIRECTIONALLIGHTS] : DirectionalColor; \
float4 AmbientLight : LightAmbient;

#define ImportFog \
float4 FogColor : FogColor; \
float2 FogData : FogData;

#define ImportBones \
float4x4 Bones[MAXBONES] : Bones;

// Vertex Declarations to import
struct VSStandard
{
    float4 Position : POSITION;
    float3 Normal   : NORMAL;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

struct VS2TCoords
{
    float4 Position : POSITION;
    float3 Normal   : NORMAL;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
    float2 TexCoord1 : TEXCOORD1;
};

struct VSTangents
{
    float4 Position : POSITION;
    float3 Normal   : NORMAL;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
    float3 Tangent  : TANGENT;
    float3 BiNormal : BINORMAL;
	float2 TexCoord2 : TEXCOORD1;
};

struct VSAnimated
{
    float4 Position : POSITION;
	float4 Weights  : BLENDWEIGHT;
    float4 Indices  : BLENDINDICES;
    float3 Normal   : NORMAL;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
	float2 TexCoord2 : TEXCOORD0;
	float3 Tangent : TANGENT;
	float3 BiNormal : BINORMAL;
};

// Texture Definitions
#define ImportTexture0\
texture Texture0 : TextureStage0;\
sampler Tex0 = sampler_state\
{\
	texture = <Texture0>;\
	MipFilter = Linear;\
	MinFilter = _FilterType;\
	MagFilter = Linear;\
	_MaxAnisotropy\
};

#define ImportTexture1\
texture Texture1 : TextureStage1;\
sampler Tex1 = sampler_state\
{\
	texture = <Texture1>;\
	MipFilter = Linear;\
	MinFilter = _FilterType;\
	MagFilter = Linear;\
	_MaxAnisotropy\
};

#define ImportTexture2\
texture Texture2 : TextureStage2;\
sampler Tex2 = sampler_state\
{\
	texture = <Texture2>;\
	MipFilter = Linear;\
	MinFilter = _FilterType;\
	MagFilter = Linear;\
	_MaxAnisotropy\
};

#define ImportTexture3\
texture Texture3 : TextureStage3;\
sampler Tex3 = sampler_state\
{\
	texture = <Texture3>;\
	MipFilter = Linear;\
	MinFilter = _FilterType;\
	MagFilter = Linear;\
	_MaxAnisotropy\
};

#define ImportShadows\
float4x4 LightProjection : LightProjection;\
texture ShadowMapTex : ShadowMap;\
sampler2D ShadowMap = sampler_state\
{\
	texture = <ShadowMapTex>;\
	AddressU = Border;\
	AddressV = Border;\
	BorderColor = 0xFFFFFFFF;\
	MipFilter = Linear;\
	MinFilter = Linear;\
	MagFilter = Linear;\
};\
float GetShadow(float4 lightProjection)\
{\
	float2 ShadowCoord = 0.5f * lightProjection.xy / lightProjection.w + float2(0.5f, 0.5f);\
    ShadowCoord.y = 1.0f - ShadowCoord.y;\
    float2 TopCoord = abs(ShadowCoord);\
    float Apply = 1.0f;\
    \
    if(max(TopCoord.x, TopCoord.y) <= 1.0f)\
    {\
        float2 Moments = tex2D(ShadowMap, ShadowCoord).xy;\
\
        float Dist2Light = lightProjection.z / lightProjection.w;\
\
        float lit_factor = (Dist2Light <= Moments.x);\
  \
        float E_x2     = Moments.y;\
        float Ex_2     = Moments.x*Moments.x;\
        float Variance = min(max(E_x2 - Ex_2, 0.0001), 1.0);\
        float m_d      = Moments.x-Dist2Light;\
        float p_max    = Variance / (Variance + m_d*m_d);\
        p_max = saturate( (p_max-0.3)/0.7 );\
    \
        lit_factor = max(lit_factor, p_max);\
        Apply = lit_factor;\
\
        ShadowCoord = saturate(ShadowCoord);\
        ShadowCoord = min( ShadowCoord/0.1, (1-ShadowCoord)/0.1);\
        float t = min( ShadowCoord.x, ShadowCoord.y );\
        t = min( t, abs(DirectionalNormal[0].y/0.1));\
        Apply = lerp(1, Apply, saturate(t));\
    }\
    else Apply = 1.0f;\
	\
	return Apply;\
}

/*
{\
	float2 ShadowCoord = 0.5f * lightProjection.xy / lightProjection.w + float2(0.5f, 0.5f);\
	ShadowCoord.y = 1.0f - ShadowCoord.y;\
	float2 TopCoord = abs(ShadowCoord);\
	\
	float Apply = 1.0f;\
	\
	if(max(TopCoord.x, TopCoord.y) <= 1.0f)\
	{\
		float2 Moments = tex2D(ShadowMap, ShadowCoord).xy;\
		\
		float Dist2Light = (lightProjection.z / lightProjection.w);\
		\
		float lit_factor = (Dist2Light <= Moments.x);\
		\
		float E_x2     = Moments.y;\
		float Ex_2     = Moments.x*Moments.x;\
		float Variance = min(max(E_x2 - Ex_2, 0.00005), 1.0);\
		float m_d      = Moments.x-Dist2Light;\
		float p_max    = Variance / (Variance + m_d*m_d);\
		p_max = saturate( (p_max-0.3)/0.7 );\
		\
		lit_factor = max(lit_factor, p_max);\
		Apply = 1;/*lit_factor;*/\
/*	}\
	\
	return Apply;\
}*/

#define ImportFrameBuffer\
texture FrameBufferTex : FrameBuffer;\
sampler FrameBuffer = sampler_state\
{\
	texture = <FrameBufferTex>;\
	MipFilter = Linear;\
	MinFilter = Linear;\
	MagFilter = Linear;\
    ADDRESSU  = CLAMP;\
    ADDRESSV  = CLAMP;\
    ADDRESSW  = CLAMP;\
};

#define ImportReflectionMap\
texture ReflectionMapTex : ReflectionMap;\
sampler ReflectionMap = sampler_state\
{\
	texture = <ReflectionMapTex>;\
	MipFilter = Linear;\
	MinFilter = Linear;\
	MagFilter = Linear;\
};


// -------------------- Functions --------------------
// Project the shadowmap onto a regular mesh
#define ProjectShadowMap(position, world, outLightProjection)\
outLightProjection = mul(position, mul(world, LightProjection));



// Calculate a diffuse point light, using a radius
float4 DiffusePointLight(float3 VertexPosition, float3 VertexNormal, float4 LightData, float3 LightColor)
{
	// Distance between light and vertex
	float3 Light = LightData.xyz - VertexPosition;

	// Light Attenuation
	float Attenuation = 1.0f - saturate(length(Light) / LightData.w);
	
	// Diffuse (N.L)
	Light = normalize(Light);
	float DiffuseAmount = saturate(dot(VertexNormal, Light));
	
	// Return
	return float4((DiffuseAmount * Attenuation * LightColor), 1.0f);
}

// Calculate a diffuse point light, using a radius but using a pretermined light normal
float4 DiffusePointLightPs(float3 Normal, float3 Light, float3 LightColor, float Radius)
{
	// Light Attenuation
	float Attenuation = 1.0f - saturate(length(Light) / Radius);
	
	// Diffuse (N.L)
	Light = normalize(Light);
	float DiffuseAmount = saturate(dot(Normal, Light));
	
	// Return
	return float4((DiffuseAmount * Attenuation * LightColor), 1.0f);
}

// Calculate a diffuse point light, using a radius adding a specular value
float4 DiffusePointLightSpecular(float3 VertexPosition, float3 VertexNormal, float4 LightData, float3 LightColor, float3 ViewDirection, float SpecularPower)
{
	
	// Distance between light and vertex
	float3 Light = LightData.xyz - VertexPosition;

	// Light Attenuation
	float Attenuation = 1.0f - saturate(length(Light) / LightData.w);
	
	// Half Vector
	float3 HalfVect = normalize(Light + ViewDirection);
	
	// Diffuse (N.L)
	Light = normalize(Light);
	float DiffuseAmount = saturate(dot(VertexNormal, Light));
	
	float Specular = pow(saturate(dot(VertexNormal, HalfVect)), SpecularPower);
	
	// Return
	return saturate(float4((DiffuseAmount * Attenuation * LightColor + Specular), 1.0f));
}

// Calculate a diffuse directional light
float4 DiffuseDirectionalLight(float3 VertexNormal, float3 LightColor, float3 LightDirection)
{
	float DiffuseAmount = saturate(dot(-LightDirection, VertexNormal));

	return float4(LightColor * DiffuseAmount, 1.0f);
}

// Calculate a diffuse + specular directional light
float4 DiffuseSpecularDirectionalLight(float3 VertexNormal, float3 LightColor, float3 LightDirection, float3 ViewDirection, float SpecularPower)
{
	// Diffuse lighting
	float DiffuseAmount = saturate(dot(-LightDirection, VertexNormal));
	
	// Half Vector
	float3 HalfVect = normalize(-LightDirection + ViewDirection);
	
	// Specular
	float Specular = pow(saturate(dot(VertexNormal, HalfVect)), SpecularPower);

	return float4(LightColor * DiffuseAmount + Specular, 1.0f);
}

// Return a packet float4 containing the vertex fog data
float4 VertexFog(float2 FogData, float4 FogColor, float Depth)
{
	return float4(FogColor.r, FogColor.g, FogColor.b, saturate((Depth - FogData[0]) / (FogData[1] - FogData[0])));
}
