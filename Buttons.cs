using UnityEngine;

public class Buttons
{
	public class ConButton : IConsoleButton
	{
		private int frame = 0;

		public bool IsDown { get; set; }

		public bool JustPressed => IsDown && frame == Time.frameCount;

		public bool JustReleased => !IsDown && frame == Time.frameCount;

		public bool IsPressed
		{
			get
			{
				return IsDown;
			}
			set
			{
				if (value != IsDown)
				{
					IsDown = value;
					frame = Time.frameCount;
				}
			}
		}

		public void Call(Arg arg)
		{
		}
	}
}
