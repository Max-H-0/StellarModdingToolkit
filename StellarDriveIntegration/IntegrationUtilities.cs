using Core.Services;
using MelonLoader;
using Player.Controls;
using Player.Interaction;
using Player.Interface.Model;
using Player.LocalClient;
using System;
using System.Collections.Generic;
using System.Reflection;
using UI.Common;
using UnityEngine;
using UnityEngine.InputSystem;

using Object = UnityEngine.Object;

namespace StellarModdingToolkit.StellarDriveIntegration;

/// <summary>
/// Provides various utilities to avoid direct interaction with StellarDrive's code in the rest of the project
/// </summary>
public static class IntegrationUtilities
{
    private static Dictionary<object, List<MenuImposter>> _imposters = [];

    /// <summary>
    /// Creates and registers a MenuImposter that's assosciated with the object
    /// </summary>
    public static T? CreateMenuImposter<T>(object obj) where T : MenuImposter
    {
        MenuTracker tracker = ServiceLocator.GetService<MenuTracker>();


        if (tracker is null) return null;


        var newImposter = new GameObject($"MenuImposter ({obj})").AddComponent<T>();

        if (!_imposters.ContainsKey(obj)) _imposters.Add(obj, []);

        MelonLogger.Msg(_imposters.ContainsKey(obj));
        tracker.TrackMenu(newImposter);
        _imposters[obj].Add(newImposter);

        return newImposter;
    }

    /// <summary>
    /// Destroys all of the MenuImposter assosciated with the object
    /// </summary>
    public static void DestroyAllMenuImposters(object obj)
    {
        if (!_imposters.ContainsKey(obj)) return;

        _imposters[obj].ForEach(i =>
        {
            ServiceLocator.GetService<MenuTracker>()?
                          .UntrackMenu(i);

            Object.Destroy(i.gameObject);
        });
        _imposters.Remove(obj);
    }

    /// <summary>
    /// Can be used to enable or disable various fatures such as the Player's ability to Walk or Input in general
    /// </summary>
    public static void SetBehaviourStates(PlayerBehaviourFlags flags, bool state, bool respectStateMap = false)
    {
        var behaviourTypes = GetFlagBehaviourTypes(flags);
        var behaviourActivator = Object.FindObjectOfType<PlayerBehaviourActivator>();

        if (behaviourActivator is null) return;

        var enabledBehavioursMap = typeof(PlayerBehaviourActivator).GetField("_enabledScriptsMap", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(behaviourActivator) as Dictionary<PlayerStateType, List<MonoBehaviour>>;
        var mainState = typeof(PlayerBehaviourActivator).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(behaviourActivator) as PlayerMainState;

        var currentStateType = mainState!.CurrentStateType;

        if (respectStateMap)
        {
            enabledBehavioursMap![currentStateType].ForEach(b =>
            {
                if (behaviourTypes.Contains(b.GetType())) b.enabled = state;
            });
        }
        else
        {
            behaviourTypes.ForEach(b =>
            {
                if (behaviourActivator.GetComponentInChildren(b) is not MonoBehaviour behaviour) return;

                behaviour.enabled = state;
                MelonLogger.Msg(behaviour);
            });
        }
    }

    private static List<Type> GetFlagBehaviourTypes(PlayerBehaviourFlags flags)
    {
        var types = new List<Type>();

        foreach (var mappingKey in _flagMappings.Keys)
        {
            MelonLogger.Msg(flags + " : " + mappingKey + " : " + flags.HasFlag(mappingKey));

            if (flags.HasFlag(mappingKey))
            {
               

                var type = _flagMappings[mappingKey];

                types.Add(type);
            }
        }

        types.ForEach(MelonLogger.Warning);

        return types;
    }


    private static readonly Dictionary<PlayerBehaviourFlags, Type> _flagMappings = new()
    {
        { PlayerBehaviourFlags.Input,                      typeof(PlayerInput) },

        { PlayerBehaviourFlags.WalkingControls,            typeof(PlayerWalkingControls) },
        { PlayerBehaviourFlags.CheatFlyControls,           typeof(PlayerCheatFlyControls) },
        { PlayerBehaviourFlags.LookControls,               typeof(PlayerLookControls) },
        { PlayerBehaviourFlags.ToolSelectionControls,      typeof(PlayerToolSelectionControls) },
        { PlayerBehaviourFlags.PrimaryActionControl,       typeof(PlayerPrimaryActionControl) },
        { PlayerBehaviourFlags.SecondaryActionControl,     typeof(PlayerSecondaryActionControl) },
        { PlayerBehaviourFlags.ScrollControl,              typeof(PlayerScrollControl) },
        { PlayerBehaviourFlags.InteractionControl,         typeof(PlayerInteractionControl) },
        { PlayerBehaviourFlags.EscapeControl,              typeof(PlayerEscapeControl) },
        { PlayerBehaviourFlags.PilotingControls,           typeof(PlayerPilotingControls) },
        { PlayerBehaviourFlags.InventoryControl,           typeof(PlayerInventoryControl) },
        { PlayerBehaviourFlags.LeverControl,               typeof(PlayerLeverControl) },
        { PlayerBehaviourFlags.LeavableObjectControl,      typeof(PlayerLeavableObjectControl) },
        { PlayerBehaviourFlags.ShiftHoldControl,           typeof(PlayerShiftHoldControl) },
        { PlayerBehaviourFlags.CheatControls,              typeof(PlayerCheatControls) },
        { PlayerBehaviourFlags.SwitchTabControls,          typeof(PlayerSwitchTabControls) },

        { PlayerBehaviourFlags.ObjectInteractionDetector,  typeof(PlayerObjectInteractionDetector) },
        { PlayerBehaviourFlags.PlugSocketDetector,         typeof(PlayerPlugSocketDetector) }
    };
}