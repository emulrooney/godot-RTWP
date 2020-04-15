using Godot;
using System;
using System.Collections.Generic;

public class AbilityToolbar : PanelContainer
{
	PlayerCharacter _pcOwner;

	List<TextureButton> general = new List<TextureButton>();
	List<TextureButton> abilities = new List<TextureButton>();
	List<TextureButton> items = new List<TextureButton>();
	List<VSeparator> dividers = new List<VSeparator>();

	public override void _Ready()
	{
		GUIManager.RegisterElement(this);

		foreach (var item in GetChild(0).GetChildren())
		{
			//TODO It would probably be better to generate a bunch of items for these instead...
			if (item.GetType() == typeof(TextureButton))
			{
				TextureButton tb = (TextureButton)item;
				if (tb.Name.Contains("General"))
					general.Add(tb);
				else if (tb.Name.Contains("Ability"))
					abilities.Add(tb);
				else if (tb.Name.Contains("Item"))
					items.Add(tb);
			}
			else if (item.GetType() == typeof(VSeparator))
				dividers.Add((VSeparator)item);
		}

		this.Visible = false;
	}

	public void UpdateFor(PlayerCharacter pc)
	{
		//Short circuit to hide if no one selected
		if (pc == null)
		{
			this.Visible = false;
			return;
		}

		_pcOwner = pc;

		//Set abilities based on character
		abilities.ForEach(tb => tb.Visible = false);

		if (_pcOwner.Abilities.Count > 0)
		{
			dividers[0].Visible = true;

			for (int i = 0; i < _pcOwner.Abilities.Count; i++)
			{
				abilities[i].Visible = true;
				abilities[i].TextureNormal = _pcOwner.Abilities[i].ToolbarIcon;
				abilities[i].Modulate = _pcOwner.Abilities[i].IconColor;
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
		GD.Print($"Force Attack by {_pcOwner}");
	}


	private void OnForceHaltPressed()
	{
		if (_pcOwner != null)
			_pcOwner.ForceHalt();
	}


	private void OnAbilityPressed(int abilityIndex)
	{
		GD.Print($"Ability #{abilityIndex} Press by {_pcOwner}");
	}


	private void OnItemPressed(int itemIndex)
	{
		GD.Print($"Item #{itemIndex} Press by {_pcOwner}");
	}

}
