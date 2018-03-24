using System;
using System.Collections.Generic;

namespace Pradox.ConsolePlus.Core
{
    public class CommandModuleInfo
    {
        public CommandModuleInfo()
        {
            Methods = new List<CommandMethodInfo>();
        }

        public string Alias { get; set; }
        public string Description { get; set; }
        public Type ClassType { get; set; }

        public IList<CommandMethodInfo> Methods { get; set; }
    }
}
