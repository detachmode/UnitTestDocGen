using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace DocGenerator
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        
    }
}