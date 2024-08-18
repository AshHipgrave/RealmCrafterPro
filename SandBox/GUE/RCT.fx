//======================================================================================//
// Filename: RCT.fx                                                                     //
//                                                                                      //
// Author:   Jared Belkus (jared.belkus@solstargames.com)                               //
//                                                                                      //
// Description: Draws RCT meshes. Performs height interpolation between LOD levels.     //
//              Splats texture maps based upon an input splat map. Excludes pixels from //
//              an exclusion (hole) map using alpharef.                                 //
//                                                                                      //
//======================================================================================//
//   (C) 2009 Solstar Games.                                                            //
//======================================================================================//

// Transform constants
//float4x4 ViewProjection : ViewProjection;
float4x4 World : World;
float4x4 WorldViewProjection;
float4x4 LightProjection : LightProjection;

#include "..\Include\BBDXInclude.fxh"
float4 FogColor;// = float3(1.0f, 0.0f, 0.0f);
float2 FogData;// = float2(100, 500);

float3 Offset;
float3 Offset2;
float CoordScales[5];
float ChunkInterp;
float3 AmbientLight;
float3 DirectionalNormal[3];
float3 DirectionalColor[3];
float3 PointLightColor[5] = {float3(1, 0, 0), float3(0, 0, 0), float3(0, 0, 0), float3(0, 0, 0), float3(0, 0, 0) };
float4 PointLightPosition[5] = {float4(16, 10, 16, 20), float4(0, 0, 0, 0), float4(0, 0, 0, 0), float4(0, 0, 0, 0), float4(0, 0, 0, 0) };
float3 CollisionColor = {1, 1, 1};

// Vertex Shader Input
struct VS_INPUT
{
	float4 Position : POSITION;
	float3 Normal : TEXCOORD0;
    //R/float4 Co1 : TEXCOORD0;
    //R/float2 YInterp : TEXCOORD1;
};

struct T1VertexLOD
{
	float3 Position : POSITION;
	float3 Normal : NORMAL;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR;
};

// Vertex Shader Output
struct VS_OUTPUT
{
    float4 Position : POSITION;
    float3 Normal : TEXCOORD0;
    float4 TexCoordDepths : TEXCOORD1;
    float2 SplatCoord : TEXCOORD2;
    float4 Lights : COLOR0;
    float4 Light0Dir : TEXCOORD3;
    float4 Light1Dir : TEXCOORD4;
    float4 Fog : TEXCOORD6;
    float4 LightProj : TEXCOORD7;
    float2 Depth : TEXCOORD5; // the y value is just to avoid the npc bug on the shader compiler (should be removed later on)
};

struct VSOut_Low
{
    float4 Position : POSITION;
	float4 Lights : COLOR0;
    float2 SplatCoord : TEXCOORD0;
	float2 TexCoord : TEXCOORD1;
    float4 Fog : TEXCOORD2;
};

struct VSOut_LOD
{
	float4 Position : POSITION;
	float4 Lights : COLOR0;
	float2 TexCoord : TEXCOORD0;
	float4 Splat : TEXCOORD1;
	float4 Fog : TEXCOORD2;
};

// Vertex entrypoint
VS_OUTPUT vs_main(VS_INPUT In)
{
    // Setup return value
    VS_OUTPUT Out;

    // Generate a position from the local X/Z. Interpolate between LOD Y values
    float4 Position = float4(In.Position.x * 2.0f, lerp(In.Position.y, In.Position.w, ChunkInterp), In.Position.z * 2.0f, 1.0f);
    
    float3 Normal = In.Normal;

    // Dividing the local position by 64 will find the position on the splat texture
    Out.SplatCoord = Position.xz / (64.0f * 2.0f);
    
    // Move vertex into terrains object space
    Position.xyz += Offset + Offset2;
    
    // Get a world position
    float4 WorldPos = mul(Position, World);
    
    // Calculate lighting
    float4 Lights = float4(AmbientLight, 1.0f);
    Lights += DiffuseDirectionalLight(Normal, DirectionalColor[1], DirectionalNormal[1]);
    Lights += DiffuseDirectionalLight(Normal, DirectionalColor[2], DirectionalNormal[2]);
    Lights += DiffusePointLight(WorldPos.xyz, Normal, PointLightPosition[2], PointLightColor[2]);
    Lights += DiffusePointLight(WorldPos.xyz, Normal, PointLightPosition[3], PointLightColor[3]);
    Lights = saturate(Lights);
    Out.Lights = Lights;

    
    Out.Light0Dir = float4(PointLightPosition[0].xyz - WorldPos.xyz, PointLightPosition[0].w);
    Out.Light1Dir = float4(PointLightPosition[1].xyz - WorldPos.xyz, PointLightPosition[1].w);

    // Transform
    Out.Position = mul(Position, WorldViewProjection);
    Out.Depth.x = Out.Position.z;
    Out.LightProj = mul(Position, LightProjection);
    Out.TexCoordDepths.zw = Out.LightProj.zw;
    Out.Normal = normalize(Normal);
    
    // Divide position by a scale to get a custom value
    Out.TexCoordDepths.xy = Position.xz;

    // Fog
    Out.Fog = float4(FogColor.xyz, saturate((Out.Position.w - FogData[0]) / (FogData[1] - FogData[0])));

    Out.Depth.y = DirectionalNormal[0].y;

    // Return output
    return Out;
}

VSOut_Low vs_main_low(VS_INPUT In)
{
    // Setup return value
    VSOut_Low Out;

    // Generate a position from the local X/Z. Interpolate between LOD Y values
    float4 Position = float4(In.Position.x * 2.0f, lerp(In.Position.y, In.Position.w, ChunkInterp), In.Position.z * 2.0f, 1.0f);
    
    float3 Normal = In.Normal;

    // Dividing the local position by 64 will find the position on the splat texture
    Out.SplatCoord = Position.xz / (64.0f * 2.0f);
    
    // Move vertex into terrains object space
    Position.xyz += Offset + Offset2;
    
    // Get a world position
    float4 WorldPos = mul(Position, World);
    
    // Calculate lighting
    float4 Lights = float4(AmbientLight, 1.0f);
	Lights += DiffuseDirectionalLight(Normal, DirectionalColor[0], DirectionalNormal[0]);
    Lights += DiffuseDirectionalLight(Normal, DirectionalColor[1], DirectionalNormal[1]);
    Lights += DiffuseDirectionalLight(Normal, DirectionalColor[2], DirectionalNormal[2]);
    Lights = saturate(Lights);
    Out.Lights = Lights;

    // Transform
    Out.Position = mul(Position, WorldViewProjection);
	
	// Divide position by a scale to get a custom value
    Out.TexCoord.xy = Position.xz;
    
    // Fog
    Out.Fog = float4(FogColor.xyz, saturate((Out.Position.w - FogData[0]) / (FogData[1] - FogData[0])));

    // Return output
    return Out;
}

VSOut_LOD vs_main_lod(T1VertexLOD In)
{
    // Setup return value
    VSOut_LOD Out;
	
	Out.Splat = In.Color;
	Out.TexCoord = In.TexCoord;

    // Generate a position from the local X/Z. Interpolate between LOD Y values
    float4 Position = float4(In.Position.x * 2.0f, In.Position.y, In.Position.z * 2.0f, 1.0f);
    
    // Move vertex into terrains object space
    Position.xyz += Offset2;
    
    // Calculate lighting
    float4 Lights = float4(AmbientLight, 1.0f);
	Lights += DiffuseDirectionalLight(In.Normal, DirectionalColor[0], DirectionalNormal[0]);
    Lights += DiffuseDirectionalLight(In.Normal, DirectionalColor[1], DirectionalNormal[1]);
    Lights += DiffuseDirectionalLight(In.Normal, DirectionalColor[2], DirectionalNormal[2]);
    Lights = saturate(Lights);
    Out.Lights = Lights;

    // Transform
    Out.Position = mul(Position, WorldViewProjection);
    
    // Fog
    Out.Fog = float4(FogColor.xyz, saturate((Out.Position.w - FogData[0]) / (FogData[1] - FogData[0])));

    // Return output
    return Out;
}

texture Texture0 : TextureStage0;
sampler Tex0 = sampler_state
{
    texture = <Texture0>;
    MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
};

texture Texture1 : TextureStage1;
sampler Tex1 = sampler_state
{
    texture = <Texture1>;
    MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
};

texture Texture2 : TextureStage2;
sampler Tex2 = sampler_state
{
    texture = <Texture2>;
    MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
};

texture Texture3 : TextureStage3;
sampler Tex3 = sampler_state
{
    texture = <Texture3>;
    MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
};

texture Texture4 : TextureStage4;
sampler Tex4 = sampler_state
{
    texture = <Texture4>;
    MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
};

texture Splat;
sampler Splats = sampler_state
{
    texture = <Splat>;
    MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    ADDRESSU = CLAMP;
    ADDRESSV = CLAMP;
};

texture Holes;
sampler HoleMap = sampler_state
{
    texture = <Holes>;
    //MIPFILTER = LINEAR;
    //MINFILTER = LINEAR;
    //MAGFILTER = LINEAR;
    MIPFILTER = NONE;
    MINFILTER = NONE;
    MAGFILTER = NONE;
    ADDRESSU = CLAMP;
    ADDRESSV = CLAMP;
};

texture ShadowMapTex : ShadowMap;
sampler2D ShadowMap = sampler_state
{
    texture = <ShadowMapTex>;
    AddressU = Border;
    AddressV = Border;
       BorderColor = 0xFFFFFFFF;
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
};

// Pixel Entrypoint
float4 ps_main(VS_OUTPUT In/*, out float4 RT1 : COLOR1*/) : COLOR0
{
    // Lookup the splat texture
    float4 Splat = tex2D(Splats, In.SplatCoord);

    
    // Get the fourth index texture
    float T4 = 1.0f - (Splat.x + Splat.y + Splat.z + Splat.w);

    // Lookup all textures
    float4 OutColor = (tex2D(Tex0, In.TexCoordDepths.xy * CoordScales[0]) * Splat.x)
        + (tex2D(Tex1, In.TexCoordDepths.xy * CoordScales[1]) * Splat.y)
        + (tex2D(Tex2, In.TexCoordDepths.xy * CoordScales[2]) * Splat.z)
        + (tex2D(Tex3, In.TexCoordDepths.xy * CoordScales[3]) * Splat.w)
        + (tex2D(Tex4, In.TexCoordDepths.xy * CoordScales[4]) * T4);
        
    float2 ShadowCoord = 0.5f * In.LightProj.xy / In.LightProj.w + float2(0.5f, 0.5f);
    ShadowCoord.y = 1.0f - ShadowCoord.y;
    float Depth = (In.TexCoordDepths.z / In.TexCoordDepths.w);
    float2 TopCoord = abs(ShadowCoord);
    float Apply = 1.0f;


    
    if(max(TopCoord.x, TopCoord.y) <= 1.0f)
    {
        float2 Moments = tex2D(ShadowMap, ShadowCoord).xy;

        float Dist2Light = Depth;

        float lit_factor = (Dist2Light <= Moments.x);
  
        float E_x2     = Moments.y;
        float Ex_2     = Moments.x*Moments.x;
        //float Variance = min(max(E_x2 - Ex_2, 0.00000001), 1.0);
        float Variance = min(max(E_x2 - Ex_2, 0.00001), 1.0);
        float m_d      = Moments.x-Dist2Light;
        float p_max    = Variance / (Variance + m_d*m_d);
        p_max = saturate( (p_max-0.3)/0.7 );
    
        lit_factor = max(lit_factor, p_max);
        Apply = lit_factor;

        // Fade out on the boundaries of the shadow map
        ShadowCoord = saturate(ShadowCoord);
        ShadowCoord = min( ShadowCoord/0.1, (1-ShadowCoord)/0.1);
        float t = min( ShadowCoord.x, ShadowCoord.y );
              t = min( t, abs(In.Depth.y/0.1)); // fade out when nearly horizontal (sunrise/sunset)
        Apply = lerp(1, Apply, saturate(t));
    }
    else Apply = 1;

    // Calculate per pixel lights
    float4 Lights = saturate(In.Lights);

    Lights += DiffuseDirectionalLight(In.Normal, DirectionalColor[0], DirectionalNormal[0]) * saturate(Apply + 0.5f);
    Lights += DiffusePointLightPs(In.Normal, In.Light0Dir.xyz, PointLightColor[0], In.Light0Dir.w);
    Lights += DiffusePointLightPs(In.Normal, In.Light1Dir.xyz, PointLightColor[1], In.Light1Dir.w);
    Lights = saturate(Lights);
    Lights.w = 1.0f;
    
    // Multiply lighting and diffuse
    return float4(lerp(OutColor.xyz * Lights.xyz, In.Fog.xyz, In.Fog.w) * CollisionColor, tex2D(HoleMap, In.SplatCoord).w);
}

float4 ps_main_low(VSOut_Low In) : COLOR0
{
    // Lookup the splat texture
    float4 Splat = tex2D(Splats, In.SplatCoord);
    
    // Get the fourth index texture
    float T4 = 1.0f - (Splat.x + Splat.y + Splat.z + Splat.w);

    // Lookup all textures
    float4 OutColor = (tex2D(Tex0, In.TexCoord.xy * CoordScales[0]) * Splat.x)
        + (tex2D(Tex1, In.TexCoord.xy * CoordScales[1]) * Splat.y)
        + (tex2D(Tex2, In.TexCoord.xy * CoordScales[2]) * Splat.z)
        + (tex2D(Tex3, In.TexCoord.xy * CoordScales[3]) * Splat.w)
        + (tex2D(Tex4, In.TexCoord.xy * CoordScales[4]) * T4);
        
    // Calculate per pixel lights
    float4 Lights = In.Lights;
    
    // Multiply lighting and diffuse
    return float4(lerp(OutColor.xyz * Lights.xyz, In.Fog.xyz, In.Fog.w) * CollisionColor, tex2D(HoleMap, In.SplatCoord).w);
}

float4 ps_main_lod(VSOut_LOD In) : COLOR0
{
    // Get the fourth index texture
    float T4 = .0f - (In.Splat.x + In.Splat.y + In.Splat.z + In.Splat.w);
	
    // Lookup all textures
    float4 OutColor = (tex2D(Tex0, In.TexCoord * CoordScales[0]) * In.Splat.x)
        + (tex2D(Tex1, In.TexCoord * CoordScales[1]) * In.Splat.y)
        + (tex2D(Tex2, In.TexCoord * CoordScales[2]) * In.Splat.z)
        + (tex2D(Tex3, In.TexCoord * CoordScales[3]) * In.Splat.w)
        + (tex2D(Tex4, In.TexCoord * CoordScales[4]) * T4);
		
    // Calculate per pixel lights
    float4 Lights = In.Lights;
    
    // Multiply lighting and diffuse
    return float4(lerp(OutColor.xyz * Lights.xyz, In.Fog.xyz, In.Fog.w), 1.0f);
}

technique RCT
{
    pass p0 
    {        
        VertexShader = compile vs_3_0 vs_main();
        PixelShader  = compile ps_3_0 ps_main();
        zenable = true;
        zwriteenable = true;
        alphatestenable = true;
        alphafunc = greaterequal;
        alpharef = 0xff;
        //fillmode = wireframe;
    }
}


technique RCTLow
{
    pass p0 
    {        
        VertexShader = compile vs_2_0 vs_main_low();
        PixelShader  = compile ps_2_0 ps_main_low();
        zenable = true;
        zwriteenable = true;
        alphatestenable = true;
        alphafunc = greaterequal;
        alpharef = 0xff;
        //fillmode = wireframe;
    }
}

technique RCTLOD
{
	pass p0
    {        
        VertexShader = compile vs_2_0 vs_main_lod();
        PixelShader  = compile ps_2_0 ps_main_lod();
        zenable = true;
        zwriteenable = true;
        alphatestenable = true;
        alphafunc = greaterequal;
        alpharef = 0xff;
        //fillmode = wireframe;
    }
}

