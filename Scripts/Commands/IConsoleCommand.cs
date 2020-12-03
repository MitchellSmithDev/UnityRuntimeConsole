public interface IConsoleCommand
{
    string KeyWord { get; }
    bool Process(string[] args);
}