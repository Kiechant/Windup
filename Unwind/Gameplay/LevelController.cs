using System;
using System.Collections.Generic;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	public class LevelController : Controller
	{
		/* AngleSteps and AngleResolution the granularity of angular positions.
		 AngleSteps should be evenly divisible by 4 and 6. */
		public const uint AngleSteps = 120;
		public const float AngleResolution = (MathHelper.TwoPi) / AngleSteps;

		GameRing ring;
		private List<Obstacle> obstacles = new List<Obstacle>();

		private float timeSinceSpawn;
		private float timeNextSpawn;
		private const float minSpawnDelay = 0.5f;
		private const float maxSpawnDelay = 2.0f;

		// TODO: LevelParameters

		//uint unwindCounter;
		//uint scoreCounter;
		//uint playTime;
		//uint maxTime;

		public override void Start(Game game)
		{
			base.Start(game);

			ring = new GameRing();
		}

		public override void OnUpdate(object source, EventArgs e)
		{
			base.OnUpdate(source, e);

			ring.Update(mouseDown);

			if (timeSinceSpawn >= timeNextSpawn)
			{
				Random random = new Random();
				SpawnPaddle(random);
				SpawnRaindrop(random);

				timeSinceSpawn = 0;
				timeNextSpawn = (float)random.NextDouble() * maxSpawnDelay + minSpawnDelay;
			}

			else timeSinceSpawn += Time.deltaTimeSeconds;

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
			var game = source as Game;
			var effectsShader = game.effectsShader;

			// Renders obstacles on top of backdrop with frosty-glass-like blur effect.
			blurShader.Bind();

			ring.Draw(blurShader);

			foreach (Obstacle o in obstacles)
				o.Draw(blurShader);
			
			game.basicShader.Bind();
		}

		private void SpawnPaddle(Random random)
		{
			float angPos = ((float)random.NextDouble() * (AngleSteps - 1)) * AngleResolution;
			float angSize = (float)random.NextDouble() * (Paddle.MaxSize - Paddle.MinSize) + Paddle.MinSize;
			uint steps = (uint)(angSize / AngleResolution);

			Obstacle paddle = new Paddle(angPos, angSize, steps);
			paddle.colour = new Vector4(Mathc.ColourToVector3(Color.Black), 0.7f);
			obstacles.Add(paddle);
		}

		private void SpawnRaindrop(Random random)
		{
			float angPos = ((float)random.NextDouble() * (AngleSteps - 1)) * AngleResolution;

			Obstacle raindrop = new Raindrop(angPos, 20);
			raindrop.colour = new Vector4(Mathc.ColourToVector3(Color.Black), 0.7f);
			obstacles.Add(raindrop);
		}

		public override void Dispose()
		{
			base.Dispose();

			ring.Dispose();

			foreach (var o in obstacles)
				o.Dispose();
		}
	}
}
