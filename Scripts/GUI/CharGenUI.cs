using Godot;
using System;
using System.Collections.Generic;

public class CharGenUI : Control
{
	[Export] public string[] portraitPaths = new string[]
	{
		"res://Assets/GUI/Portraits/debugAxeGuySmall.jpg",
		"res://Assets/GUI/Portraits/debugKnifeLadySmall.jpg"
	};

	[Export] public string[] spriteSetPaths = new string[]
	{
		"res://Assets/Characters/debugPlayer/idle (1).png",
		"res://Assets/Characters/debugPlayer2/idle (1).png"
	};

	private int selectedPortrait = 0;
	private int selectedSprite = 0;
	private Color selectedColor = new Color(255, 255, 255, 1); //White

	private CharGenStep[] steps;
	private CharGenStep nameStep;
	private CharGenStep attributesStep;
	private CharGenStep specialAbilitiesStep;

	private string currentName = "";
	private TextureRect currentPortrait;
	private TextureRect currentSprite;
	private RichTextLabel currentStatOutput;

	private AcceptDialog nameDialog;

	private Button saveCharacterButton;
	private Button previousStepButton;
	private Button nextStepButton;

	private Statblock currentStatBlock;
	private List<Ability> currentAbilities = new List<Ability>();

	public override void _Ready()
	{
		var statusUpdate = GetTree().GetNodesInGroup("PlayerStatusUpdate");
		currentPortrait = (TextureRect)statusUpdate[0];
		currentSprite = (TextureRect)statusUpdate[1];
		currentStatOutput = (RichTextLabel)statusUpdate[2];

		var buttons = GetTree().GetNodesInGroup("CharGenButtons");
		steps = new CharGenStep[4];
		steps[0] = new CharGenStep(0, (Button)buttons[0]);  //Name
		steps[1] = new CharGenStep(1, (Button)buttons[1]);  //Attributes
		steps[2] = new CharGenStep(2, (Button)buttons[2]);  //Special Abilities
		steps[3] = new CharGenStep(3, (Button)buttons[3]);  //Customization
		
		saveCharacterButton = (Button)buttons[4];
		previousStepButton = (Button)buttons[5];
		nextStepButton = (Button)buttons[6];

		var nd = Owner.GetNode("CenterContainer").GetNode("NameDialog");
		nameDialog = (AcceptDialog)nd;
		SetNameDialogVisible(true);
	}

	public void StepPortrait(int direction)
	{
		selectedPortrait += direction;
		if (selectedPortrait < 0)
			selectedPortrait = portraitPaths.Length - 1;
		else if (selectedPortrait >= portraitPaths.Length)
			selectedPortrait = 0;
	}

	public void UpdateStats()
	{
		var sb = new System.Text.StringBuilder();

		sb.AppendLine(currentName.Length > 0 ? currentName : "Unnamed Character");
		sb.AppendLine();

		if (attributesStep.Complete)
		{
			sb.AppendLine("--ATTRIBUTES--");
			sb.AppendLine($"HP  : {currentStatBlock.MaxHP}");
			sb.AppendLine($"ACC : {currentStatBlock.BaseAccuracy}");
			sb.AppendLine($"DAM : {currentStatBlock.BaseDamage}");
			sb.AppendLine($"DEF : {currentStatBlock.Defense}");
			sb.AppendLine();
		}

		if (specialAbilitiesStep.Complete)
		{
			sb.AppendLine("--ABILITIES--");
			foreach (Ability a in currentAbilities)
			{
				sb.AppendLine("- " + a.AbilityName);
			}
		}


	}

	public void UpdateButtons()
	{
		if (nameStep.Complete)
			attributesStep.StepButton.Disabled = false;

		if (attributesStep.Complete)
			specialAbilitiesStep.StepButton.Disabled = false;
	}

	//Signaled
	public void SaveName()
	{
		var te = (TextEdit)nameDialog.GetNode("TextEdit");
		if (te.Text.Length > 0)
		{
			currentName = te.Text;
			SetNameDialogVisible(false);
			UpdateStats();
		}

	}

	public void SetNameDialogVisible(bool visible)
	{

		if (visible)
			nameDialog.Popup_();
		else
			nameDialog.Visible = false;
	}

}
