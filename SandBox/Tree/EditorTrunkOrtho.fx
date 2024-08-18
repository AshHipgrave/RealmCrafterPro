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
	half2 TexCoord       : TEXCOORD0;
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
VSOut VS_Main(VSInput In)
{
	// Setup return value
	VSOut Out;

	// Copy input data to output
	Out.TexCoord = In.TexCoord;
	
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
	
	float4x4 NewWorld = World;
	float4x4 WorldRot = {-0.000004, 0.000000, -1.000000, 0.000000,
		0.000000, 1.000000, 0.000000, 0.000000,
		1.000000, 0.000000, -0.000004, 0.000000,
		0.000000, 0.000000, 0.000000, 1.000000};
	
	if(Angle > 0.5f)
	{
		NewWorld = mul(WorldRot, World);
	}

	// Transform
	float4 WorldPosition = mul(float4(In.Position, 1.0f), NewWorld);
	Out.Position = mul(WorldPosition, mul(View, Projection));
	
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

	//Normal *= ShowNormal;
	//Diffuse *= (1.0f - ShowNormal);
	
	Out = half4(Diffuse, 1.0f);
	//Out = half4(Diffuse, 1.0f);

	// Return
	return Out;
}

technique ProfileHigh
{
    pass p0 
    {		
		VertexShader = compile vs_2_0 VS_Main();
		PixelShader  = compile ps_2_0 PS_Main();
		zenable = true;
		zwriteenable = true;
    }
}
