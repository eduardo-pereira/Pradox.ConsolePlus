using Pradox.ConsolePlus.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.CommandModules
{
    [CommandModule("file", Description = "File commands")]
    public class FileModule
    {
        [CommandMethod("write", Description = "Saves text to file")]
        public void Write(IEnumerable<string> lines, string filename)
        {
            using (var file = new StreamWriter(filename))
            {
                foreach (var line in lines)
                {
                    file.WriteLine(line);
                }
            }
        }

        [CommandMethod("read", Description = "Reads text from file")]
        public IEnumerable<string> Read(string filename)
        {
            using (var file = new StreamReader(filename))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
