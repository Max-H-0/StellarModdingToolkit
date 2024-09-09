using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace StellarModdingToolkit.UI.StellarHub;

/// <summary>
/// Base for Windows the StellarHub can display
/// </summary>
public abstract class StellarHubWindow
{
    private bool _isVisible = true;
    
    private VisualElement? _content;

    /// <summary>
    /// Name and Header of the Window
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Min Size of the Window's Content
    /// </summary>
    public abstract Vector2 MinSize { get; }

    /// <summary>
    /// Min Size of the Window's Content
    /// </summary>
    public abstract Vector2 MaxSize { get; }

    /// <summary>
    /// UI-Content of the Window
    /// </summary>
    public VisualElement Content => _content ??= CreateContent();

    /// <summary>
    /// Indicates wheter the Window is visible/toggled
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                OnVisibilityChanged?.Invoke(this, value);
            }
        }
    }


    /// <summary>
    /// Use this to create the Window's content aswell as its logic
    /// </summary>
    protected abstract VisualElement CreateContent();

    /// <summary>
    /// Called after visibility changed
    /// </summary>
    public event EventHandler<bool>? OnVisibilityChanged;
}