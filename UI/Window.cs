using MelonLoader;
using System;
using UnityEngine;
using UnityEngine.UIElements;

using Cursor = UnityEngine.UIElements.Cursor;

namespace StellarModdingToolkit.UI;

internal class Window : ResizeableElement
{
    public readonly string Title;
    
    private readonly VisualElement _titlebar;
    private readonly VisualElement _contentContainer;
    
    
    public Window(string title, Action closeButtonAction)
    {
        Title = title;
        
        var body = CreateBody();
        Add(body);
        
        _titlebar = CreateToolbar(closeButtonAction);
        _contentContainer = CreateContentContainer();
        
        body.Add(_titlebar);
        body.Add(_contentContainer);
        
        AddToClassList("smtk-window");
        
        RegisterCallback<PointerMoveEvent>(OnPointerMove);
    }


    public override VisualElement contentContainer => _contentContainer ?? this;
    protected override VisualElement MoveHandle => _titlebar;

    
    private void OnPointerMove(PointerMoveEvent e)
    {
        if (CurrentResizeMode is ResizeMode.None)
        {
            ResizeMode resizeMode = GetResizeMode(e.localPosition);
            SetCursor(resizeMode);
        }
        else
        {
            SetCursor(CurrentResizeMode);
        }
    }
    

    private VisualElement CreateBody()
    {
        VisualElement body = new() { name = "smtk-body", pickingMode = PickingMode.Ignore };
        body.AddToClassList("smtk-window__body");
        
        return body;
    }

    private VisualElement CreateToolbar(Action closeButtonAction)
    {
        VisualElement titlebar = new() { name = "smtk-titlebar" };
        titlebar.AddToClassList("smtk-window__titlebar");

        Label title = new(Title) { name = "smtk-title", pickingMode = PickingMode.Ignore };
        title.AddToClassList("smtk-window__title");
        
        Button closeButton = new(closeButtonAction) { name = "smtk-close-button" };
        closeButton.AddToClassList("smtk-window__close-button");

        var key = StellarModdingToolkitPlugin.CrossSmallKey;
        var image = StellarModdingToolkitPlugin.AssetLoader?.GetAsset<Sprite>(key);
        closeButton.style.backgroundImage = image?.texture;
        
        titlebar.Add(title);
        titlebar.Add(closeButton);
        
        return titlebar;
    }
    
    private VisualElement CreateContentContainer()
    {
        VisualElement container = new() { name = "smtk-content-container" };
        container.AddToClassList("smtk-window__content-container");
        
        container.RegisterCallback<PointerDownEvent>(e => e.StopPropagation());
        
        return container;
    }


    private void SetCursor(ResizeMode resizeMode)
    {
        string horizontalKey   = StellarModdingToolkitPlugin.ResizeHorizontalCursorKey;
        string verticalKey     = StellarModdingToolkitPlugin.ResizeVerticalCursorKey;
        string diagonalUpKey   = StellarModdingToolkitPlugin.ResizeDiagonalUpCursorKey;
        string diagonalDownKey = StellarModdingToolkitPlugin.ResizeDiagonalDownCursorKey;
        
        Texture2D? horizontal   = StellarModdingToolkitPlugin.AssetLoader?.GetAsset<Texture2D>(horizontalKey);
        Texture2D? vertical     = StellarModdingToolkitPlugin.AssetLoader?.GetAsset<Texture2D>(verticalKey);
        Texture2D? diagonalUp   = StellarModdingToolkitPlugin.AssetLoader?.GetAsset<Texture2D>(diagonalUpKey);
        Texture2D? diagonalDown = StellarModdingToolkitPlugin.AssetLoader?.GetAsset<Texture2D>(diagonalDownKey);

        if (resizeMode is ResizeMode.None or ResizeMode.Move)
        {
            style.cursor = StyleKeyword.None;
            return;
        }
        
        Texture2D? texture = resizeMode switch
        {
            ResizeMode.Left => horizontal,
            ResizeMode.Right => horizontal,
            
            ResizeMode.Top => vertical,
            ResizeMode.Bottom => vertical,
            
            ResizeMode.TopLeft => diagonalDown,
            ResizeMode.BottomRight => diagonalDown,
            
            ResizeMode.TopRight => diagonalUp,
            ResizeMode.BottomLeft => diagonalUp,
            
            _ => throw new ArgumentOutOfRangeException()
        };


        if (texture is null)
        {
            texture = Texture2D.redTexture;
            MelonLogger.Error($"Failed loading cursor for ResizeMode: {resizeMode}");
        }


        style.cursor = new Cursor
        {
            texture = texture, 
            hotspot = new Vector2(texture.width/2f, texture.height/2f)
        };
    }
}