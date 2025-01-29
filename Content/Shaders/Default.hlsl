#include "Partials/Methods.hlsl"

// Vertex Shader:

#define IDENTITY_MATRIX float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)

cbuffer VertexUniforms : register(b0, space1)
{
	float4x4 u_mvp;
	float4x4 u_model;
	float4x4 u_model_inverse;
	float    u_jointMult;
};

cbuffer JointUniforms : register(b1, space1)
{
	float4x4 u_jointMat[32];
};

struct VertexInput
{
	float3 position : TEXCOORD0;
	float2 tex      : TEXCOORD1;
	float3 color    : TEXCOORD2;
	float3 normal   : TEXCOORD3;
	float4 joint    : TEXCOORD4;
	float4 weight   : TEXCOORD5;
};

struct VertexOutput
{
	float2 tex      : TEXCOORD0;
	float3 color    : TEXCOORD1;
	float3 normal   : TEXCOORD2;
	float3 world    : TEXCOORD3;
	float4 position : SV_Position;
};

VertexOutput vertex_main(VertexInput input)
{
    float4x4 skinMat =
        input.weight.x * u_jointMat[int(input.joint.x)] +
        input.weight.y * u_jointMat[int(input.joint.y)] +
        input.weight.z * u_jointMat[int(input.joint.z)] +
        input.weight.w * u_jointMat[int(input.joint.w)];

    float4x4 skinRes = 
		IDENTITY_MATRIX * (1.0 - u_jointMult) + 
		skinMat * u_jointMult;

	VertexOutput output;
	output.tex = input.tex;
    output.color = input.color;
	output.normal = TransformNormal(input.normal, u_model_inverse);
	output.world = (float3)mul(u_model, float4(input.position, 1.0));
	output.position = mul(mul(u_mvp, skinRes), float4(input.position, 1.0));
	return output;
}

// Fragment Shader:

cbuffer FragmentUniforms : register(b0, space3)
{
	float4  u_color;
	float4  u_sun;
	float4  u_silhouette_color;
	float4  u_vertical_fog_color;
	float   u_near;
	float   u_far;
	float   u_effects;
	float   u_silhouette;
	float   u_time;
	float   u_cutout;
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
	// get texture color
	float4 src = Texture.Sample(Sampler, input.tex) * u_color;

	// only enable if you want ModelFlags.Cutout types to work, didn't end up using
	//if (src.a < u_cutout)
	//	discard;

	float depth = LinearizeDepth(input.position.z, u_near, u_far);
	float fall = Map(input.world.z, 50, 0, 0, 1);
	float fade = Map(depth, 0.9, 1, 1, 0);
	float3  col = src.rgb;

	// lighten texture color based on normal
	float lighten = max(0.0, -dot(input.normal, (float3)u_sun));
	col = lerp(col, float3(1,1,1), lighten * 0.10 * u_effects);

	// shadow
	float darken = max(0.0, dot(input.normal, (float3)u_sun));
	col = lerp(col, float3(4/255.0, 27/255.0, 44/255.0), darken * 0.80 * u_effects);

	// passthrough mode
	col = lerp(col, u_silhouette_color.rgb, u_silhouette);

	// fade bottom to white
	col = lerp(col, u_vertical_fog_color.rgb * src.a, fall);

	FragmentOutput output;
	output.depth = depth;
	output.color = float4(col, src.a) * fade;
	return output;
}