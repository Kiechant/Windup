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

		public Vector4 colour
		{
			get { return shape.colour; }
			set { shape.colour = value; }
		}

		protected Shape shape = new Shape();

		protected Obstacle()
		{
			this.ID = nextID++;
			this.radius = SpawnRadius;
			this.fallRate = BaseFallRate;
		}

		public abstract void Update(out bool disposed);

		public void Draw(Shader shader)
		{
			shape.Draw(shader);

			// DEBUG
			Console.WriteLine("Obstacle " + ID + " drawn.");
		}

		public void Dispose()
		{
			shape.Dispose();
		}
	}
}
