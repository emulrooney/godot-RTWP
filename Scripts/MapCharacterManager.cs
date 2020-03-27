using Godot;
using System.Collections.Generic;
using System.Linq;

public class MapCharacterManager : Node2D
{
    public static MapCharacterManager _mcm; //singleton

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
        if (_mcm == null)
            _mcm = this;
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
        _mcm.PresentCharacters.Add(registrant);

        if (registrant is PlayerCharacter)
            _mcm.PlayerCharacters.Add((PlayerCharacter)registrant);
        else if (registrant is MonsterCharacter)
            _mcm.MonsterCharacters.Add((MonsterCharacter)registrant);
    }

    public static void ResetPresent()
    {
        _mcm.PresentCharacters = new List<Character>();
    }

    public static void UnregisterPresent(Character character)
    {
        foreach (Character c in _mcm.PresentCharacters)
        {
            if (c.AttackTarget != null)
                c.AttackTarget = null;
        }

        if (_mcm.PresentCharacters.Contains(character))
            _mcm.PresentCharacters.Remove(character);

        if (_mcm.Selected.Contains(character))
            _mcm.Selected.Remove((PlayerCharacter)character);

        if (_mcm.PlayerCharacters.Contains(character))
            _mcm.Selected.Remove((PlayerCharacter)character);

        character.QueueFree();
    }

    public static Character SelectPartyMember(int partyMember, bool individualSelect = true)
    {
        if (partyMember == 0)
        {
            foreach (PlayerCharacter pc in _mcm.PlayerCharacters)
            {
                AddCharacterToSelected(pc);
            }

            return _mcm.PlayerCharacters.FirstOrDefault();
        }
        else if (_mcm.PlayerCharacters.Count >= partyMember)
        {
            if (individualSelect)
                DeselectAll();

            return AddPartyMemberToSelected(partyMember);
        }

        return null;
    }

    public static Character AddPartyMemberToSelected(int partyMember)
    {
        if (_mcm.PlayerCharacters.Count >= partyMember)
        {
            AddCharacterToSelected(_mcm.PlayerCharacters[partyMember - 1]);
            _mcm.UpdateSelectionCircles();
            return _mcm.PlayerCharacters[partyMember - 1];
        }

        return null;
    }

    public static Character DeselectPartyMember(int partyMember)
    {
        if (_mcm.PlayerCharacters.Count >= partyMember)
        {
            _mcm.Selected.Remove(_mcm.PlayerCharacters[partyMember - 1]);
            _mcm.UpdateSelectionCircles();
            return _mcm.PlayerCharacters[partyMember - 1];
        }

        return null;
    }

    public static void DeselectAll()
    {
        _mcm.Selected = new List<PlayerCharacter>();
        _mcm.UpdateSelectionCircles();
    }

    public static void AddCharacterToSelected(PlayerCharacter character)
    {
        if (!_mcm.Selected.Contains(character))
            _mcm.Selected.Add(character);

        _mcm.UpdateSelectionCircles();
    }

    
    /* VISUALS */

    private void UpdateSelectionCircles()
    {
        foreach (Character c in PlayerCharacters)
            SetSelectionCircle(c, false);

        foreach (Character s in Selected)
            SetSelectionCircle(s, true);
    }

    private void SetSelectionCircle(Character character, bool visibility)
    {
        var circle = (SelectionCircle)character.GetNode("SelectionCircle");
        circle.SetVisible(visibility);
    }

    /* INPUT MANAGEMENT */
    public static void MouseClick()
    {
        //if (Input.IsActionJustPressed)
        foreach (PlayerCharacter pc in _mcm.Selected)
        {
            pc.AttackTarget = null;
            pc.AddToPath(_mcm.GetGlobalMousePosition(), Input.IsActionPressed("modify"));   
        }
    }

    public static void EnemyClicked(MonsterCharacter monster)
    {
        if (Input.IsMouseButtonPressed(1)) //leftmouse
        {
            foreach (var pc in _mcm.Selected)
            {
                GD.Print($"{pc.Name} attacking Enemy {monster.Name}");
                pc.Path.Clear();
                pc.AttackTarget = monster;
            }
        }
    }

    public static void SelectAllInRect(List<PlayerCharacter> results)
    {
        //var players = results.OfType<PlayerCharacter>();
        _mcm.Selected.AddRange(results);

        _mcm.UpdateSelectionCircles();
    }

}
