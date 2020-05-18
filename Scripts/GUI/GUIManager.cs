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

		switch (element)
		{
			case PartyIconStrip pis:
				_partyIcons = pis;
				_partyIcons.Initialize();
				while (characterQueue.Count > 0)
					_partyIcons.SetupCharacterPortrait(characterQueue.Dequeue());
				return true;
			case AbilityToolbar at:
				_abilityToolbar = at;
				return true;
			case CameraControls cc:
				_cameraControls = cc;
				_cameraControls.Loaded = true;
				return true;
			case CombatLog cl:
				_combatLog = cl;
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
				//var success = LocalCharacterManager.DeselectPartyMember(partyMember);
				//if (success != null)
				//	_partyIcons.SetPortraitSelected(partyMember, false);
			}
			else
			{
				/* ADDING TO SELECTED */
				//var selectedCharacter = LocalCharacterManager.AddPartyMemberToSelected(partyMember);
				//if (selectedCharacter != null)
				//{
				//	_partyIcons.SetPortraitSelected(partyMember, true);

				//	if (_cameraControls != null)
				//		FocusOn(selectedCharacter);
				//}


			}
		}
		else
		{
			/* SELECT ONE */
			//var selectedCharacter = LocalCharacterManager.SelectPartyMember(partyMember);

			//if (selectedCharacter != null)
			//{
			//	//Active party member, individual
			//	_partyIcons.SetPortraitSelected(partyMember, true, true);

			//	if (_cameraControls != null)
			//		FocusOn(selectedCharacter);
			//}
		}

		UpdateAbilityToolbar();
	}

	/// <summary>
	/// Set 'Loaded' for all 'ILoadable' GUI elements. Used to set elements to not function
	/// while the map is transitioning. 
	/// </summary>
	/// <param name="loaded"></param>
	public static void SetLoadables(bool loaded)
	{
		_cameraControls.Loaded = loaded;
	}

	public static void FocusAt(Vector2 location)
	{
		_cameraControls.FocusAt(location);
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
			//if (LocalCharacterManager.GetSelectedCount() == 1)
			//	_abilityToolbar.UpdateFor(LocalCharacterManager.GetSingleSelected());
			//else
			//	_abilityToolbar.UpdateFor(LocalCharacterManager.GetAllSelected());

		}
	}

	/// <summary>
	/// Toggles 'paused' status. While paused, orders can be issued but nothing is executed.
	/// </summary>
	/// NOTE: This is in GUI Manager rather than input manager due to Godot signals.
	public static void TogglePause()
	{
		GM.IsPaused = !GM.IsPaused;
		_gui.GetTree().Paused = GM.IsPaused;
		_pauseButton.Text = (GM.IsPaused ? "PAUSED" : "UNPAUSED" );
	}
}


