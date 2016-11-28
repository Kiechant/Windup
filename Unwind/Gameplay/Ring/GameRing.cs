using System;
using OpenTK;
using OpenTK.Input;

namespace Unwind
{
	public class GameRing : Ring
	{
		ushort revCount = 0;
		ushort maxRevs = 10;

		float unwindSpeed; // may need for future calculations
		float stress; // no. between 0 and 1 for unwind pressure
		float stressDelayDuration = 0.3f;
		float stressDelayTime;

		float minWindSpeed = 150;
		float unwindElasticity = 1.1f;
		float maxUnwindSpeed = 1000;
		float unwindAccelerator = 1.1f;

		public bool invulnerable { get; private set; }

		public GameRing()
		{
			windSpeed = 400;
			invulnerable = false;
		}

		public void Update(bool mouseDown)
		{
			if (mouseDown)
				Wind();
			else if (!unwound)
				Unwind();
		}

		override protected void Wind()
		{
			if (unwound || unwinding)
			{
				unwound = false;
				ResetUnwindVariables();
			}

			if (revCount < maxRevs)
			{
				IncreaseStress(-Time.deltaTimeSeconds);
				float t = stress * stress;
				float currWindSpeed = Mathc.Lerp(windSpeed, minWindSpeed, t);
				angle += currWindSpeed * Time.deltaTimeSeconds;
			}

			if (angle >= 360.0f)
			{
				revCount++;
				UpdateRevCount();
				angle = angle - 360.0f;

				if (revCount >= maxRevs)
				{
					angle = 0;
				}
			}

			Console.WriteLine("winding");
		}

		override protected void Unwind()
		{
			unwinding = true;

			float t = 0;

			if (stressDelayTime < stressDelayDuration)
			{
				stressDelayTime += Time.deltaTimeSeconds;
			}
			else
			{
				IncreaseStress(Time.deltaTimeSeconds / 2);
				t = stress * stress;
			}

			unwindSpeed = Mathc.Lerp(windSpeed, maxUnwindSpeed, t);

			if (t >= 1)
			{
				unwindSpeed = maxUnwindSpeed;
				invulnerable = true;
			}

			angle -= Time.deltaTimeSeconds * unwindSpeed;

			if (angle <= 0.0f)
			{
				if (revCount > 0)
				{
					revCount--;
					UpdateRevCount();
					angle = angle + 360.0f;
				}
				else //revCount == 0
				{
					unwound = true;
					ResetUnwindVariables();
					angle = 0;
				}
			}

			Console.WriteLine("unwinding");
		}

		private void IncreaseStress(float amount)
		{
			stress += amount;

			// clamps stress between 0 and 1
			if (stress > 1) stress = 1;
			else if (stress < 0)
			{
				stress = 0;
				stressDelayTime = 0;
			}
		}

		private void ResetUnwindVariables()
		{
			unwinding = false;
			invulnerable = false;
		}

		private void UpdateRevCount()
		{
			//GameObject rc = gameObject.transform.Find("RevCounter Text").gameObject;
			//TextMesh rc_tm = rc.GetComponent<TextMesh>() as TextMesh;
			//rc_tm.text = revCount.ToString();
		}
	}
}
