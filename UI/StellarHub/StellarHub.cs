using MelonLoader;
using StellarModdingToolkit.StellarDriveIntegration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

using Object = UnityEngine.Object;

namespace StellarModdingToolkit.UI.StellarHub;

public class StellarHub 
{
    private GameObject? _uiGameObject;
    private VisualElement _container;
    private StellarHubToolbar _toolbar;
    
    private readonly Dictionary<StellarHubWindow, Window> _windows = [];

    private bool _isVisible = false;
    
    
    internal StellarHub(PanelSettings panelSettings)
    {
        InitializePanelSettings(panelSettings);
        CreateDocument();
        
        _container = CreateContainer();
        _toolbar = CreateToolbar();


        var binding = Melon<StellarModdingToolkitPlugin>.Instance.ToggleHubInputBinding?.Value;
        InputAction input = new(binding: binding);

        input.performed += _ => ToggleVisibility();
        input.Enable();

        IsVisible = false;
    }


    /// <summary>
    /// The StellarHub's PanelSettings
    /// (UITK needs this)
    /// </summary>
    public static PanelSettings? PanelSettings { get; private set; }

    /// <summary>
    /// The StellarHub's UIDocument
    /// (Contains the UI)
    /// </summary>
    public static UIDocument? UIDocument { get; private set; }


    /// <summary>
    /// Every Window in the StellarHub as StellarHubWindow
    /// </summary>
    public ReadOnlyCollection<StellarHubWindow> Windows => new(_windows.Keys?.ToArray());


    /// <summary>
    /// Called once a StellarHubWindow has been added
    /// </summary>
    public EventHandler<StellarHubWindow>? OnAddedHubWindow { get; private set; }

    /// <summary>
    /// Called once a StellarHubWindow has been removed
    /// </summary>
    public EventHandler<StellarHubWindow>? OnRemovedHubWindow { get; private set; }

    /// <summary>
    /// Indicates wheter the StellarHub is visible/displayed
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
                IntegrationUtilities.SetBehaviourStates(PlayerBehaviourFlags.All, false);
                IntegrationUtilities.SetBehaviourStates(PlayerBehaviourFlags.Input | PlayerBehaviourFlags.EscapeControl, true);
                var imposter = IntegrationUtilities.CreateMenuImposter<ClosableMenuImposter>(this);

                if (imposter is not null) imposter.OnClose += (_, _)  => IsVisible = false;
            }
            else
            {
                IntegrationUtilities.SetBehaviourStates(PlayerBehaviourFlags.All, true);
                IntegrationUtilities.DestroyAllMenuImposters(this);
            }
        }
    } 


    public void ToggleVisibility()
    {
        IsVisible = !IsVisible;
    }


    /// <summary>
    /// Adds/Creates a new Window for the StellarHub
    /// </summary>
    public void AddHubWindow(StellarHubWindow hubWindow)
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
    /// Removes the corresponding Window from the StellarHub
    /// </summary>
    public void RemoveHubWindow(StellarHubWindow hubWindow)
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
    }
    

    private void CreateDocument()
    {
        _uiGameObject = new GameObject("StellarHub");
        Object.DontDestroyOnLoad(_uiGameObject);
        
        UIDocument = _uiGameObject.AddComponent<UIDocument>();
        UIDocument.panelSettings = PanelSettings;

        var styleSheetAssetKeys = new[]
        {
            StellarModdingToolkitPlugin.Keys.MiscellaneousStyleSheet,
            StellarModdingToolkitPlugin.Keys.TextInputFieldsStyleSheet,
            StellarModdingToolkitPlugin.Keys.CompositeFieldsStyleSheet,
            StellarModdingToolkitPlugin.Keys.ButtonsStyleSheet,
            StellarModdingToolkitPlugin.Keys.TogglesStyleSheet,
            StellarModdingToolkitPlugin.Keys.SlidersStyleSheet,
            StellarModdingToolkitPlugin.Keys.EnumsStyleSheet,
            StellarModdingToolkitPlugin.Keys.DropdownsStyleSheet,
            StellarModdingToolkitPlugin.Keys.ProgressBarsStyleSheet,
            StellarModdingToolkitPlugin.Keys.BoundsFieldsStyleSheet,
            StellarModdingToolkitPlugin.Keys.WindowsStyleSheet,
            StellarModdingToolkitPlugin.Keys.ScrollersStyleSheet,
            StellarModdingToolkitPlugin.Keys.StellarHubToolbarStyleSheet
        };

        foreach (var key in styleSheetAssetKeys)
        {
            StyleSheet? styleSheet = StellarModdingToolkitPlugin.AssetLoader?.GetAsset<StyleSheet>(key);
            
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

    private StellarHubToolbar CreateToolbar()
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
        StellarHubToolbar toolbar = new();

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