using UnityEngine;

[CreateAssetMenu(fileName = "ExampleCommand", menuName = "Commands/Example")]
public class ExampleCommand : ConsoleCommand
{
    string _keyWord = "example";
    public override string KeyWord => _keyWord;

    public override bool Process(string[] args)
    {
        if(RuntimeConsole.Instance != null)
        {
            RuntimeConsole.Instance.Respond("This is an example command.\n" + args.Length + " argument(s) were given.");
            return true;
        }
        return false;
    }
}
