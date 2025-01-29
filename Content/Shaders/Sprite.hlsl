#include "Partials/Methods.hlsl"

// Vertex Shader:

cbuffer VertexUniforms : register(b0, space1)
{
	float4x4 u_matrix;
};

struct VertexInput
{
	float3 position : TEXCOORD0;
	float2 tex : TEXCOORD1;
	float4 color : TEXCOORD2;
};

struct VertexOutput
{
	float4 position : SV_Position;
	float2 tex : TEXCOORD0;
	float4 color : TEXCOORD1;
};

VertexOutput vertex_main(VertexInput input)
{
	VertexOutput output;
	output.position = mul(u_matrix, float4(input.position, 1.0));
	output.tex = input.tex;
    output.color = input.color;
	return output;
}

// Fragment Shader:

cbuffer FragmentUniforms : register(b0, space3)
{
	float   u_near;
	float   u_far;
};

Texture2D<float4> Texture : register(t0, space2);
SamplerState Sampler : register(s0, space2);

struct FragmentOutput
{
    float4 color : SV_Target0;
    float depth : SV_Depth;
};

FragmentOutput fragment_main(VertexOutput input)
{
	FragmentOutput output;
	output.depth = LinearizeDepth(input.position.z, u_near, u_far);
	output.color = Texture.Sample(Sampler, input.tex) * input.color;
	return output;
}