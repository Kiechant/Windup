using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace Unwind
{
	public class LevelController : Controller
	{
		/* AngleSteps and AngleResolution the granularity of angular positions.
		 AngleSteps should be evenly divisible by 4 and 6. */
		public const uint AngleSteps = 120;
		public const float AngleResolution = (MathHelper.TwoPi) / AngleSteps;

		private float gameSpeed = 1.0f;
		private List<Obstacle> obstacles = new List<Obstacle>();

		//Debug
		private Shape triangle;

		private float timeSinceSpawn;
		private float timeNextSpawn;
		private const float minSpawnDelay = 0.5f;
		private const float maxSpawnDelay = 2.0f;

		// TODO: LevelParameters

		uint unwindCounter;
		uint scoreCounter;
		uint playTime;
		uint maxTime;

		public override void Start()
		{
			base.Start();
		}

		public override void OnUpdate(object source, EventArgs e)
		{
			base.OnUpdate(source, e);

			if (timeSinceSpawn >= timeNextSpawn)
			{
				SpawnPaddle();
				SpawnRaindrop();

				timeSinceSpawn = 0;
				Random random = new Random();
				timeNextSpawn = (float)random.NextDouble() * maxSpawnDelay + minSpawnDelay;
			}

			else
			{
				timeSinceSpawn += Time.deltaTimeSeconds;
			}

			for (int i = 0; i < obstacles.Count; i++)
			{
				bool disposed;
				obstacles[i].Update(out disposed);
				if (disposed) obstacles.RemoveAt(i);
			}
		}

		public override void OnRender(object source, EventArgs e)
		{
			base.OnRender(source, e);

			foreach (Obstacle o in obstacles)
			{
				o.Draw();
			}
		}

		private void SpawnPaddle()
		{
			Random random = new Random();

			float angPos = ((float)random.NextDouble() * (AngleSteps - 1)) * AngleResolution;
			float angSize = (float)random.NextDouble() * (Paddle.MaxSize - Paddle.MinSize) + Paddle.MinSize;
			uint steps = (uint)(angSize / AngleResolution);

			obstacles.Add(new Paddle(angPos, angSize, steps));
		}

		private void SpawnRaindrop()
		{
			Random random = new Random();

			float angPos = ((float)random.NextDouble() * (AngleSteps - 1)) * AngleResolution;

			obstacles.Add(new Raindrop(angPos, 20));
		}
	}
}
