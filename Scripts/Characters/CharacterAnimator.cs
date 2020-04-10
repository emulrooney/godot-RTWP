using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CharacterAnimator : AnimatedSprite
{
	//TODO: If other animations affect offset, the setHFlip method will not be adequate
	//Need to track base offset, current offset as done by sprite
	//affect current w methods
	//Probably flip should affect base only

	[Export] private Vector2 FlipOffset { get; set; } //When flipping, offset sprite this much

    [Export] private Color[] HitFlashColors { get; set; }

	public Particles2D HitParticles { get; private set; }

    public SelectionCircle SelectionCircle { get; private set; }


	public override void _Ready()
	{
		HitParticles = GetNodeOrNull<Particles2D>("OnHit");
        SelectionCircle = GetNodeOrNull<SelectionCircle>("SelectionCircle");
	}

	public void SetFlipHWithOffset(bool flipH)
	{

		if (flipH)
			Offset = new Vector2(Offset.x + FlipOffset.x, Offset.y);
		else
			Offset = new Vector2(Offset.x - FlipOffset.x, Offset.y);
		
		FlipH = flipH;
	}

	public void OnHit()
	{
		if (HitParticles != null)
			HitParticles.Emitting = true;

        if (HitFlashColors.Length > 0)
            FlickerOnHit();
	}

    private async Task FlickerOnHit()
    {
        Color baseColor = SelfModulate;

        for (int i = 0; i < 4; i++)
        {
            if (Owner.IsQueuedForDeletion())
                return;

            SelfModulate = HitFlashColors[i % HitFlashColors.Length];
            await ToSignal(GetTree(), "idle_frame");
        }

        SelfModulate = baseColor;
    }

    public void SetSelectionCircleOn(bool visibility)
    {
        if (SelectionCircle != null && !SelectionCircle.IsQueuedForDeletion() )
            SelectionCircle.Visible = visibility;
    }

}
