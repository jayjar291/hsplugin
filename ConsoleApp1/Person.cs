using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Person
    {
        public string Name { get; set; }
        public List<String> Properties { get; set; }
        public Person()
        {
            Properties = new List<string>();
        }
    }
}
