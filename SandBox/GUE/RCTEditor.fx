//======================================================================================//
// Filename: RCTEditor.fx                                                               //
//                                                                                      //
// Author:   Jared Belkus (jared.belkus@solstargames.com)                               //
//                                                                                      //
// Description: Used for selection circle/square in RCT Editor.                         //
//                                                                                      //
//======================================================================================//
//   (C) 2009 Solstar Games.                                                            //
//======================================================================================//



// Constants
float4x4 WorldViewProjection;
float3 Offset;
float3 Offset2;
float ChunkInterp;

// Editor data
float Radius;
float Hardness;
float2 BrushPosition;
bool Circular;

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
	float4 Pos : TEXCOORD0;
};

// Vertex entrypoint
VS_OUTPUT vs_main(VS_INPUT In)
{
	// Setup return value
	VS_OUTPUT Out;

	// Get position of this vertex (and transform from chunk->terrain)
	float4 Position = float4(In.Position.x * 2.0f, lerp(In.Position.y, In.Position.w, ChunkInterp), In.Position.z * 2.0f, 1.0f);
	Position.xyz += Offset + Offset2;
	Out.Pos = Position;
	
	// Transform vertex to screen
	Out.Position = mul(Position, WorldViewProjection);

	// Return output
	return Out;
}


// Pixel Entrypoint
float4 ps_main(VS_OUTPUT In) : COLOR0
{
	// Default color is nothing
	float4 Color = float4(0, 0, 0, 0);
	float4 Position = In.Pos;

	// Selection is a circle
	if(Circular)
	{
		float XDist = BrushPosition.x  - Position.x;
		float ZDist = BrushPosition.y - Position.z;
		float RadSQ = pow(Radius, 2);
		float HardSQ = RadSQ * Hardness;
		
		float DistSQ = pow(XDist, 2) + pow(ZDist, 2);
		
		if(DistSQ < RadSQ)
			if(DistSQ > HardSQ)
				Color = float4(0, 0, 1, 1);
			else
				Color = float4(1, 0, 0, 1);
	}else // Selection is a square
	{
		float MinX = BrushPosition.x - Radius;
		float MinZ = BrushPosition.y - Radius;
		float MaxX = BrushPosition.x + Radius;
		float MaxZ = BrushPosition.y + Radius;
		
		float IMinX = BrushPosition.x - Radius * Hardness;
		float IMinZ = BrushPosition.y - Radius * Hardness;
		float IMaxX = BrushPosition.x + Radius * Hardness;
		float IMaxZ = BrushPosition.y + Radius * Hardness;
		
		if(Position.x > MinX && Position.x < MaxX && Position.z > MinZ && Position.z < MaxZ)
			if(Position.x > IMinX && Position.x < IMaxX && Position.z > IMinZ && Position.z < IMaxZ)
				Color = float4(1, 0, 0, 1);
			else
				Color = float4(0, 0, 1, 1);
	}

	Color.w *= 0.3f;	
	return Color;
}

technique RCTEditor
{
    pass p0 
    {		
		VertexShader = compile vs_2_0 vs_main();
		PixelShader  = compile ps_2_0 ps_main();
		alphablendenable = true;
		srcblend = srcalpha;
		destblend = invsrcalpha;
		zenable = true;
		zwriteenable = true;
    }
}
