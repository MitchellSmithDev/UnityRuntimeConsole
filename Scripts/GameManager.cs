using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Instantiate();
        } else if(Instance != this)
        {
            Destroy(gameObject);
            throw new System.InvalidOperationException("Another Game Manager instance already exists, destroying this instance.");
        }
    }

    void Instantiate()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if(_consoleEnabled)
            EnableConsole();
        GUI.depth = 1;
    }

    static bool _consoleEnabled = true;
    public static bool ConsoleEnabled
    {
        get
        {
            return _consoleEnabled;
        } set
        {
            if(Application.isPlaying && _consoleEnabled != value)
                if(value)
                    EnableConsole();
                else
                    DisableConsole();
            
            _consoleEnabled = value;
        }
    }

    public static bool hideCursor = false;
    public static bool UsingConsole { get; private set; }
    public static bool MiniConsole { get; private set; }

    [SerializeField] private string _commandPrefix = "/";
    public static string CommandPrefix => Instance._commandPrefix;

    [SerializeField] ConsoleCommand[] _commands = new ConsoleCommand[0];
    static ConsoleCommand[] commands => Instance._commands;
    public static int CommandCount => Instance != null ? commands.Length : 0;

    public static ConsoleCommand Command(int i)
    {
        return commands[i];
    }

    static DevConsole _console;
    public static DevConsole Console
    {
        get
        {
            if(_console == null && _consoleEnabled)
                EnableConsole();
            return _console;
        }
    }

    static void EnableConsole()
    {
        _console = new DevConsole();
        Application.logMessageReceived += OnLogMessageReceived;

        Debug.Log("Console Enabled");
    }

    static void DisableConsole()
    {
        UsingConsole = false;
        MiniConsole = false;
        _console = null;
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    public class DevConsole
    {
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

        public DevConsole()
        {
            consoleQueue = new Queue<string>();
            colorQueue = new Queue<byte>();

            background = new Texture2D(1, 1);
            UpdateBackground();

            style = new GUIStyle();
            style.normal.textColor = NormalColor;
            style.alignment = TextAnchor.LowerLeft;
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
            if(GameManager.ConsoleEnabled)
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
                Console.Update();
                ResetScroll();
            } else if(input.Length > 3 && input[0] == '#' && input[2] == ' ')
            {
                input = input.Substring(3);
            }
            ProcessCommand(input);
        }

        public void Respond(string input, byte type = 0)
        {
            if(GameManager.ConsoleEnabled)
            {
                consoleQueue.Enqueue(input);
                colorQueue.Enqueue(type);
                Console.Update();
            }
        }

        void ProcessCommand(string inputValue)
        {
            if(!inputValue.StartsWith(CommandPrefix))
                return;

            inputValue = inputValue.Remove(0, CommandPrefix.Length);

            string[] inputSplit = inputValue.Split(' ');

            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();

            ProcessCommand(commandInput, args);
        }

        void ProcessCommand(string commandInput, string[] args)
        {
            for(int i = 0; i < CommandCount; i++)
            {
                if(GameManager.Command(i) != null)
                {
                    if(!commandInput.Equals(GameManager.Command(i).KeyWord, StringComparison.OrdinalIgnoreCase))
                        continue;

                    if(GameManager.Command(i).Process(args))
                        return;
                }
            }
            Respond("Invalid Command", 1);
        }

        public void ResetScroll()
        {
            scroll = new Vector2(0, Mathf.Infinity);
        }

        public void NormalGUI()
        {
            style.fontSize = (int)Mathf.Max(Screen.height / 67.5f, 16f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height / 2f), background);
            scroll = GUILayout.BeginScrollView(scroll, style, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height / 2f - style.fontSize * 1.1f));

            GUILayout.FlexibleSpace();
            for(int i = 0; i < Count; i++)
            {
                style.normal.textColor = Color(i);
                GUILayout.Label("# " + Console[i], style);
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

        public void MiniGUI()
        {
            style.fontSize = (int)Mathf.Max(Screen.height / 67.5f, 16f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, style.fontSize * 2.1f), background);
            GUILayout.BeginScrollView(new Vector2(0, Mathf.Infinity), style, GUILayout.Width(Screen.width), GUILayout.Height(style.fontSize * 2.1f));

            GUILayout.FlexibleSpace();
            for(int i = Count < 2 ? 0 : Count - 2; i < Count; i++)
            {
                style.normal.textColor = Color(i);
                GUILayout.Label("# " + Console[i], style);
            }
            style.normal.textColor = NormalColor;
            GUILayout.EndScrollView();
        }
    }

    static void OnLogMessageReceived(string logString, string stackTrace, LogType logType)
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
        Console.Respond(response, type);
    }

    void OnGUI()
    {
        bool wasEnabled = GUI.enabled;
        GUI.depth = 0;
        GUI.enabled = true;

        if(UsingConsole)
        {
            Cursor.visible = true;
            Console.NormalGUI();
        } else
        {
            Cursor.visible = !hideCursor && Cursor.visible;
            if(MiniConsole)
                Console.MiniGUI();
        }

        if(ConsoleEnabled && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.BackQuote)
        {
            if(MiniConsole)
            {
                MiniConsole = false;
            } else
            {
                UsingConsole = !UsingConsole;
                Console.ResetScroll();
                if(!UsingConsole)
                    MiniConsole = true;
            }
        }

        GUI.depth = 1;
        GUI.enabled = wasEnabled;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GameManager.ConsoleEnabled = EditorGUILayout.Toggle("Console Enabled", GameManager.ConsoleEnabled);
        GameManager.hideCursor = EditorGUILayout.Toggle("Hide Cursor", GameManager.hideCursor);
        base.OnInspectorGUI();
    }
}
#endif