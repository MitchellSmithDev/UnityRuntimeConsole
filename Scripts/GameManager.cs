using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Initialize();
        } else if(Instance != this)
        {
            Destroy(gameObject);
            throw new System.InvalidOperationException("Another Game Manager instance already exists, destroying this instance.");
        }
    }

    [SerializeField] bool enableConsole = true;
    [SerializeField] string commandPrefix = "/";
    [SerializeField] Font consoleFont;
    [SerializeField] ConsoleCommand[] commands = new ConsoleCommand[0];

    void Initialize()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        RuntimeConsole.textFont = consoleFont;
        RuntimeConsole.IsEnabled = enableConsole;
        RuntimeConsole.SetCommands(commandPrefix, commands);
        RuntimeConsole.Initialize();
    }

    void OnGUI()
    {
        if(RuntimeConsole.Instance != null)
            RuntimeConsole.Instance.OnGUI();
    }
}
