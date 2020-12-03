using UnityEngine;

[CreateAssetMenu(fileName = "ConsoleLimitCommand", menuName = "Commands/ConsoleLimit")]
public class ConsoleLimitCommand : ConsoleCommand
{
    string _keyWord = "consolelimit";
    public override string KeyWord => _keyWord;

    public override bool Process(string[] args)
    {
        if(args.Length != 1)
            return false;

        if(!int.TryParse(args[0], out int value))
            return false;

        GameManager.Console.Limit = value;
        GameManager.Console.Respond("Set Console line limit to " + GameManager.Console.Limit);

        return true;
    }
}
