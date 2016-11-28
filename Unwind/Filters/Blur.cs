using System;

namespace Unwind
{
	public static class Blur
	{
		public static void GaussianBlur(byte[] src, ref byte[] dst, int width, int height, int radius)
		{
			int n = 3;
			int[] sizes = BoxKernelSizes(radius, n);

			for (int i = 0; i < n; i++)
			{
				sizes[i] = (sizes[i] - 1) / 2;
				BoxBlur(src, ref dst, width, height, sizes[i]);

				for (int j = 0; j < src.Length; j++)
					src[j] = dst[j];
			}
		}

		public static void BoxBlur(byte[] src, ref byte[] dst, int width, int height, int radius)
		{
			for (int i = 0; i < src.Length; i++)
				dst[i] = src[i];

			HorizontalBoxBlur(dst, ref src, width, height, radius);
			VerticalBoxBlur(src, ref dst, width, height, radius);
		}

		private static void HorizontalBoxBlur(byte[] src, ref byte[] dst, int width, int height, int radius)
		{
			double iarr = 1.0d / (radius + radius + 1);

			for (int i = 0; i < height; i++)
			{
				int ti = i * width, li = ti, ri = ti + radius;
				uint fv = src[ti], lv = src[ti + width - 1], val = (uint)(radius + 1) * fv;

				for (int j = 0; j < radius; j++)
				{
					val += src[ti + j];
				}
				for (int j = 0; j <= radius; j++)
				{
					val += src[ri++] - fv;
					dst[ti++] = (byte)Math.Round(val * iarr);
				}
				for (int j = radius + 1; j < width - radius; j++)
				{
					val += (uint)(src[ri++] - src[li++]);
					dst[ti++] = (byte)Math.Round(val * iarr);
				}
				for (int j = width - radius; j < width; j++)
				{
					val += lv - src[li++];
					dst[ti++] = (byte)Math.Round(val * iarr);
				}
			}
		}

		private static void VerticalBoxBlur(byte[] src, ref byte[] dst, int width, int height, int radius)
		{
			double iarr = 1.0d / (radius + radius + 1);

			for (int i = 0; i < width; i++)
			{
				int ti = i, li = ti, ri = ti + radius * width;
				uint fv = src[ti], lv = src[ti + width * (height - 1)], val = (uint)(radius + 1) * fv;

				for (int j = 0; j < radius; j++)
				{
					val += src[ti + j * width];
				}
				for (int j = 0; j <= radius; j++)
				{
					val += src[ri] - fv;
					dst[ti] = (byte)Math.Round(val * iarr);
					ri += width; ti += width;
				}
				for (int j = radius + 1; j < height - radius; j++)
				{
					val += (uint)(src[ri] - src[li]);
					dst[ti] = (byte)Math.Round(val * iarr);
					li += width; ri += width; ti += width;
				}
				for (int j = height - radius; j < height; j++)
				{
					val += lv - src[li];
					dst[ti] = (byte)Math.Round(val * iarr);
					li += width; ti += width;
				}
			}
		}

		/* Calculates a series of n box kernal sizes to approximate a gaussian blur
		 with a radius sigma. */
		private static int[] BoxKernelSizes(int sigma, int n)
		{
			var sizes = new int[n];

			double wIdeal = Math.Sqrt(((12.0d * sigma * sigma) / n) + 1);
			int w1 = (int)Math.Floor(wIdeal);
			if (w1 % 2 == 0)
				w1--;
			int wu = w1 + 2;

			double mIdeal = (12.0d * sigma * sigma - (double)n * w1 * w1 - 4.0d * n * w1 - 3.0d * n) / (-4.0d * w1 - 4);
			int m = (int)Math.Round(mIdeal);

			for (int i = 0; i < n; i++)
				sizes[i] = i < m ? w1 : wu;

			return sizes;
		}
	}
}
