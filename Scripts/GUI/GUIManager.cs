using Godot;
using System;
using System.Collections.Generic;

public class GUIManager : CanvasLayer
{
	//Singleton
	private static GUIManager _gui;

	//Registered Controls; these need to add themselves to the manager
	private static CombatLog _combatLog;
	private static PartyIconStrip _partyIcons;
	private static CameraControls _cameraControls;
	private static AbilityToolbar _abilityToolbar;

	//Unregistered Controls; these don't have scripts so the manager can find them
	private static Button _pauseButton;

	private static int registeredCount = 0;

	//This will be cleared on _Ready IF execution order was screwed up
	private static Queue<Node> registrationQueue = new Queue<Node>();

	//Queue player characters who will need to be added to multiple GUI screens
	private static Queue<PlayerCharacter> characterQueue = new Queue<PlayerCharacter>();


	public override void _Ready()
	{
		if (_gui == null)
			_gui = this;
		else
			QueueFree();

		while (registrationQueue.Count > 0)
			RegisterElement(registrationQueue.Dequeue());

		_pauseButton = (Button)GetNode("PauseButton");
	}

	/* MULTIPLE GUI ELEMENTS */
	
	public static bool RegisterElement(Node element)
	{
		//If registration happens out of normal execution order, element added to registration queue
		//Queue will be cleared on _Ready
		if (_gui == null)
		{
			registrationQueue.Enqueue(element);
			return true;
		}

		string typeString = element.GetType().ToString();

		switch (typeString)
		{
			case "PartyIconStrip":
				_partyIcons = (PartyIconStrip)element;
				_partyIcons.Initialize();
				while (characterQueue.Count > 0)
					_partyIcons.SetupCharacterPortrait(characterQueue.Dequeue());
				return true;
			case "AbilityToolbar":
				_abilityToolbar = (AbilityToolbar)element;
				return true;
			case "CameraControls":
				_cameraControls = (CameraControls)element;
				return true;
			case "CombatLog":
				_combatLog = (CombatLog)element;
				return true;
			default:
				return false;
		}
	}

	public static int RegisterPlayerCharacter(PlayerCharacter pc)
	{
		if (_partyIcons != null)
			_partyIcons.SetupCharacterPortrait(pc);
		else
			characterQueue.Enqueue(pc);

		return ++registeredCount;
	}

	public static void WipePartyElements()
	{
		//Party Icons
		_partyIcons.WipeAllPortraits();
	}

	public static void UpdateFor(PlayerCharacter pc)
	{
		//Update all GUI related to this character
		if (_partyIcons != null)
			_partyIcons.UpdateFor(pc);
	}

	/* CHARACTER PORTRAITS AND SELECTION */

	//Modify Override -- used by group to ensure drag keeps working
	public static void SelectPartyMember(int partyMember, bool modifyOverride = false)
	{
		//TODO At some point, players will have friendly spells to hit eachother with
		//They should be able to target eachother by clicking portraits
		InputManager.InterruptTargetedAbility();

		//if SHIFT held, add to selection
		//if regular mouse, set to active
		if (Input.IsActionPressed("modify") || modifyOverride)
		{
			//Right-click first
			//This allows the 'else' to handle left clicks AND keyboard shortcuts

			/* REMOVING FROM SELECTED */
			if (Input.IsMouseButtonPressed(2))
			{
				var success = LocalCharacterManager.DeselectPartyMember(partyMember);
				if (success != null)
					_partyIcons.SetPortraitSelected(partyMember, false);
			}
			else
			{
				/* ADDING TO SELECTED */
				var success = LocalCharacterManager.AddPartyMemberToSelected(partyMember);
				if (success != null)
				{
					_partyIcons.SetPortraitSelected(partyMember, true);

					if (_cameraControls != null)
						_cameraControls.FocusPartyMember(partyMember);
				}


			}
		}
		else
		{
			/* SELECT ONE */
			var success = LocalCharacterManager.SelectPartyMember(partyMember);
			_cameraControls.FocusOn(success);

			if (success != null)
			{
				//Active party member, individual
				_partyIcons.SetPortraitSelected(partyMember, true, true);

				if (_cameraControls != null)
					_cameraControls.FocusPartyMember(partyMember);
			}
		}

		UpdateAbilityToolbar();
	}

	public static void FocusOn(PlayerCharacter pc)
	{
		_cameraControls.FocusOn(pc);
	}

	public static void SelectPartyMembers(List<PlayerCharacter> party)
	{
		foreach (var pc in party)
		{
			SelectPartyMember(pc.PartyMemberOrder, true); //modify override on
		}
	}

	public static void UpdateAbilityToolbar()
	{
		if (_abilityToolbar != null)
		{
			if (LocalCharacterManager.GetSelectedCount() == 1)
				_abilityToolbar.UpdateFor(LocalCharacterManager.GetSingleSelected());
			else
				_abilityToolbar.UpdateFor(LocalCharacterManager.GetAllSelected());

		}
	}

	/// <summary>
	/// Toggles 'paused' status. While paused, orders can be issued but nothing is executed.
	/// </summary>
	/// NOTE: This is in GUI Manager rather than input manager due to Godot signals.
	public static void TogglePause()
	{
		InputManager.IsPaused = !InputManager.IsPaused;
		_gui.GetTree().Paused = InputManager.IsPaused;
		_pauseButton.Text = (InputManager.IsPaused ? "PAUSED" : "UNPAUSED" );
	}
}


