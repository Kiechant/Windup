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

		public static float Lerp(float x, float y, float t)
		{
			t = MathHelper.Clamp(t, 0, 1);
			return x + t * (y - x);
		}
	}
}