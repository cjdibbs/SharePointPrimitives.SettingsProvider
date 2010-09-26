using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPPrimitives.SettingsProvider.Example.Properties;

namespace SPPrimitives.SettingsProvider.Example {
    class Program {
        public static void Main(string[] args){
            Console.WriteLine(Settings.Default.StringExample);
            Console.WriteLine(Settings.Default.IntergerExample);
            Console.WriteLine(Settings.Default.TestConnection);
        }
    }
}
