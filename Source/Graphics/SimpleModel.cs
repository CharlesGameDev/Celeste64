using SharpGLTF.Transforms;
using System.Runtime.InteropServices;

namespace Celeste64;

public class SimpleModel : Model
{
	public record struct Part(int MaterialIndex, int IndexStart, int IndexCount);
	public readonly Mesh<Vertex> Mesh = new(Game.Instance.GraphicsDevice);
	public readonly List<Part> Parts = [];
	public CullMode CullMode = CullMode.Front;

	public SimpleModel() {}

	public SimpleModel(List<SimpleModel> combine)
	{
		throw new NotImplementedException();
	}
	
	public SimpleModel(List<SkinnedModel> combine)
	{
		var vertices = new List<Vertex>();
		var indices = new List<int>();
		
		foreach (var it in combine)
		{
			for (int i = 0; i < it.Instance.Count; i++)
			{
				var drawable = it.Instance[i];

				if (drawable.Transform is not RigidTransform statXform)
					continue;

				var meshPart = it.Template.Parts[drawable.Template.LogicalMeshIndex];
				var meshMatrix = statXform.WorldMatrix * it.Transform;
				var meshVertices = it.Template.Vertices;
				var meshIndices = it.Template.Indices;
				var vertexOffset = vertices.Count;
				var indexOffset = indices.Count;

				for (int n = 0; n < meshVertices.Count; n++)
					vertices.Add(meshVertices[n].Transform(meshMatrix));

				for (int n = 0; n < meshIndices.Count; n++)
					indices.Add(vertexOffset + meshIndices[n]);

				foreach (var primitive in meshPart)
				{
					var mat = it.Materials[primitive.Material];
					var matIndex = Materials.IndexOf(mat);
					if (matIndex < 0)
					{
						matIndex = Materials.Count;
						Materials.Add(mat);
					}

					var next = new Part()
					{
						MaterialIndex = matIndex,
						IndexStart = primitive.Index + indexOffset,
						IndexCount = primitive.Count
					};

					if (Parts.Count > 0)
					{
						var last = Parts[^1];
						if (last.MaterialIndex == next.MaterialIndex)
						{
							var end = last.IndexStart + last.IndexCount;
							if (end == next.IndexStart)
							{
								last.IndexCount += next.IndexCount;
								Parts[^1] = last;
								continue;
							}
						}
					}

					Parts.Add(next);
				}
			}
		}

		Mesh.SetVertices(CollectionsMarshal.AsSpan(vertices));
		Mesh.SetIndices(CollectionsMarshal.AsSpan(indices));
		Transform = Matrix.Identity;
		Flags = ModelFlags.Terrain;
	}

	public override void Render(ref RenderState state)
	{
		foreach (var mat in Materials)
		{
			state.ApplyToMaterial(mat, Matrix.Identity);
			mat.VertexUniforms = mat.VertexUniforms with { JointsMult = 0 };
		}

		foreach (var segment in Parts)
		{
			if (segment.IndexCount <= 0 || segment.MaterialIndex < 0)
				continue;

			state.GraphicsDevice.Draw(new DrawCommand(state.Camera.Target, Mesh, Materials[segment.MaterialIndex])
			{
				DepthCompare = state.DepthCompare,
				DepthTestEnabled = true,
				DepthWriteEnabled = state.DepthMask,
				CullMode = CullMode,
				IndexOffset = segment.IndexStart,
				IndexCount = segment.IndexCount
			});
			state.Calls++;
			state.Triangles += segment.IndexCount / 3;
		}
	}
}
