namespace ConVar;

[Factory("vis")]
public class Vis : ConsoleSystem
{
	[ClientVar]
	[Help("Turns on debug display of lerp")]
	public static bool lerp = false;

	[ServerVar]
	[Help("Turns on debug display of damages")]
	public static bool damage = false;

	[ServerVar]
	[ClientVar]
	[Help("Turns on debug display of attacks")]
	public static bool attack = false;

	[ServerVar]
	[ClientVar]
	[Help("Turns on debug display of protection")]
	public static bool protection = false;

	[ServerVar]
	[Help("Turns on debug display of weakspots")]
	public static bool weakspots = false;

	[ServerVar]
	[Help("Show trigger entries")]
	public static bool triggers = false;

	[ServerVar]
	[Help("Turns on debug display of hitboxes")]
	public static bool hitboxes = false;

	[ServerVar]
	[Help("Turns on debug display of line of sight checks")]
	public static bool lineofsight = false;

	[ServerVar]
	[Help("Turns on debug display of senses, which are received by Ai")]
	public static bool sense = false;
}
