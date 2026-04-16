using System;

namespace AutomataSimulator.Core
{
    public class BadCode
    {
        public void DoSomething()
        {
            int n = 100;
            int i = 0;
            string s = "This is a string"; // StyleCop: Неиспользуемая переменная

            // SonarAnalyzer: Potential division by zero
            int result = n / i;

            // SonarAnalyzer: Equality comparison with float/double
            double magic = 3.14159;
            if (magic == 3.14159)
            {
                Console.WriteLine("Magic!");
            }
        }
    }
}