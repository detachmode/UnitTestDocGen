using System;
using System.Reflection;

namespace DocGenerator
{

    [AttributeUsage(AttributeTargets.Class)]
    public class DocClassAttribute : Attribute
    {
        public Type ClassType { get; set; }
        public DocClassAttribute(Type type)
        {
            ClassType = type;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DocMethodAttribute : Attribute
    {
        public string Method { get; set; }
        public DocMethodAttribute(string method)
        {
            Method = method;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class DocMeFactAttribute : Attribute
    {
        public string Fact { get; set; }
        public DocMeFactAttribute(string fact)
        {
            Fact = fact;
        }
    }
}