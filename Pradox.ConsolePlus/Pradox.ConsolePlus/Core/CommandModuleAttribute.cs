using System;

namespace Pradox.ConsolePlus.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CommandModuleAttribute : Attribute
    {
        public CommandModuleAttribute(string alias)
        {
            Alias = alias;
        }

        public string Alias { get; set; }

        public string Description { get; set; }
    }
}
