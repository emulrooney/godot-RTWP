using Godot;
using System;

public interface IAbility { 

    string AbilityName { get; set; }
    Texture ToolbarIcon {get; set;}
    Color IconColor {get; set;}

    bool Use();


}
