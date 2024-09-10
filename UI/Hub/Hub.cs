using MelonLoader;
using Player.Interface.Model;
using StellarModdingToolkit.StellarDriveIntegration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

using Object = UnityEngine.Object;

namespace StellarModdingToolkit.UI.Hub;

/// <summary>
/// Used to show easily display custom windows
/// </summary>
public class Hub 
{
    private GameObject? _uiGameObject;
    private VisualElement _container;
    private HubToolbar _toolbar;
    
    private readonly Dictionary<HubWindow, Window> _windows = [];

    private bool _isVisible = false;
    private PlayerBehaviourFlags _savedBehaviours;

    
    internal Hub(PanelSettings panelSettings)
    {
        InitializePanelSettings(panelSettings);
        CreateDocument();

        _container = CreateContainer();
        _toolbar = CreateToolbar();


        var toggleBinding = Melon<SMTK>.Instance.ToggleHubInputBinding?.Value;
        InputAction toggleInput = new(binding: toggleBinding);
        toggleInput.performed += _ => TryToggle();
        toggleInput.Enable();

        InputAction closeInput = new(binding: "<Keyboard>/escape");
        closeInput.performed += _ => IsVisible = false;
        closeInput.Enable();

        IsVisible = false;
    }


    /// <summary>
    /// The Hub's PanelSettings
    /// (UITK needs this)
    /// </summary>
    public static PanelSettings? PanelSettings { get; private set; }

    /// <summary>
    /// The Hub's UIDocument
    /// (Contains the UI)
    /// </summary>
    public static UIDocument? UIDocument { get; private set; }


    /// <summary>
    /// Every Window in the Hub as HubWindow
    /// </summary>
    public ReadOnlyCollection<HubWindow> Windows => new(_windows.Keys?.ToArray());


    /// <summary>
    /// Called once a HubWindow has been added
    /// </summary>
    public EventHandler<HubWindow>? OnAddedHubWindow { get; private set; }

    /// <summary>
    /// Called once a HubWindow has been removed
    /// </summary>
    public EventHandler<HubWindow>? OnRemovedHubWindow { get; private set; }

    /// <summary>
    /// Indicates wheter the Hub is visible/displayed
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;

            var displayStyle = _isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            _container.style.display = displayStyle;
            _toolbar.style.display = displayStyle;

            if (_isVisible)
            {
                _savedBehaviours = IntegrationUtilities.GetCurrentBehaviourStates();
                IntegrationUtilities.SetBehaviourStates(PlayerBehaviourFlags.Input | PlayerBehaviourFlags.EscapeControl);

                var imposter = IntegrationUtilities.CreateMenuImposter<ClosableMenuImposter>(this);

                if (imposter is not null) imposter.OnClose += (_, _)  => IsVisible = false;
            }
            else
            {
                IntegrationUtilities.SetBehaviourStates(_savedBehaviours);
                IntegrationUtilities.DestroyAllMenuImposters(this);
            }
        }
    } 


    public void TryToggle()
    {
        if (IntegrationUtilities.GetCurrentPlayerStateType() is PlayerStateType.WritingBeaconName) return;

        IsVisible = !IsVisible;
    }


    /// <summary>
    /// Adds/Creates a new Window for the Hub
    /// </summary>
    public void AddHubWindow(HubWindow hubWindow)
    {
        const int resizeHandleWidth = 15;
        
        Window window = new(hubWindow.Name, () => hubWindow.IsVisible = false)
        {
            style =
            {
                position = Position.Absolute,
                left = 500,
                right = 500,
                top = 300,
                bottom = 300,
                
                paddingLeft = resizeHandleWidth,
                paddingRight = resizeHandleWidth,
                paddingTop = resizeHandleWidth,
                paddingBottom = resizeHandleWidth,
                
                borderTopLeftRadius = 10,
                borderTopRightRadius = 10,
                borderBottomLeftRadius = 10,
                borderBottomRightRadius = 10,
            },
            ResizeHandleWidth = resizeHandleWidth,
            visible = hubWindow.IsVisible
        };
        
        window.Add(hubWindow.Content);
        _container.Add(window);

        void setMinMax(GeometryChangedEvent _)
        {
            float horizontalPadding = window.layout.width - window.contentContainer.layout.width;
            float verticalPadding = window.layout.height - window.contentContainer.layout.height;

            window.style.minWidth = hubWindow.MinSize.x + horizontalPadding;
            window.style.minHeight = hubWindow.MinSize.y + verticalPadding;

            window.style.maxWidth = hubWindow.MaxSize.x + horizontalPadding;
            window.style.maxHeight = hubWindow.MaxSize.y + verticalPadding;

            window.UnregisterCallback((EventCallback<GeometryChangedEvent>?)setMinMax);
        }

        window.RegisterCallback((EventCallback<GeometryChangedEvent>?)setMinMax);
        
        _windows.Add(hubWindow, window);
        OnAddedHubWindow?.Invoke(this, hubWindow);
    }

    /// <summary>
    /// Removes the corresponding Window from the HubWindow
    /// </summary>
    public void RemoveHubWindow(HubWindow hubWindow)
    {
        Window window = _windows[hubWindow];
        
        _container.Remove(window);
        _windows.Remove(hubWindow);
        
        OnRemovedHubWindow?.Invoke(this, hubWindow);
    }
    
    
    private void InitializePanelSettings(PanelSettings panelSettings)
    {
        //TODO: Apply changes
        PanelSettings = panelSettings;
        PanelSettings.sortingOrder = 100;
    }
    

    private void CreateDocument()
    {
        _uiGameObject = new GameObject("StellarHub");
        Object.DontDestroyOnLoad(_uiGameObject);
        
        UIDocument = _uiGameObject.AddComponent<UIDocument>();
        UIDocument.panelSettings = PanelSettings;

        var styleSheetAssetKeys = new[]
        {
            SMTK.Keys.MiscellaneousStyleSheet,
            SMTK.Keys.TextInputFieldsStyleSheet,
            SMTK.Keys.CompositeFieldsStyleSheet,
            SMTK.Keys.ButtonsStyleSheet,
            SMTK.Keys.TogglesStyleSheet,
            SMTK.Keys.SlidersStyleSheet,
            SMTK.Keys.EnumsStyleSheet,
            SMTK.Keys.DropdownsStyleSheet,
            SMTK.Keys.ProgressBarsStyleSheet,
            SMTK.Keys.BoundsFieldsStyleSheet,
            SMTK.Keys.WindowsStyleSheet,
            SMTK.Keys.ScrollersStyleSheet,
            SMTK.Keys.HubToolbarStyleSheet
        };

        foreach (var key in styleSheetAssetKeys)
        {
            StyleSheet? styleSheet = SMTK.AssetLoader?.GetAsset<StyleSheet>(key);
            
            UIDocument.rootVisualElement.styleSheets.Add(styleSheet);
        }
    }

    private VisualElement CreateContainer()
    {
        var container = new VisualElement
        {
            name = "smtk-stellar-hub",
            style =
            {
                position = Position.Absolute,
                width = new Length(100, LengthUnit.Percent),
                height = new Length(100, LengthUnit.Percent),
                
                backgroundColor = new Color(0, 0, 0 ,0.5f)
            }
        };
        
        UIDocument?.rootVisualElement.Add(container);
        return container;
    }

    private HubToolbar CreateToolbar()
    {
        VisualElement absoluteContainer = new()
        {
            name = "smtk-stellar-hub__toolbar",
            style =
            {
                position = Position.Absolute,
                width = new Length(100, LengthUnit.Percent),
                height = new Length(100, LengthUnit.Percent)
            }, 
            pickingMode = PickingMode.Ignore
        };
        HubToolbar toolbar = new();

        OnAddedHubWindow += (s, e) =>
        {
            toolbar.Items = _windows;
            toolbar.RebulidList();
        };
        
        OnRemovedHubWindow += (s, e) =>
        {
            toolbar.Items = _windows;
            toolbar.RebulidList();
        };
            
        absoluteContainer.Add(toolbar);
        
        UIDocument?.rootVisualElement.Add(absoluteContainer);
        return toolbar;
    }
}