using Godot;
using System;

public class CombatLog : Control
{
	[Export] private int MaxLines { get; set; } = 16;

	public static CombatLog _log;

	public RichTextLabel Output { get; private set; }

	[Export] private Color[] TextColors = new Color[]
	{
		new Color(1, 1, 1, 1),
		new Color(.8f, .8f, .8f, 1),
		new Color(.75f, 1f, .8f, 1)
		//"#DDDDDD",  //Miss
		//"#cffffe", //Player
		//"#ffd9d6", //Enemy
		//"#a60037", //Player Killed
		//"#efbaff"  //Hostile Magic
	};

	public override void _Ready()
	{
		if (_log == null)
			_log = this;
		else
		{
			QueueFree();
			return;
		}

		Output = GetNode<RichTextLabel>("Output");
	}

	public static void Attack(string attacker, string target, int accuracyRoll)
	{
		_log.WriteLine($"{attacker} attacked {target}. [roll: {accuracyRoll}]");
	}

	public static void Hit(string target, int damage)
	{

		_log.WriteLine($"  {target} took {damage} damage.");
	}

	public static void Miss()
	{
		_log.WriteLine("    Miss!", 1);
	}

	public static void Death(string dead)
	{
		_log.WriteLine($"  {dead} dies.");
	}

	private void WriteLine(string message, int textType = -1)
	{
		Output.Newline();

		if (textType > -1)
			Output.PushColor(TextColors[textType]);

		Output.AddText(message);

		if (textType > -1)
			Output.Pop();

		if (Output.GetLineCount() > MaxLines)
			Output.RemoveLine(0);
	}
	


}
