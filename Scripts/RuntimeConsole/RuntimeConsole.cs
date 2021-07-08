using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeConsole
{
    /* Runtime Console Static */

    static RuntimeConsole _instance = null;
    public static RuntimeConsole Instance { get { if(_instance == null && IsEnabled) Initialize(); return _instance; } }

    static string _prefix = "/";
    public static string Prefix => _prefix;

    static ConsoleCommand[] _commands = new ConsoleCommand[0];
    public static int CommandCount => _commands != null ? _commands.Length : 0;

    public static Font textFont;

    public static void SetCommands(string prefix, ConsoleCommand[] commands)
    {
        if(prefix != null)
            _prefix = prefix;

        if(_commands != null)
            _commands = commands;
    }

    public static ConsoleCommand Command(int i)
    {
        if(i < CommandCount)
            return _commands[i];
        return null;
    }

    static bool _isEnabled = true;
    public static bool IsEnabled
    {
        get
        {
            return _isEnabled;
        } set
        {
            if(_instance != null && Application.isPlaying && _isEnabled != value)
                if(value)
                    Enable();
                else
                    Disable();
            
            _isEnabled = value;
        }
    }
    
    public static bool Using { get; private set; }
    public static bool Mini { get; private set; }

    public static void Initialize()
    {
        if(_isEnabled)
            Enable();
        GUI.depth = 1;
    }

    static void Enable()
    {
        _instance = new RuntimeConsole();
        Application.logMessageReceived += _instance.OnLogMessageReceived;

        Debug.Log("Console Enabled");
    }

    static void Disable()
    {
        Using = false;
        Mini = false;

        if(_instance != null)
        {
            Application.logMessageReceived -= _instance.OnLogMessageReceived;
            _instance = null;
        }
    }

    /* Runtime Console Instance */

    public string this[int i] { get { return consoleQueue.ElementAt(i); } }

    Queue<string> consoleQueue;
    Queue<byte> colorQueue;
    public int Count => consoleQueue.Count;

    int _limit = 50;
    public int Limit { get { return _limit; } set { _limit = value < 1 ? 1 : value; Update(); } }

    Color _backgroundColor = new Color(0, 0, 0, 0.75f);
    public Color BackgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; UpdateBackground(); } }

    public Color NormalColor { get; set; } = UnityEngine.Color.white;
    public Color ErrorColor { get; set; } = UnityEngine.Color.red;
    public Color WarningColor { get; set; } = UnityEngine.Color.yellow;

    GUIStyle style;
    Texture2D background;

    Vector2 scroll = new Vector2(0, Mathf.Infinity);
    string inputString = "";

    RuntimeConsole()
    {
        consoleQueue = new Queue<string>();
        colorQueue = new Queue<byte>();

        background = new Texture2D(1, 1);
        UpdateBackground();

        style = new GUIStyle();
        style.normal.textColor = NormalColor;
        style.alignment = TextAnchor.LowerLeft;

        if(textFont != null)
            style.font = textFont;
    }

    void UpdateBackground()
    {
        background.SetPixels(new Color[]{ BackgroundColor });
        background.Apply();
    }

    public Color Color(int i)
    {
        switch(colorQueue.ElementAt(i))
        {
            case 1 :
                return ErrorColor;
            case 2 :
                return WarningColor;
            default :
                return NormalColor;
        }
    }

    public void Update()
    {
        for(int i = Count; Count > Limit; i--)
        {
            consoleQueue.Dequeue();
            colorQueue.Dequeue();
        }
    }

    public void Clear()
    {
        consoleQueue.Clear();
        colorQueue.Clear();
    }

    public void Input(string input, byte type = 0)
    {
        if(IsEnabled)
        {
            if(input.Length > 3 && input[0] == '#' && input[2] == ' ')
            {
                switch(input[1])
                {
                    case '0' :
                        colorQueue.Enqueue(0);
                        break;
                    case '1' :
                        colorQueue.Enqueue(1);
                        break;
                    case '2' :
                        colorQueue.Enqueue(2);
                        break;
                    default :
                        colorQueue.Enqueue(type);
                        break;
                }
                input = input.Substring(3);
            } else
            {
                colorQueue.Enqueue(type);
            }

            consoleQueue.Enqueue(input);
            Update();
            ResetScroll();
        } else if(input.Length > 3 && input[0] == '#' && input[2] == ' ')
        {
            input = input.Substring(3);
        }
        ProcessCommand(input);
    }

    public void Respond(string input, byte type = 0)
    {
        if(IsEnabled)
        {
            consoleQueue.Enqueue(input);
            colorQueue.Enqueue(type);
            Update();
        }
    }

    void ProcessCommand(string inputValue)
    {
        if(!inputValue.StartsWith(Prefix))
            return;

        inputValue = inputValue.Remove(0, Prefix.Length);

        string[] inputSplit = inputValue.Split(' ');

        string commandInput = inputSplit[0];
        string[] args = inputSplit.Skip(1).ToArray();

        ProcessCommand(commandInput, args);
    }

    void ProcessCommand(string commandInput, string[] args)
    {
        for(int i = 0; i < CommandCount; i++)
        {
            if(Command(i) != null)
            {
                if(!commandInput.Equals(Command(i).KeyWord, StringComparison.OrdinalIgnoreCase))
                    continue;

                if(Command(i).Process(args))
                    return;
            }
        }
        Respond("Invalid Command", 1);
    }

    public void ResetScroll()
    {
        scroll = new Vector2(0, Mathf.Infinity);
    }

    void NormalGUI()
    {
        style.fontSize = (int)Mathf.Max(Screen.height / 67.5f, 16f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height / 2f), background);
        scroll = GUILayout.BeginScrollView(scroll, style, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height / 2f - style.fontSize * 1.1f));

        GUILayout.FlexibleSpace();
        for(int i = 0; i < Count; i++)
        {
            style.normal.textColor = Color(i);
            GUILayout.Label("# " + this[i], style);
        }
        style.normal.textColor = NormalColor;
        GUILayout.EndScrollView();

        if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
        {
            Input(inputString);
            inputString = "";
        }

        GUI.SetNextControlName("ConsoleInput");
        inputString = GUILayout.TextField(inputString, style);
        
        if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.BackQuote)
            GUI.FocusControl("ConsoleInput");
        else if(Event.current.keyCode == KeyCode.BackQuote)
            GUI.FocusControl("");
    }

    void MiniGUI()
    {
        style.fontSize = (int)Mathf.Max(Screen.height / 67.5f, 16f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, style.fontSize * 2.1f), background);
        GUILayout.BeginScrollView(new Vector2(0, Mathf.Infinity), style, GUILayout.Width(Screen.width), GUILayout.Height(style.fontSize * 2.1f));

        GUILayout.FlexibleSpace();
        for(int i = Count < 2 ? 0 : Count - 2; i < Count; i++)
        {
            style.normal.textColor = Color(i);
            GUILayout.Label("# " + this[i], style);
        }
        style.normal.textColor = NormalColor;
        GUILayout.EndScrollView();
    }

    void OnLogMessageReceived(string logString, string stackTrace, LogType logType)
    {
        try
        {
            int traceStart = stackTrace.IndexOf("(at ");
            int traceEnd = stackTrace.IndexOf("\n", traceStart);
            string trace = stackTrace.Substring(traceStart, traceEnd - traceStart);
            string response = "[" + logType + "] " + logString + " " + trace;
            byte type = 0;
            switch(logType)
            {
                case LogType.Error :
                    type = 1;
                    break;
                case LogType.Assert :
                    type = 1;
                    break;
                case LogType.Exception :
                    type = 1;
                    break;
                case LogType.Warning :
                    type = 2;
                    break;
                default :
                    type = 0;
                    break;
            }
            Respond(response, type);
        } catch(Exception exception)
        {
            Respond("[" + LogType.Error + "] " + exception + " (from Console on Receiving Log Message)", 1);
        }
    }

    public void OnGUI()
    {
        bool wasEnabled = GUI.enabled;
        GUI.depth = 0;
        GUI.enabled = true;

        if(Using)
            NormalGUI();
        else if(Mini)
            MiniGUI();

        if(IsEnabled && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.BackQuote)
        {
            if(Mini)
            {
                Mini = false;
            } else
            {
                Using = !Using;
                ResetScroll();
                if(!Using)
                    Mini = true;
            }
        }

        GUI.depth = 1;
        GUI.enabled = wasEnabled;
    }
}