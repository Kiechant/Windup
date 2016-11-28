using System;
using System.Drawing;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Unwind
{
	public class Backdrop : IDisposable
	{
		private ZoneName zoneName;
		private float sequenceDuration;
		private float sequenceTime;

		private Shape background;
		private List<Shape> shapes = new List<Shape>();

		public Backdrop(ZoneName zoneName, RectangleF bounds)
		{
			this.zoneName = zoneName;

			Create(bounds);
		}

		public void Create(RectangleF bounds)
		{
			switch (zoneName)
			{
				case ZoneName.Dawn:
					sequenceDuration = 1.3f;

					background = ShapeBuilder.BuildRectangle(bounds);
					background.colour = Mathc.ColourToVector4(Color.Red);
					background.type = PrimitiveType.Quads;
					background.zPosition = 0.7f;
					background.Update();

					Shape temp = new Shape(new float[] { 0.0f, 0.5f, 0.25f, 0.5f, 0.25f, 0.75f, 0.0f, 0.75f });
					temp.colour = Mathc.ColourToVector4(Color.Goldenrod);
					temp.type = PrimitiveType.Quads;
					temp.zPosition = 0.6f;
					temp.Update();
					shapes.Add(temp);

					break;

				case ZoneName.Rainstorm:
					break;

				case ZoneName.Tempest:
					break;

				default:
					Console.WriteLine("Zone name " + zoneName + " not recognised in SetBackground.");
					break;
			}
		}

		public void Update()
		{
			sequenceTime += Time.deltaTimeSeconds;
			sequenceTime = Mathc.Modulo(sequenceTime, sequenceDuration);
			Console.WriteLine("Zone sequence time: " + sequenceTime);

			switch (zoneName)
			{
				case ZoneName.Dawn:
					break;

				case ZoneName.Rainstorm:
					break;

				case ZoneName.Tempest:
					break;

				default:
					Console.WriteLine("Zone name " + zoneName + " not recognised in Update.");
					break;
			}
		}

		public void Draw(ShaderProgram shader)
		{
			foreach (var shape in shapes)
				shape.Draw(shader);
			background.Draw(shader);
		}

		public void Dispose()
		{
			foreach (var shape in shapes)
				shape.Dispose();
			background.Dispose();
		}
	}
}
