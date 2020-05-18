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
    private PlayerCharacter LeadCharacter { get; set; }

	//Every hostile character
	private List<MonsterCharacter> MonsterCharacters {get; set;} = new List<MonsterCharacter>();
    public static MapLogic CurrentMap { get; set; }

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
	}

	public static void ResetPresent()
	{
		_lcm.PresentCharacters.Clear();
	}

	public static void ResetPlayers()
	{
		_lcm.PlayerCharacters.Clear();
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

		character.QueueFree();
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

    public static Character GetCharacterAt(Vector2 location)
    {
        var gridLocation = GM.GetGridPosition(location); //Normalize to grid coords

        foreach (Character c in _lcm.PresentCharacters)
        {
            if (c.GridPosition == gridLocation)
            {
                GD.Print("Got character with click: " + location + " at " + gridLocation);
                return c;
            }
        }

        return null;
    }


}
