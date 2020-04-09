using Godot;
using System;
using System.Collections.Generic;

public class GUIManager : CanvasLayer
{
    //Singleton
    private static GUIManager _gui;

    //Controls; these need to register themselves
    private static PartyIconStrip _partyIcons;
    private static CameraControls _cameraControls;

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
    }
    
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
            case "CameraControls":
                _cameraControls = (CameraControls)element;
                return true;
            default:
                return false;
        }
    }

    public static void RegisterPlayerCharacter(PlayerCharacter pc)
    {
        GD.Print("Register called by " + pc.CharacterName);

        if (_partyIcons != null)
            _partyIcons.SetupCharacterPortrait(pc);
        else
            characterQueue.Enqueue(pc);
    }

    public static void WipePartyElements()
    {
        GD.Print("Wiping all player elements!");

        //Party Icons
        _partyIcons.WipeAllPortraits();
    }


    /* CHARACTER PORTRAITS */

    public static void ClickPortrait(int partyMember)
    {
        //if SHIFT held, add to selection
        //if regular mouse, set to active
        if (Input.IsActionPressed("modify"))
        {
            if (Input.IsMouseButtonPressed(1))
            {
                var success = MapCharacterManager.AddPartyMemberToSelected(partyMember);
                if (success != null)
                {
                    _partyIcons.SetPortraitSelected(partyMember, true);

                    if (_cameraControls != null)
                        _cameraControls.FocusPartyMember(partyMember);
                }
            }
            else if (Input.IsMouseButtonPressed(2))
            {
                var success = MapCharacterManager.DeselectPartyMember(partyMember);
                if (success != null)
                    _partyIcons.SetPortraitSelected(partyMember, false);
            }
        }
        else
        {
            if (Input.IsMouseButtonPressed(1))
            {
                var success = MapCharacterManager.SelectPartyMember(partyMember);
                if (success != null)
                {
                    //Active party member, individual
                    _partyIcons.SetPortraitSelected(partyMember, true, true);

                    if (_cameraControls != null)
                        _cameraControls.FocusPartyMember(partyMember);
                }
            }
        }
    }
}
