using Godot;
using System;
using System.Collections.Generic;

public class CombatLog : Control
{
	[Export] private int MaxLines { get; set; } = 16;

	public static CombatLog _log;

	public RichTextLabel Output { get; private set; }

	private readonly Color[] TextColors = new Color[]
	{
		new Color(1, 1, 1, 1),          //0 Normal
		new Color(.8f, .8f, .8f, 1),    //1 Dim/Miss
		new Color(.7f, .7f, 1, 1),      //2 Ability
		new Color(1f, .1f, .1f, 1)      //3 Player Death
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
		_log.WriteLine($"{attacker} attacked {target}. [roll: {accuracyRoll}]", LogMessageType.NORMAL);
	}

	public static void Hit(string target, int damage)
	{
		_log.WriteLine($"  {target} took {damage} damage.", LogMessageType.NORMAL);
	}

	public static void UseAbility(string message)
	{
		_log.WriteLine(message, LogMessageType.ABILITY);
	}

	public static void Miss()
	{
		_log.WriteLine("    Miss!", LogMessageType.MISS);
	}

	public static void Death(string dead, bool isPlayerDeath = false)
	{
		_log.WriteLine($"  {dead} dies.", (isPlayerDeath ? LogMessageType.DEATH : LogMessageType.NORMAL ));
	}

	private void WriteLine(string message, LogMessageType textType)
	{
		Output.Newline();

		Output.PushColor(GetTextColor(textType));
		Output.AddText(message);
		Output.Pop();

		if (Output.GetLineCount() > MaxLines)
			Output.RemoveLine(0);
	}

	private Color GetTextColor(LogMessageType messageType)
	{
		switch (messageType)
		{
			case LogMessageType.MISS:
				return TextColors[1];
			case LogMessageType.ABILITY:
				return TextColors[2];
			case LogMessageType.DEATH:
				return TextColors[3];
			case LogMessageType.NORMAL:
			default:
				return TextColors[0];
		}
	}
}

enum LogMessageType {
	NORMAL,
	MISS,
	DEATH,
	ABILITY
}
