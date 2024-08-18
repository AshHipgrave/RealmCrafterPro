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
float BillboardScale : BillboardScale = 1.0f;
float Time : Time;
float Selected = 0.0f;

// Vertex Shader Input
struct VSInput
{
	float3 Position : POSITION;
	half2 TexCoord : TEXCOORD0;
	half4 Color : COLOR0;
};

// Vertex Shader Output
struct VSOut
{
	float4 Position : POSITION;
	half2 TexCoord : TEXCOORD0;
	half4 Color : COLOR0;
};

texture Texture0 : TextureStage0;
sampler DiffuseTex = sampler_state
{
    texture = <Texture0>;
    MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
};

texture Texture1 : TextureStage1;
sampler MaskTex = sampler_state
{
    texture = <Texture0>;
    MIPFILTER = Point;
    MINFILTER = Point;
    MAGFILTER = Point;
};


// Vertex entrypoint
VSOut VS_Main(VSInput In)
{
	// Setup return value
	VSOut Out;
	
	// Copy
	Out.TexCoord = In.TexCoord;
	Out.TexCoord.y = 1.0f - Out.TexCoord.y;
	
	if(Selected > 0.0f)
		Out.Color = saturate(lerp(half4(1, 0.4, 0.4, 1), half4(1, 1, 1, 1), sin(Time * 0.005f)));
	else
		Out.Color = half4(1, 1, 1, 1);

	// Transform
	float4 WorldPosition = mul(float4(In.Position, 1.0f), World);
	Out.Position = mul(WorldPosition, View);
	Out.Position.xy += (In.TexCoord - float2(0.5f, 0.5f)) * (BillboardScale * float2(World[0][0], World[1][1]));
	
	Out.Position = mul(Out.Position, Projection);
	
	// Return output
	return Out;
}

// Pixel Entrypoint
half4 PS_Main(VSOut In) : COLOR0
{
	half4 Diffuse = tex2D(DiffuseTex, In.TexCoord);
	half4 Mask = tex2D(MaskTex, In.TexCoord);
	
	return half4(Diffuse.xyz * In.Color.xyz, Mask.w);
}

technique ProfileHigh
{
    pass p0 
    {		
		VertexShader = compile vs_2_0 VS_Main();
		PixelShader  = compile ps_2_0 PS_Main();
		alphablendenable = false;
		alphatestenable = true;
		alpharef = 40;
		alphafunc = greater;
		zenable = true;
		zwriteenable = true;
		cullmode = none;
    }
}
