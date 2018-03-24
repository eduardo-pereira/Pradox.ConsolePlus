using Pradox.ConsolePlus.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.CommandModules
{
    [CommandModule("deco", Description = "Decorates text lines")]
    public class DecorateModule
    {
        [CommandMethod("add", Description = "Decorates a text line with the head prefix and tail suffix")]
        public IEnumerable<string> Add(IEnumerable<string> lines, string head, string tail)
        {
            foreach (var item in lines)
            {
                yield return string.Format("{0}{1}{2}", head, item, tail);
            }
        }
    }
}
