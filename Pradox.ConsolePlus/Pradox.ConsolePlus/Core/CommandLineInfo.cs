namespace Pradox.ConsolePlus.Core
{
    public class CommandLineInfo
    {
        public CommandLineInfo(string[] args)
        {
            Args = args;
        }

        public string[] Args { get; set; }
    }
}
