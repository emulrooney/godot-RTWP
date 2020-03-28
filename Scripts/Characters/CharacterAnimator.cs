using Godot;
using System;
using System.Collections.Generic;

public class CharacterAnimator : AnimatedSprite
{
    //TODO: If other animations affect offset, the setHFlip method will not be adequate
    //Need to track base offset, current offset as done by sprite
    //affect current w methods
    //Probably flip should affect base only

    [Export] private Vector2 FlipOffset { get; set; } //When flipping, offset sprite this much

    public void SetFlipHWithOffset(bool flipH)
    {

        if (flipH)
            Offset = new Vector2(Offset.x + FlipOffset.x, Offset.y);
        else
            Offset = new Vector2(Offset.x - FlipOffset.x, Offset.y);
        
        SetFlipH(flipH);
    }

}
