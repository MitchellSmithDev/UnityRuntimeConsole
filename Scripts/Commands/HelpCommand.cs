using UnityEngine;

[CreateAssetMenu(fileName = "HelpCommand", menuName = "Commands/Help")]
public class HelpCommand : ConsoleCommand
{
    string _keyWord = "help";
    public override string KeyWord => _keyWord;

    public override bool Process(string[] args)
    {
        string response = "List of all Commands :\n";
        for(int i = 0; i < GameManager.CommandCount; i++)
            response += GameManager.CommandPrefix + GameManager.Command(i).KeyWord + "\n";
        GameManager.Console.Respond(response + "You can also add '#1 ' or '#2 ' at the start to make the line colored as an erorr or a warning, respectively.");
        return true;
    }
}