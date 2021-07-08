using UnityEngine;

[CreateAssetMenu(fileName = "ConsoleLimitCommand", menuName = "Commands/ConsoleLimit")]
public class ConsoleLimitCommand : ConsoleCommand
{
    string _keyWord = "consolelimit";
    public override string KeyWord => _keyWord;

    public override bool Process(string[] args)
    {
        if(RuntimeConsole.Instance != null)
        {
            if(args.Length != 1)
                return false;

            if(!int.TryParse(args[0], out int value))
                return false;

            RuntimeConsole.Instance.Limit = value;
            RuntimeConsole.Instance.Respond("Set Console line limit to " + RuntimeConsole.Instance.Limit);

            return true;
        }
        return false;
    }
}
