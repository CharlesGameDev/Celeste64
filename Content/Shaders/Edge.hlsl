// Vertex Shader

cbuffer VertexUniforms : register(b0, space1)
{
    float4x4 Matrix;
};

struct VsInput
{
    float2 Position : TEXCOORD0;
    float2 TexCoord : TEXCOORD1;
    float4 Color : TEXCOORD2;
    float4 Type : TEXCOORD4;
};

struct VsOutput
{
    float2 TexCoord : TEXCOORD0;
    float4 Color : TEXCOORD1;
    float4 Type : TEXCOORD4;
    float4 Position : SV_Position;
};

VsOutput vertex_main(VsInput input)
{
    VsOutput output;
    output.TexCoord = input.TexCoord;
    output.Color = input.Color;
    output.Type = input.Type;
    output.Position = mul(Matrix, float4(input.Position, 0.0, 1.0));
    return output;
}

// Fragment Shader

cbuffer FragmentUniforms : register(b0, space3)
{
    float4 EdgeColor;
	float2 Pixel;
};

Texture2D Texture : register(t0, space2);
SamplerState TextureSampler : register(s0, space2);

Texture2D Depth : register(t1, space2);
SamplerState DepthSampler : register(s1, space2);

float depth(float2 uv)
{
	return Depth.Sample(DepthSampler, uv).r;
}

float4 fragment_main(VsOutput input) : SV_Target0
{
	// get depth and adjacent depth values
	float it = depth(input.TexCoord);
	float other = 
		depth(input.TexCoord + float2(Pixel.x, 0)) * 0.25 +
		depth(input.TexCoord + float2(-Pixel.x, 0)) * 0.25 +
		depth(input.TexCoord + float2(0, Pixel.y)) * 0.25 +
		depth(input.TexCoord + float2(0, -Pixel.y)) * 0.25;
	
	// more edge the closer to the screen
	float edge = step(0.001, other - it);

	// calculate edge color mixed with default color
	float4 col = Texture.Sample(TextureSampler, input.TexCoord);
	float3 res = lerp(col.rgb, EdgeColor.rgb, edge * 0.70);
	return float4(res, col.a);
}
        