using Godot;
using System;

public class AbilityEffect : Node
{
	[Export] public AbilityEffectType Type { get; private set; }
	[Export] public StatType StatType { get; private set; }
	[Export] public int BonusAccuracy {get; private set;}
	[Export] public int BonusDie {get; private set;}
	[Export] public int BonusSides {get; private set;}
	[Export] public int BonusFlatValue { get; private set; }
	[Export] public int BonusTime { get; private set; }


}
