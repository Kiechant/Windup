using OpenTK;
using System;

namespace Unwind
{
	/* Custom mathematical constants and functions not provided
	 in System.Math or OpenTK.MathHelper. */
	public static class Mathc
	{
		public const float Epsilon = 1E-5f;

		/* Calculates the distance between vectors p and q. */
		public static double Distance(Vector2 p, Vector2 q)
		{
			double dx = p.X - q.X;
			double dy = p.Y - q.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		/* Calculates the angle from vector p to q. */
		public static float Angle(Vector2 p, Vector2 q)
		{
			p.Normalize();
			q.Normalize();
			return (float)(Math.Atan2(q.X, q.Y) - Math.Atan2(p.X, p.Y));
		}

		/* Linearly interpolates from x to y by variable t.
		 t is clamped between 0 and 1. */
		public static float Lerp(float x, float y, float t)
		{
			t = MathHelper.Clamp(t, 0, 1);
			return x + t * (y - x);
		}

		/* Performs a modulo calculation on two positive floats a and b
		 for a greater than b. Negative values will yield an incorrect answer.
		 If b is zero, a divide by 0 error will occur. */
		public static float Modulo(float a, float b)
		{
			if (a > b)
			{
				int q = (int)(a / b);
				a -= q * b;
			}
			return a;
		}

		// TODO: Move ColorToVec4 function
		public static Vector4 ColourToVector4(System.Drawing.Color colour)
		{
			Vector4 output;
			output.X = colour.R / 255.0f;
			output.Y = colour.G / 255.0f;
			output.Z = colour.B / 255.0f;
			output.W = colour.A / 255.0F;
			return output;
		}
	}
}