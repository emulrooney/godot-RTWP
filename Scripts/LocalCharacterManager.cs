using Godot;
using System.Collections.Generic;
using System.Linq;

public class LocalCharacterManager : Node2D
{
	public static LocalCharacterManager _lcm; //singleton

	/* CHARACTER LISTS -- these will populate themselves on load*/
	//Everyone in the scene
	private List<Character> PresentCharacters { get; set; } = new List<Character>();
	//Every player in the scene
	private List<PlayerCharacter> PlayerCharacters { get; set; } = new List<PlayerCharacter>();
	//Every player currently selected 
	private List<PlayerCharacter> Selected { get; set; } = new List<PlayerCharacter>();
	//Every hostile character
	private List<MonsterCharacter> MonsterCharacters {get; set;} = new List<MonsterCharacter>();

	public override void _Ready()
	{
		if (_lcm == null)
			_lcm = this;
		else
			QueueFree();
	}

	/* LOCAL CHARACTER MANAGEMENT */

	/// <summary>
	/// Called by characters to add themselves to appropriate list
	/// </summary>
	/// <param name="registrant">Any character</param>
	public static void RegisterPresent(Character registrant)
	{
		_lcm.PresentCharacters.Add(registrant);

		if (registrant is PlayerCharacter)
			_lcm.PlayerCharacters.Add((PlayerCharacter)registrant);
		else if (registrant is MonsterCharacter)
			_lcm.MonsterCharacters.Add((MonsterCharacter)registrant);
	}
	
	public static void ResetAll()
	{
		ResetPresent();
		ResetMonsters();
		ResetPlayers();

		TopPrinter.One = "Present Count: " + _lcm.PresentCharacters.Count;
	}

	public static void ResetPresent()
	{
		_lcm.PresentCharacters.Clear();
	}

	public static void ResetPlayers()
	{
		_lcm.PlayerCharacters.Clear();
		_lcm.Selected.Clear();
	}

	public static void ResetMonsters()
	{
		_lcm.MonsterCharacters.Clear();
	}

	public static void UnregisterPresent(Character character)
	{
		foreach (Character c in _lcm.PresentCharacters)
		{
			if (c.AttackTarget != null)
				c.AttackTarget = null;
		}

		if (_lcm.PresentCharacters.Contains(character))
			_lcm.PresentCharacters.Remove(character);

		if (_lcm.Selected.Contains(character))
			_lcm.Selected.Remove((PlayerCharacter)character);

		if (_lcm.PlayerCharacters.Contains(character))
			_lcm.Selected.Remove((PlayerCharacter)character);

		character.QueueFree();
	}

	public static PlayerCharacter SelectPartyMember(PlayerCharacter partyMember, bool exclusive = true)
	{
		if (exclusive)
		{
			if (_lcm.Selected.Contains(partyMember))
				//ie. if we already have them, focus
				GUIManager.FocusOn(partyMember);

			DeselectAll();
		}

		AddPartyMemberToSelected(partyMember);
		return partyMember;  //this allows the overload to still return something
	}

	/// <summary>
	/// Overloaded version. Take party member index to select them. If exclusive, 
	/// </summary>
	/// <param name="partyMember"></param>
	/// <param name="exclusive"></param>
	/// <returns></returns>
	public static PlayerCharacter SelectPartyMember(int partyMember, bool exclusive = true)
	{
		if (partyMember == 0)
		{
			foreach (PlayerCharacter pc in _lcm.PlayerCharacters)
				SelectPartyMember(pc);

			return _lcm.PlayerCharacters.FirstOrDefault();
		}
		else if (_lcm.PlayerCharacters.Count >= partyMember)
		{
			if (exclusive)
				DeselectAll();

			return SelectPartyMember(_lcm.PlayerCharacters[partyMember - 1]);
		}

		return null;
	}

	public static PlayerCharacter AddPartyMemberToSelected(int partyMember)
	{
		if (_lcm.PlayerCharacters.Count >= partyMember)
		{
			AddPartyMemberToSelected(_lcm.PlayerCharacters[partyMember - 1]);
			return _lcm.PlayerCharacters[partyMember - 1];
		}

		return null;
	}

	public static void AddPartyMemberToSelected(PlayerCharacter character)
	{
		if (!_lcm.Selected.Contains(character))
			_lcm.Selected.Add(character);

		_lcm.UpdateSelectionCircles();
	}

	public static Character DeselectPartyMember(int partyMember)
	{
		if (_lcm.PlayerCharacters.Count >= partyMember)
		{
			_lcm.Selected.Remove(_lcm.PlayerCharacters[partyMember - 1]);
			_lcm.UpdateSelectionCircles();
			return _lcm.PlayerCharacters[partyMember - 1];
		}

		return null;
	}

	public static void DeselectAll()
	{
		_lcm.Selected = new List<PlayerCharacter>();
		_lcm.UpdateSelectionCircles();
	}

	
	/* GET INFORMATION */

	/// <summary>
	/// Gets the Vector2 position of everyone in the party.
	/// </summary>
	/// <returns>Vector2[] of party member co-ords</returns>
	public static Vector2[] GetPartyPositions()
	{
		return _lcm.PlayerCharacters.Select(pc => pc.Position).ToArray();
	}

	
	/* VISUALS */

	private void UpdateSelectionCircles()
	{
		foreach (Character c in PlayerCharacters)
			c.SetSelectionCircle(false);

		foreach (Character s in Selected)
			s.SetSelectionCircle(true);
	}

	/* INPUT MANAGEMENT */
	public static void MapClick(Vector2 location, Navigation2D nav = null)
	{
		foreach (PlayerCharacter pc in _lcm.Selected)
		{
			pc.AttackTarget = null;

			if (nav != null)
				pc.AddToPath(nav.GetSimplePath(pc.Position, location), Input.IsActionPressed("modify"));
			else 
				pc.AddToPath(new Vector2[] { location }, Input.IsActionPressed("modify"));   
		}
	}

	public static void EnemyClicked(MonsterCharacter monster)
	{
		foreach (var pc in _lcm.Selected)
		{
			pc.QueuedMoves.Clear();
			pc.AttackTarget = monster;
		}
	}

	public static void SelectAllInRect(List<PlayerCharacter> results, bool exclusive = true)
	{
		if (exclusive)
			DeselectAll();

		_lcm.Selected.AddRange(results);

		_lcm.UpdateSelectionCircles();

		GUIManager.SelectPartyMembers(results);
	}

	public static int GetSelectedCount()
	{
		return _lcm.Selected.Count();
	}

	public static PlayerCharacter GetSingleSelected()
	{
		if (GetSelectedCount() == 1)
			return _lcm.Selected.First();

		return null;
	}

	public static List<PlayerCharacter> GetAllSelected()
	{
		return _lcm.Selected;
	}
}
