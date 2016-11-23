using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	public abstract class Obstacle : IDisposable
	{
		public const float SpawnRadius = 2.0f;
		public const float BaseFallRate = 0.5f;

		private static uint nextID;
		public uint ID { get; private set; }

		protected float radius;
		protected float angle;
		public float fallRate;

		public Vector4 colour;

		protected Shape shape = new Shape();

		protected Obstacle()
		{
			this.ID = nextID++;
			this.radius = SpawnRadius;
			this.fallRate = BaseFallRate;
		}

		public abstract void Update(out bool disposed);

		public void Draw(int program)
		{
			int loc = GL.GetAttribLocation(program, "colour");
			GL.VertexAttrib4(loc, ref colour);
			shape.Draw();

			// DEBUG
			Console.WriteLine("Obstacle " + ID + " drawn.");
		}

		public void Dispose()
		{
			shape.Dispose();
		}
	}
}
