# Stellar Modding Toolkit (SMTK)

Provides various utilities for the creation of StellarDrive mods with MelonLoader.


## 🚀 Getting Started

### Requirements
- MelonLoader installed
- Already setup MelonLoader mod project

### Installing

Install the `SMTK.dll` and place it in the directory: `\StellarDrive\Plugin`.
<br/>
You'll also need to add a dependency to `SMTK.dll`, as well as to `UnityEngine.CoreModule.dll` and `UnityEngine.UIElementsModule.dll`.
<br/>
<br/>
Those are most likely not the only dlls you'll use but they're the only required ones to fully use SMTK.

### Hub example

The Hub is an overlay mods can use to easily display custom ✨resizable✨ windows.
<br/>
So lets create a new one!
```cs
public class ExampleHubWindow : HubWindow
{
    public override string Name => "Example"; // Sets the name/header on the top of the window

    public override Vector2 MinSize => new(300, 0);   // The height can't get smaller than its menubar so 0 is fine
    public override Vector2 MaxSize => new(500, 500); // Sets max width and height

    protected override VisualElement CreateContent()
    {
        var textField = new TextField() { label = "Example Input" }; // Create text field
        var button = new Button() { text = "Print Message" }; // Create button

        var container = new ScrollView(); // Lets you scroll through the content if the window's too small
        container.AddRange([textField, button]); // Add content as children (the order matters)

        button.clicked += () => // triggered after button click
        {
            MelonLogger.Msg($"Button clicked! Current Input: {textField.text}"); // Print input in ML Console
        };

        return container; // Return finished content
    }
}
```
If you tried the mod in this state you'd see... nothing? <br/>
That's because we havent actually added one (ore more) to the Hub yet. <br/>
<br/>
You could do that at any point in time but since we always want access to our window we'll create it right at the start of the game. <br/>
The best place to this is in 'OnInitializeMelon', when the Hub hasn't even been created yet. <br/>

```cs
public class HubExampleMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        SMTK.OnCreatedHub += (s, e) => // Called right after Hub creation
        {
            SMTK.Hub?.AddHubWindow(new ExampleHubWindow()); // Adds our window (not limited to a single window)
        };
    }
}
```

## ❓ Help

Any advise for common problems or issues.
```
command to run if program contains helper info
```
