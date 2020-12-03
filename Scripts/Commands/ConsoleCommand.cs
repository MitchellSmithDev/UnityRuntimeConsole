using UnityEngine;

public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
{
    public abstract string KeyWord { get; }

    public abstract bool Process(string[] args);
}