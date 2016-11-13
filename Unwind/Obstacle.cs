using System;
using OpenTK;

namespace Unwind
{
	public abstract class Obstacle
	{
		public const float SpawnRadius = 1.0f;
		public const float BaseFallRate = 0.5f;

		private static uint nextID;
		public uint ID { get; private set; }

		protected float radius;
		protected float angle;
		public float fallRate;

		protected Shape shape = new Shape();

		public Obstacle()
		{
			this.ID = nextID++;
			this.radius = SpawnRadius;
			this.fallRate = BaseFallRate;
		}

		public virtual void Update()
		{
			shape.Update();
		}

		public void Draw()
		{
			shape.Draw();

			// DEBUG
			Console.WriteLine("Obstacle " + ID + " drawn.");
		}
	}
}
