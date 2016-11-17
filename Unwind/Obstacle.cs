using System;
using OpenTK;

namespace Unwind
{
	public abstract class Obstacle : IDisposable
	{
		public const float SpawnRadius = 1.0f;
		public const float BaseFallRate = 0.5f;

		private static uint nextID;
		public uint ID { get; private set; }

		protected float radius;
		protected float angle;
		public float fallRate;

		protected Shape shape = new Shape();

		protected Obstacle()
		{
			this.ID = nextID++;
			this.radius = SpawnRadius;
			this.fallRate = BaseFallRate;
		}

		public abstract void Update(out bool disposed);

		public void Draw()
		{
			shape.Draw();

			// DEBUG
			Console.WriteLine("Obstacle " + ID + " drawn.");
		}

		public void Dispose()
		{
			shape.Dispose();
			Console.WriteLine("Dispose obstacle " + ID);
			return;
		}
	}
}
