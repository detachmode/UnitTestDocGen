using System.Linq.Expressions;
using System;
using DocGenerator;

namespace csharp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // GetMethodInfo(Program.Main);
            var ut = new CalculatorTest();
            ut.Add_Test();
            //nameof(Main).CallWithDefaults();
        }

        // public static MethodInfo GetMI()
        // {
        //     // Get a delegate for the method you wish to fetch
        //     var mainDelegate = Program.Main.GetMethodInfo();
        //     MethodInfo mainMethod = mainDelegate.Method;
        //     return mainMethod;
        // }

        // MethodInfo GetMethodInfo(Action d)
        // {
        //     return d.Method;
        // }

    }
}
