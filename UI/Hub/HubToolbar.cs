using System.Collections.Generic;
using UnityEngine.UIElements;

namespace StellarModdingToolkit.UI.Hub;

internal class HubToolbar : VisualElement
{
    public HubToolbar()
    {
        AddToClassList("smtk-hub-toolbar");
    }

    private Dictionary<HubWindow, Window> _items = [];
    public Dictionary<HubWindow, Window> Items 
    { 
        get => _items; 
        set 
        {
            if (value is not null) _items = value;
        } 
    }
    
    
    public void RebulidList()
    {
        Clear();

        foreach (var window in Items.Keys)
        {
            var element = CreateListElement(window);
            
            Add(element);
        }
    }
    
    
    private VisualElement CreateListElement(HubWindow window)
    {
        Button button = new()
        {
            text = window.Name
        };

        
        button.clicked += () =>
        {
            window.IsVisible = !window.IsVisible;
        };

        window.OnVisibilityChanged += (_, isVisible) =>
        {
            Items[window].visible = isVisible;
            
            if(isVisible) Items[window].BringToFront();
            
            UpdateButtonClassList(button, isVisible);
        };
        
        
        button.AddToClassList("smtk-hub-toolbar__element");

        UpdateButtonClassList(button, window.IsVisible);

        
        return button;
    }

    private void UpdateButtonClassList(Button button, bool isVisible)
    {
        if (!isVisible)
        {
            button.RemoveFromClassList("smtk-hub-toolbar__element-unchecked");
            button.AddToClassList("smtk-hub-toolbar__element-checked");
        }
        else
        {
            button.RemoveFromClassList("smtk-hub-toolbar__element-checked");
            button.AddToClassList("smtk-hub-toolbar__element-unchecked");
        }
    }
}