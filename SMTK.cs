using System;
using UnityEngine.UIElements;
using MelonLoader;
using StellarModdingToolkit.Assets;
using StellarModdingToolkit.UI.Hub;

namespace StellarModdingToolkit;

/// <summary>
/// Responsible for Hub creation
/// (also provides some base Assets)
/// </summary>
public class SMTK : MelonPlugin
{
    /// <summary>
    /// Provides some base Assets
    /// (you probably don't need this)
    /// </summary>
    public static AssetLoader? AssetLoader { get; private set; }

    /// <summary>
    /// Manages UI Windows
    /// </summary>
    public static Hub? Hub { get; private set; }


    /// <summary>
    /// Called once the AssetLoader has been created
    /// </summary>
    public static EventHandler? OnCreatedAssetLoader;

    /// <summary>
    /// Called once the Hub has been created
    /// </summary>
    public static EventHandler? OnCreatedHub;


    /// <summary>
    /// Primary MelonPreferences_Category
    /// </summary>
    public MelonPreferences_Category? PreferenceCategory { get; set; }

    /// <summary>
    /// InputBinding that's used to toggle the Hub
    /// </summary>
    public MelonPreferences_Entry<string>? ToggleHubInputBinding { get; set; }


    /// <summary>
    /// Contains the names of the base Assets
    /// (required to load/access them with the AssetLoader)
    /// </summary>
    [AssetKeyCollection]
    public struct Keys
    {
        [AssetKey] 
        public const string PanelSettings = "panel-settings";


        [AssetKey] 
        public const string CrossSmall = "cross-small";


        [AssetKeyCollection]
        public struct ResizeCursors
        {
            public const string Horizontal = "resize-horizontal-cursor";
            public const string Vertical = "resize-vertical-cursor";
            public const string DiagonalUp = "resize-diagonal-up-cursor";
            public const string DiagonalDown = "resize-diagonal-down-cursor";
        }


        [AssetKeyCollection]
        public struct StyleSheets
        {
            public const string Miscellaneous = "miscellaneous-style-sheet";
            public const string TextInputFields = "text-input-fields-style-sheet";
            public const string CompositeFields = "composite-fields-style-sheet";
            public const string Buttons = "buttons-style-sheet";
            public const string Toggles = "toggles-style-sheet";
            public const string Sliders = "sliders-style-sheet";
            public const string Enums = "enums-style-sheet";
            public const string Dropdowns = "dropdowns-style-sheet";
            public const string ProgressBars = "progress-bars-style-sheet";
            public const string BoundsFields = "bounds-fields-style-sheet";
            public const string Windows = "windows-style-sheet";
            public const string Scrollers = "scrollers-style-sheet";
            public const string HubToolbar = "stellar-hub-toolbar-style-sheet";
        }
    }


    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();

        PreferenceCategory = MelonPreferences.CreateCategory("SMTK");
        ToggleHubInputBinding = PreferenceCategory.CreateEntry("ToggleHubInputBinding", "<Keyboard>/f12");
    }


    public override void OnLateInitializeMelon()
    {
        base.OnLateInitializeMelon();

        AssetLoader = new(MelonAssembly.Assembly, LoggerInstance, AssetUtilities.ExtractAssetKeysFrom(typeof(Keys)));
        OnCreatedAssetLoader?.Invoke(this, EventArgs.Empty);


        var panelSettings = AssetLoader.GetAsset<PanelSettings>(Keys.PanelSettings)
                                       .Expect("Failed loading PanelSettings!");

        Hub = new Hub(panelSettings);
        OnCreatedHub?.Invoke(this, EventArgs.Empty);
    }
}