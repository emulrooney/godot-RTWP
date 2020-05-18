using Godot;
using System;
using System.Collections.Generic;

public class TurnManager : Node
{
    public static bool CombatMode { get; set; }

    public static Character Active { get; set; }

    public static List<Character> LeftInTurn { get; set; }

}
