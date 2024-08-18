//======================================================================================//
// Filename: WireFrame.fx                                                               //
//                                                                                      //
// Author:   Jared Belkus (jared.belkus@solstargames.com)                               //
//                                                                                      //
// Description: Renders a cheap writeframe box.                                         //
//                                                                                      //
//======================================================================================//
//   (C) 2008 Solstar Games.                                                            //
//======================================================================================//

// Transform constants
float4x4 WorldViewProjection : WorldViewProjection;

texture Texture0 : TextureStage0;
sampler Tex0 = sampler_state
{
	texture = <Texture0>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
};

// Vertex Declarations to import
struct VSStandard
{
    float4 Position : POSITION;
};

// Vertex Shader Output
struct VSOut
{
	float4 Position	: POSITION;
};

// Vertex entrypoint
VSOut vs_main(VSStandard In)
{
	// Setup return value
	VSOut Out;

	// Transform
	Out.Position = mul(float4(In.Position.xyz, 1.0f), WorldViewProjection);

	// Return output
	return Out;
}

// Pixel Entrypoint
float4 ps_main(void) : COLOR0
{
	// Get texture colours
	return tex2D(Tex0, float2(0, 0));
}

technique Particle
{
    pass p0 
    {		
		VertexShader = compile vs_2_0 vs_main();
		PixelShader  = compile ps_2_0 ps_main();
		fillmode = wireframe;		
    }
}