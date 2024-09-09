using System;
using UI.Interface.Model;

namespace StellarModdingToolkit.StellarDriveIntegration;

/// <summary>
/// Provides same functionality as its base class
/// Also implements IClosableMenu so the game can close mod menus 
/// </summary>
public class ClosableMenuImposter : MenuImposter, IClosableMenu
{
    internal ClosableMenuImposter() { }

    /// <summary>
    /// Called when something tries closing the menu
    /// (Should be utilised to close menu)
    /// </summary>
    public EventHandler? OnClose;

    /// <summary>
    /// Should be utilised to close menu
    /// </summary>
    public void Close()
    {
        OnClose?.Invoke(this, EventArgs.Empty);
    }
}
