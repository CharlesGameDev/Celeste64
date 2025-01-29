float LinearizeDepth(float depth, float near, float far) 
{
    float z = depth * 2.0 - 1.0; // back to NDC 
    return ((2.0 * near * far) / (far + near - z * (far - near))) / far;	
}

float Map(float value, float min1, float max1, float min2, float max2)
{
	float perc = clamp((value - min1) / (max1 - min1), 0.0, 1.0);
	return perc * (max2 - min2) + min2;
}

float3 TransformNormal(float3 normal, float4x4 transform)
{
	return normalize(mul(transpose((float3x3)transform), normal));
}