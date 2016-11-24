using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Unwind
{
	/* Defines a polygon by a sequential set of vertices,
	 and triangles for mesh calculations. */
	public class Shape : IDisposable
	{
		public Vector2[] vertices;
		public int[] triangles;
		public PrimitiveType type;
		public Vector4 colour;

		public int vertexBuffer { get; private set; }
		private int positionAttrib;
		private int indexBuffer;
		private int indexAttrib;
		private static int nextAttribPtrHandle = 0;

		public Shape()
		{
			vertices = new Vector2[0];
			triangles = new int[0];
			Setup();
		}

		public Shape(Vector2[] vertices, int[] triangles)
		{
			this.vertices = vertices;
			this.triangles = triangles;
			Setup();
		}

		/* Function to be called by constructors only.
		 Generates vbo for shape and calls Update. */
		private void Setup()
		{
			vertexBuffer = GL.GenBuffer();
			Update();
		}

		public void Update()
		{
			// Generates vertex data with interleaved position and colour attributes

			int n = triangles.Length;
			int a = 8;
			int count = a * n;
			float[] vertexData = new float[count];

			for (int i = 0; i < n; i++)
			{
				// Position data
				Vector2 vertex = vertices[triangles[i]];
				vertexData[a * i] = vertex.X;
				vertexData[a * i + 1] = vertex.Y;
				vertexData[a * i + 2] = 0.5f;
				vertexData[a * i + 3] = 1.0f;

				// Colour data
				vertexData[a * i + 4] = colour.X;
				vertexData[a * i + 5] = colour.Y;
				vertexData[a * i + 6] = colour.Z;
				vertexData[a * i + 7] = colour.W;
			}

			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, count * sizeof(float), vertexData, BufferUsageHint.DynamicDraw);
		}

		public void Draw(Shader shader)
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

			int stride = 8 * sizeof(float);

			GL.VertexAttribPointer(shader.attributes.position, 4, VertexAttribPointerType.Float, false, stride, 0);
			GL.EnableVertexAttribArray(shader.attributes.position);

			GL.VertexAttribPointer(shader.attributes.colourIn, 4, VertexAttribPointerType.Float, false, stride, 4 * sizeof(float));
			GL.EnableVertexAttribArray(shader.attributes.colourIn);

			GL.DrawArrays(type, 0, triangles.Length);

			GL.DisableVertexAttribArray(shader.attributes.position);
			GL.DisableVertexAttribArray(shader.attributes.colourIn);
		}

		public void Dispose()
		{
			GL.DeleteBuffer(vertexBuffer);
		}

		public void Print()
		{
			foreach (Vector2 vertex in vertices)
			{
				Console.WriteLine(vertex);
			}
		}
	}
}