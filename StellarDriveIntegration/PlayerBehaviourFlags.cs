using System;

namespace StellarModdingToolkit.StellarDriveIntegration;

/// <summary>
/// Represents various features of the StellarDrive Player
/// <summary/>
[Flags]
public enum PlayerBehaviourFlags : uint
{
    None                       = 0,

    Input                      = 1 << 0,

    WalkingControls            = 1 << 1,
    CheatFlyControls           = 1 << 2,
    LookControls               = 1 << 3,
    ToolSelectionControls      = 1 << 4,
    PrimaryActionControl       = 1 << 5,
    SecondaryActionControl     = 1 << 6,
    ScrollControl              = 1 << 7,
    InteractionControl         = 1 << 8,
    EscapeControl              = 1 << 9,
    PilotingControls           = 1 << 10,
    InventoryControl           = 1 << 11,
    LeverControl               = 1 << 12,
    LeavableObjectControl      = 1 << 13,
    ShiftHoldControl           = 1 << 14,
    CheatControls              = 1 << 15,
    SwitchTabControls          = 1 << 16,

    ObjectInteractionDetector  = 1 << 17,
    PlugSocketDetector         = 1 << 18,

    All                        = uint.MaxValue,
}
