using Pradox.ConsolePlus.Core;

namespace TestConsole.CommandModules
{
    [CommandModule("sam", Description = "sample commands")]
    public class Sample
    {
        [CommandMethod("sum", Description = "Sums two numbers")]
        public int Sum(int v1, int v2, int v3 = 0)
        {            
            return v1 + v2 + v3;
        }

        public int Sum2(int v1, int v2, int v3 = 0)
        {
            return v1 + v2 + v3;
        }

        [CommandMethod("sum3", Description = "Sums two numbers")]
        public int Sum3 (int v1, int v2)
        {
            return v1 + v2;
        }
    }
}
