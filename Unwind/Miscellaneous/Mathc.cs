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

		public static Vector3 ColourToVector3(System.Drawing.Color colour)
		{
			Vector3 output;
			output.X = colour.R / 255.0f;
			output.Y = colour.G / 255.0f;
			output.Z = colour.B / 255.0f;
			return output;
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

		/* TODO: Move Mathc Mipmapping function to appropriate class
		 or replace with OpenGL 3.0+ functionality. */
		/* Generates mipmap texture at a particular level for a four channel texture with
		 byte pixel format. */
		public static void GenerateMipmapTexture(byte[] src, int level, int width, int height, out byte[] dst)
		{
			if (level == 0)
			{
				dst = new byte[src.Length];
				for (int i = 0; i < src.Length; i++)
					dst[i] = src[i];
			}
			else
			{
				int n = 4; // Number of channels
				int h = height / 2, w = width / 2;
				var med = new byte[src.Length / 4];

				for (int ch = 0; ch < n; ch++)
				{
					for (int i = 0; i < h; i++)
					{
						for (int j = 0; j < w; j++)
						{
							int r = n * width * i, s = 2 * n * j, t = 2 * r;
							int u = t + s + ch, v = u + n * width;
							double val = src[u] + src[u + n] + src[v] + src[v + n];
							med[(n * w * i) + (n * j) + ch] = (byte)Math.Round(val / 4);
						}
					}
				}

				GenerateMipmapTexture(med, level - 1, w, h, out dst);
			}
		}
	}
}