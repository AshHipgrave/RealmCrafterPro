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
float4x4 World : World;
float BillboardScale : BillboardScale = 1.0f;

float3 BoxMin = {-2.1f, 0.0f, -3.8f};
float3 BoxMax = {2.1f, 12.0f, 3.9f};

float Angle = 0;

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
	half2 TexCoord : TEXCOORD0;
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
	
	float Width = BoxMax.x - BoxMin.x;
	float Height = BoxMax.y - BoxMin.y;
	
	if(Height > Width)
		Width = Height;
		
	float4x4 Projection = 0;
	Projection[0][0] = (1.0f / Width) * 2.0f;
	Projection[1][1] = Projection[0][0];
	
	Projection[2][2] = 0.001000;
	Projection[2][3] = -0.000010;
	Projection[3][3] = 1.0f;
	
	float4x4 View = 0;
	View[0][0] = View[1][1] = View[2][2] = View[3][3] = 1.0f;
	
	View[3][0] = - (BoxMin.x + ((BoxMax.x - BoxMin.x) * 0.5f));
	View[3][1] = -(BoxMin.y + ((BoxMax.y - BoxMin.y) * 0.5f));
	View[3][2] = 100.000000;
	
	float4 WorldPosition = 0;
	float4x4 WorldRot = {-0.000004, 0.000000, -1.000000, 0.000000,
		0.000000, 1.000000, 0.000000, 0.000000,
		1.000000, 0.000000, -0.000004, 0.000000,
		0.000000, 0.000000, 0.000000, 1.000000};
	
	if(Angle > 0.5f)
	{
		WorldPosition = mul(float4(World[3].xyz, 1.0f), WorldRot);
	}else
	{
		WorldPosition = World[3].xyzw;
	}
	
	// Transform
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
	
	return half4(Diffuse.xyz, Mask.w);
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
