using System;

namespace Pradox.ConsolePlus.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CommandMethodAttribute : Attribute
    {
        public CommandMethodAttribute(string alias)
        {
            Alias = alias;
        }

        public string Alias { get; set; }

        public string Description { get; set; }
    }
}
