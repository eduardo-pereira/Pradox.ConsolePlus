using System.Reflection;

namespace Pradox.ConsolePlus.Core
{
    public class CommandMethodInfo
    {
        public string Alias { get; set; }
        public string Description { get; set; }

        public MethodInfo Method { get; set; }
    }
}
