using Pradox.ConsolePlus;
using Pradox.ConsolePlus.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ConPlus.Initialize("Test Console Application", string.Empty);

            while (true)
            {
                try
                {                    
                    ConPlus.ExecuteCommand();

                }
                catch (Exception ex)
                {
                    ConPlus.Write(MessageType.Error, "Exception: {0}", ex.GetType());
                    ConPlus.Write(MessageType.Error, "Message: {0}", ex.Message);
                }
            }
        }
    }
}
