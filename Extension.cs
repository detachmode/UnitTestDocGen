using System.Linq;
using System.Reflection;
using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;
using System.IO;

namespace DocGenerator
{
    public static class Caller
    {

        public static string ClassToYaml(this object o)
        {
            var yamlser = new Serializer();
            using (var writer = new StringWriter())
            {
                yamlser.Serialize(writer, o);
                return writer.ToString();
            }
        }

        public static void CallWithDefaults(Type type, string methodname)
        {
            var m = type.GetMethod(methodname);
            m.Invoke(
                null, m.GetParameters().Select(GetDefaultValue).ToArray());
        }

        private static object GetDefaultValue(ParameterInfo p)
        {
            var t = p.ParameterType.IsByRef ?
                p.ParameterType.GetElementType()
                : p.ParameterType;

            return GetDefault(t);
        }


        public static object GetDefault(Type t)
        {
            return typeof(Caller)
            .GetMethod("GetDefaultGeneric")
            .MakeGenericMethod(t).Invoke(typeof(Caller), null);
        }


        
        public static void Foo(ref int x, string h)
        {
            x = 10;
        }

        public static T GetDefaultGeneric<T>()
        {
            return default(T);
        }

    }
}
