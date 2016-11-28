using System;

namespace Unwind
{
	public static class Blur
	{
		public static void GaussianBlur(float[] pixels, int width, int height, int radius)
		{
			
		}

		public static void BoxBlur(float[] pixels, int width, int height, int radius)
		{

		}

		private static void HorizontalBoxBlur(float[] pixels, int width, int height, int radius)
		{

		}

		private static void TotalBoxBlur(float[] pixels, int width, int height, int radius)
		{

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
