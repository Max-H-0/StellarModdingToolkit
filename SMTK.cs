using System;
using UnityEngine.UIElements;
using MelonLoader;
using StellarModdingToolkit.Assets;
using StellarModdingToolkit.UI.Hub;
using StellarModdingToolkit.StellarDriveIntegration;

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
    public struct Keys
    {
        public const string PanelSettings = "panel-settings";

        public const string CrossSmall = "cross-small";

        public const string ResizeHorizontalCursor = "resize-horizontal-cursor";
        public const string ResizeVerticalCursor = "resize-vertical-cursor";
        public const string ResizeDiagonalUpCursor = "resize-diagonal-up-cursor";
        public const string ResizeDiagonalDownCursor = "resize-diagonal-down-cursor";

        public const string MiscellaneousStyleSheet = "miscellaneous-style-sheet";
        public const string TextInputFieldsStyleSheet = "text-input-fields-style-sheet";
        public const string CompositeFieldsStyleSheet = "composite-fields-style-sheet";
        public const string ButtonsStyleSheet = "buttons-style-sheet";
        public const string TogglesStyleSheet = "toggles-style-sheet";
        public const string SlidersStyleSheet = "sliders-style-sheet";
        public const string EnumsStyleSheet = "enums-style-sheet";
        public const string DropdownsStyleSheet = "dropdowns-style-sheet";
        public const string ProgressBarsStyleSheet = "progress-bars-style-sheet";
        public const string BoundsFieldsStyleSheet = "bounds-fields-style-sheet";
        public const string WindowsStyleSheet = "windows-style-sheet";
        public const string ScrollersStyleSheet = "scrollers-style-sheet";
        public const string HubToolbarStyleSheet = "stellar-hub-toolbar-style-sheet";
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

        AssetLoader = new(MelonAssembly.Assembly, LoggerInstance,
        [
            Keys.PanelSettings,

            Keys.CrossSmall,

            Keys.ResizeHorizontalCursor,
            Keys.ResizeVerticalCursor,
            Keys.ResizeDiagonalUpCursor,
            Keys.ResizeDiagonalDownCursor,

            Keys.MiscellaneousStyleSheet,
            Keys.TextInputFieldsStyleSheet,
            Keys.CompositeFieldsStyleSheet,
            Keys.ButtonsStyleSheet,
            Keys.TogglesStyleSheet,
            Keys.SlidersStyleSheet,
            Keys.EnumsStyleSheet,
            Keys.DropdownsStyleSheet,
            Keys.ProgressBarsStyleSheet,
            Keys.BoundsFieldsStyleSheet,
            Keys.WindowsStyleSheet,
            Keys.ScrollersStyleSheet,
            Keys.HubToolbarStyleSheet
        ]);
        OnCreatedAssetLoader?.Invoke(this, EventArgs.Empty);


        var panelSettings = AssetLoader.GetAsset<PanelSettings>(Keys.PanelSettings);


        if (panelSettings is null)
        {
            LoggerInstance.Error("Failed loading PanelSettings!");

            throw new ArgumentNullException(nameof(panelSettings));
        }


        Hub = new Hub(panelSettings);
        OnCreatedHub?.Invoke(this, EventArgs.Empty);
    }
}