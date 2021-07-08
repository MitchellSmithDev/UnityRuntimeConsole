# Plug 'n Play Runtime Console for Unity
This is an easy to use, modular, fully featured, runtime developer console made for Unity. It is meant to be an extension of Unity's built-in console along with giving developers easier access to debugging commands and features.

## Installation
1. Download and import the latest release from this repository
2. Add `_Game` scene as the first scene in the build
3. Open `_Game` scene and select the `SceneRedirect` gameobject
4. Drag and drop your first scene (make sure it's also added to the build beforhand) into the `RedirectToScene` property
5. Play `_Game` or build the project to use the Runtime Console

## How To Use
Press the `backquote key` once to open the console.<br />
Pressing it again will minimize the console without fully closing it.<br />
Pressing it for a third time will fully close it.<br />
While the console is open, and not minimized, simply type out your command with the keyboard and press enter.<br />
By default, the command prefix is `/`, but this can be changed in the `GameManager.prefab` properties.

## How to Add New Commands
First, create a script called `TemplateCommand.cs` but replace `Template` with whatever your command is called. I recommend saving this script to the folder `Scripts/Commands`.<br />
Next delete everything and fill out the script with this template, replacing `Template` like before.<br />

```c#
using UnityEngine;

[CreateAssetMenu(fileName = "TemplateCommand", menuName = "Commands/Template")]
public class TemplateCommand : ConsoleCommand
{
    string _keyWord = "template";
    public override string KeyWord => _keyWord;
    
    public override bool Process(string[] args)
    {
        //Insert Code Here
        //Return true if the command was successful, such as having valid arguments
        //Or false if it wasn't, which will say "Invalid Command" in the console
        return true;
    }
}
```

After writing your script for the command, you'll need to create a new asset file and add it to the command array.<br />
In the top toolbar, click `Assets/Create/Commands/Template` (swap out `Template` like before) and save the asset file. I recommend saving it in the folder `Prefabs/Commands`.<br />
Finally, click on the `GameManager.prefab` found in the `Prefabs` folder then drag and drop the command asset file into the `Commands` array property.<br />
To use your command, simply run the project and open the console and enter your command with the command prefix before it, by default it is `/`, and press enter..<br />
*Note: Arguments are separated by spaces*

## Documentation
By adding `#0`, `#1`, or `#2` at the start of commands, the console will color that line as normal, an error, or a warning respectively.

#### GameManager.Instance
`public static GameManager Instance`<br />
The singular instance of the Game Manager. (Read Only)

#### RuntimeConsole.Instance
`public static RuntimeConsole Instance`<br />
The singular instance of the console used to assist debugging and developing. (Read Only)

#### RuntimeConsole.Prefix
`public static string Prefix`<br />
The prefix that all commands start with when entered into the console. (Read Only)

#### RuntimeConsole.CommandCount
`public static int CommandCount`<br />
Returns how many commands are available. (Read Only)

#### RuntimeConsole.textFont
`public static Font textFont`<br />
The font for all text seen in the console. By default it is `Arial`.

#### RuntimeConsole.SetCommands()
`public static void SetCommands(string prefix, ConsoleCommand[] commands)`<br />

#### RuntimeConsole.Command()
`public static ConsoleCommand Command(int i)`<br />
Returns a command variable from the array of available commands.

#### RuntimeConsole.IsEnabled
`public static bool IsEnabled`<br />
Boolean to enable or disable the developer console. By default it is `true`.

#### RuntimeConsole.Using
`public static bool Using`<br />
Boolean that shows if the console is open. (Read Only)

#### RuntimeConsole.Mini
`public static bool Mini`<br />
Boolean that shows if the console is minimized. (Read Only)

#### RuntimeConsole.Initialize()
`public static void Initialize()`<br />

#### RuntimeConsole.Instance[]
`public string this[int i]`<br />
Returns a line stored in the console. (Read Only)

#### RuntimeConsole.Instance.Count
`public int Count`<br />
The amount of lines currently stored in the console. (Read Only)

#### RuntimeConsole.Instance.Limit
`public int Limit`<br />
The limit to how many lines can be stored in the console. By default it is `50`.

#### RuntimeConsole.Instance.BackgroundColor
`public Color BackgroundColor`<br />
The background color for the console whenever it is open or minimized. By default it is `Color(0, 0, 0, 0.75f)`.

#### RuntimeConsole.Instance.NormalColor
`public Color NormalColor`<br />
The normal text color for lines in the console. By default it is `Color.white`.

#### RuntimeConsole.Instance.ErrorColor
`public Color ErrorColor`<br />
The text color of errors for lines in the console. By default it is `Color.red`.

#### RuntimeConsole.Instance.WarningColor
`public Color WarningColor`<br />
The text color of warning for lines in the console. By default it is `Color.yellow`.

#### RuntimeConsole.Instance.Color()
`public Color Color(int i)`<br />
Returns the text color of the line stored in the console.

#### RuntimeConsole.Instance.Update()
`public void Update()`<br />
Updates the console's line queues to set them to the current limit.

#### RuntimeConsole.Instance.Clear()
`public void Clear()`<br />
Clears all lines from the console.

#### RuntimeConsole.Instance.Input()
`public void Input(string input, byte type = 0)`<br />
Sends an input into the console which is then parsed into a command. Type can be set to `0`, `1`, or `2` which sets the line as normal, an error, or a warning respectively.

#### RuntimeConsole.Instance.Respond()
`public void Respond(string input, byte type = 0)`<br />
Sends a line into the console which is not parsed. Type can be set to `0`, `1`, or `2` which sets the line as normal, an error, or a warning respectively.

#### RuntimeConsole.Instance.ResetScroll()
`public void ResetScroll()`<br />
Resets the console scroll.

#### RuntimeConsole.Instance.OnGUI()
`public void OnGUI()`<br />
The general GUI function for the console. **It is not intended for use.**