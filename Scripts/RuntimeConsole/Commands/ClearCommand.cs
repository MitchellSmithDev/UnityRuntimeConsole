using UnityEngine;

[CreateAssetMenu(fileName = "ClearCommand", menuName = "Commands/Clear")]
public class ClearCommand : ConsoleCommand
{
    string _keyWord = "clear";
    public override string KeyWord => _keyWord;

    public override bool Process(string[] args)
    {
        if(RuntimeConsole.Instance != null)
        {
            RuntimeConsole.Instance.Clear();
            return true;
        }
        return false;
    }
}