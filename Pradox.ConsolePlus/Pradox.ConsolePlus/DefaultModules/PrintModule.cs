using Pradox.ConsolePlus.Core;
using System;
using System.Collections.Generic;

namespace Pradox.ConsolePlus.DefaultModules
{
    [CommandModule("print", Description = "Print methods")]
    public class Print
    {
        [CommandMethod("write", Description = "Write text to console")]
        public void Write(IEnumerable<string> lines)
        {
            foreach (var item in lines)
            {
                Console.WriteLine("{0}", item);
            }
        }
    }
}
