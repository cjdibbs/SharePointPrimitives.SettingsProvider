using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleExample.Properties;

namespace SimpleExample {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine(Settings.Default.ExampleInt);
            Console.WriteLine(Settings.Default.ExampleString);
            Console.WriteLine(Settings.Default.ExampleConnection);
        }
    }
}
