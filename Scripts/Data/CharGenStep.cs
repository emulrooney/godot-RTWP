using Godot;
using System;

public struct CharGenStep
{
    public int Index { get; set; }
    public Button StepButton {get; set;}
    public bool Complete { get; set; }

    public CharGenStep(int index, Button button)
    {
        Index = index;
        StepButton = button;
        Complete = false;
    } 
}
