# Stellar Modding Toolkit (SMTK)

Provides various utilities for the creation of StellarDrive mods with MelonLoader.

## 🗒️ Description

SMTK offers utilities for StellarDriv MelonLoadere mods, including:
- UITK support, including custom runtime theme and panel settings bundle
- The Hub, an overlay menu mods can use to display custom resizable windows with ease
- AssetBundleLoader, it loads asset bundles...

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

The `Hub` is an overlay mods can use to easily display custom ✨resizable✨ windows.
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
That's because we havent actually added a window (ore more) to the `Hub` yet. <br/>
<br/>
You could do that at any point in time but since we always want access to our window we'll create it right at the start of the game. <br/>
The best place to this is in 'OnInitializeMelon', when the Hub hasn't even been created yet. <br/>
Only use `OnCreatedHub` when calling `AddHubWindow` this early, if you use it after its creation your window won't be added.

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
After starting the game and opening the hub (F12 by default) you'll now see your very own window. 🎉 <br/>
![image](https://github.com/user-attachments/assets/d644f66c-c4b1-4199-9888-eb2f679b84ad) <br/>
Not happy with how a window looks? :( <br/>
Then use the `AssetLoader` to import some custom style sheets!


## ⚙️ Changing the Hub Keybind

You can switch to another Keybind by editing `ToggleHubInputBinding` in `\StellarDrive\UserData\MelonPreferences.cfg`. <br/>
The default value is `"<Keyboard>/f12"`. <br/> 
<br/>
If you wanted to use F11 instead of F12 you'd simply have to replace the default with `"<Keyboard>/f11"`.



## ❓ Help

If you have any issues or questions, feel free to reach out to me on the <a href="https://discord.gg/6KAvq3S9ZW">Discord server</a>!
