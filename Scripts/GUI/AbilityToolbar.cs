using Godot;
using System;
using System.Collections.Generic;

public class AbilityToolbar : PanelContainer
{
	[Export] private Texture AttackIcon;
	[Export] private Texture HaltIcon;


	private List<PlayerCharacter> _pcOwners = new List<PlayerCharacter>();
	private List<AbilityToolbarButton> general = new List<AbilityToolbarButton>();
	private List<AbilityToolbarButton> abilities = new List<AbilityToolbarButton>();
	private List<AbilityToolbarButton> items = new List<AbilityToolbarButton>();
	private List<VSeparator> dividers = new List<VSeparator>();

	public override void _Ready()
	{
		GUIManager.RegisterElement(this);

		var buttons = GetChild(0).GetChildren();

		for (int i = 0; i < buttons.Count; i++)
		{
			//TODO It would probably be better to generate a bunch of icons for these instead...
			if (buttons[i].GetType() == typeof(AbilityToolbarButton))
			{
				AbilityToolbarButton tb = (AbilityToolbarButton)buttons[i];
				tb.Setup();
		
				switch (tb.ButtonType)
				{
					case ToolbarButtonType.ABILITY:
						abilities.Add(tb);
						tb.Connect("pressed", this, "OnAbilityPressed", new Godot.Collections.Array() {abilities.Count - 1});
						break;
					case ToolbarButtonType.ITEM:
						tb.Connect("pressed", this, "OnItemPressed", new Godot.Collections.Array() {items.Count - 1});
						items.Add(tb);
						break;
					default:
						general.Add(tb);
						break;
				}
			}
			else if (buttons[i].GetType() == typeof(VSeparator))
				dividers.Add((VSeparator)buttons[i]);
		}

		//Manually configure general icons; they're the same for everyone
		general[0].UpdateVisual(AttackIcon, new Color(1, 1, 1, 1)); //TODO This could show the user's weapon instead
		general[1].UpdateVisual(HaltIcon, new Color(1, 1, 1, 1));

		this.Visible = false;
	}

	public void UpdateFor(List<PlayerCharacter> group)
	{
		if (group.Count <= 1) //short-circuit if somehow passed group of 1 or 0
			UpdateFor(group[0]);
		else
		{
			_pcOwners = group;
			abilities.ForEach(tb => tb.Visible = false);
			dividers.ForEach(d => d.Visible = false);
		}

		this.Visible = true;
		RectSize = GetMinimumSize();
	}

	public void UpdateFor(PlayerCharacter pc)
	{
		_pcOwners.Clear();
		
		//Short circuit to hide if no one selected
		if (pc == null)
		{
			this.Visible = false;
			return;
		}

		_pcOwners.Add(pc);

		//Set abilities based on character
		abilities.ForEach(tb => tb.Visible = false);

		if (pc.Abilities.Count > 0)
		{
			dividers[0].Visible = true;

			for (int i = 0; i < pc.Abilities.Count; i++)
			{
				abilities[i].Visible = true;
				abilities[i].UpdateVisual(pc.Abilities[i].ToolbarIcon, pc.Abilities[i].IconColor);
			}
		}
		else
		{
			dividers[0].Visible = false;
		}

		//Set item based on character
		this.Visible = true;
		RectSize = GetMinimumSize();
	}
	
	private void OnForceAttackPressed()
	{
		foreach (var pc in _pcOwners)
			GD.Print($"Force Attack by {pc}");
	}


	private void OnForceHaltPressed()
	{
		foreach (var pc in _pcOwners)
		{
			pc.ForceHalt();
		}
	}


	private void OnAbilityPressed(int abilityIndex)
	{
		foreach (var pc in _pcOwners)
		{
			GD.Print($"Ability #{abilityIndex} Press by {pc}");
			try
			{
                pc.UseAbility(abilityIndex);
			}
			catch (Exception e)
			{
				GD.Print($"  Failed to cast! E: {e}");
			}

		}
	}


	private void OnItemPressed(int itemIndex)
	{
		foreach (var pc in _pcOwners)
			GD.Print($"Item #{itemIndex} Press by {pc}");
	}

}
