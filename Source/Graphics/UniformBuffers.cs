using System.Runtime.InteropServices;

namespace Celeste64.UniformBuffers;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct DefaultVertex
{
	public Matrix MVP;
	public Matrix Model;
	public Matrix ModelInverse;
	public float JointsMult;
}

[System.Runtime.CompilerServices.InlineArray(SkinnedModel.SkinMatrixCount)]
public struct DefaultJoints { private Matrix _element0; }

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct DefaultFragment
{
	public Vec4 Color;
	public Vec4 Sun;
	public Vec4 SilhouetteColor;
	public Vec4 VerticalFogColor;
	public float Near;
	public float Far;
	public float Effects;
	public float Silhouette;
	public float Time;
	public float Cutout;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct EdgeVertex
{
	public Matrix Matrix;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EdgeFragment
{
	public Vec4 Edge;
	public Vec2 Pixel;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct SpriteVertex(in Matrix matrix)
{
	public Matrix Matrix = matrix;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SpriteFragment(in Camera camera)
{
	public float Near = camera.NearPlane;
	public float Far = camera.FarPlane;
}

