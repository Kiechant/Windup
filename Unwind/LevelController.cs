using OpenTK;
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
		private const float minSpawnDelay = 1.0f;
		private const float maxSpawnDelay = 2.0f;

		// TODO: LevelParameters

		uint unwindCounter;
		uint scoreCounter;
		uint playTime;
		uint maxTime;

		public override void Start()
		{
			base.Start();

			// Debug draw a normal triangle shape
			var builder = new ShapeBuilder();
			builder.Open(new Vector2(0, 1));
			builder.AddLine(new Vector2(-0.5f, 0.5f));
			builder.AddLine(new Vector2(0.5f, 0.5f));
			builder.Close();
			triangle = builder.Build();
		}

		public override void OnUpdateFrame(object source, EventArgs e)
		{
			base.OnUpdateFrame(source, e);

			if (timeSinceSpawn >= timeNextSpawn)
			{
				Random random = new Random();

				float angPos = ((float)random.NextDouble() * (AngleSteps - 1)) * AngleResolution;
				float angSize = (float)random.NextDouble() * (Paddle.MaxSize - Paddle.MinSize) + Paddle.MinSize;
				uint steps = (uint)(angSize / AngleResolution);
				      
				obstacles.Add(new Paddle(angPos, angSize, steps));
				Console.WriteLine("added obstacle");

				timeSinceSpawn = 0;
				timeNextSpawn = (float)random.NextDouble() * maxSpawnDelay + minSpawnDelay;
			}

			else
			{
				timeSinceSpawn += Time.deltaTimeSeconds;
			}

			foreach (var obstacle in obstacles) obstacle.Update();

			triangle.Update();
		}

		// TODO: Remove function
		public Vector2[] GetVertices()
		{
			List<Vector2> vertices = new List<Vector2>();
			foreach (Obstacle o in obstacles)
			{
				//vertices.AddRange(o.GetVertices());
			}
			return vertices.ToArray();
		}

		public void Draw()
		{
			foreach (Obstacle o in obstacles)
			{
				o.Draw();
			}

			triangle.Draw();
		}
	}
}
