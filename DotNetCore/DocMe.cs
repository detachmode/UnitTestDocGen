using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NSubstitute;
using NSubstitute.ExceptionExtensions;


namespace DocGenerator
{
    public class DocumentedAssert
    {
        public Type Class { get; set; }
        public string Method { get; set; }
        public string Fact { get; set; }
        public List<string> DocTextBlocks { get; set; } = new List<string>();


        private void AddCsharp(string code)
        {
            StringBuilder sb = new StringBuilder();
            void append(string str) => sb.AppendLine(str);
            append("```csharp");
            append(code);
            append("```");
            this.DocTextBlocks.Add(sb.ToString());
        }
        public void Call(params object[] args)
        {

            var ms = typeof(Calculator).GetMethod("Add");
            Insert($"When called `{this.Method}` of the class `{this.Class.Name}` with:");

            var parameter = Class
                .GetMethod(Method)
                .GetParameters();

            var formattedArgs = String.Join(",",
                args.Zip(parameter, (a, p) =>
                {
                    if (!a.GetType().IsPrimitive)
                        return p.Name;
                    else
                        return a;
                }));


            AddCsharp($"{this.Method}({formattedArgs});");

            string createRow(object a, string name)
            {
                return $@"  <tr>
        <td>{name}</td>
        <td>{a.GetType().Name}</td>
        <td style=""white-space:pre-wrap"">   
{a.ClassToYaml()}</td>
    </tr>";
            };

            var values = args
                 .Where(x => !x.GetType().IsPrimitive);



            var classParameters =
                parameter
                    .Where(p => !p.ParameterType.IsPrimitive)
                    .Select(p => p.Name);


            var rows = values.Zip(classParameters, createRow);

            var paramTable = ($@"<table>
    <tr>
        <th>Parameter</th>
        <th>Type</th>
        <th>State</th>
    </tr>
{String.Concat(rows)}
</table>");
            this.DocTextBlocks.Add(paramTable);
        }

        public void Insert(string v) =>
            this.DocTextBlocks.Add(v + Environment.NewLine);

        public void State(object o)
        {
            StringBuilder sb = new StringBuilder();
            void append(string str) => sb.AppendLine(str);
            append($"Now the {o.GetType().Name} object has the following state:");
            append("");
            append("```yaml");
            append($"{o.ClassToYaml()}```");

            DocTextBlocks.Add(sb.ToString());
        }

    }
    public class Mock<T>
    {
        public string Method { get; set; }
        public Exception Throws { get; set; }
    }

    public class DocMe
    {
        private static DocMe _instance = null;
        public Action<string> OnOutput { get; set; }
        public List<DocumentedAssert> Docs { get; set; } = new List<DocumentedAssert>();
        public static DocMe Instance()
        {
            if (_instance != null)
                return _instance;
            _instance = new DocMe();
            return _instance;
        }


        public DocMe()
        {
            OnOutput = Console.Write;
        }



        public static void SetupMocks<T>(Mock<T> mock) where T : class
        {
            typeof(T)
                .When(info => Caller.CallWithDefaults(typeof(T), mock.Method))
                .ThrowsForAnyArgs(mock.Throws);
        }

        public void DocMock<T>(Mock<T> mock)
        {
            var str = $"If the method {mock.Method} of the interface {typeof(T).Name} \n";
            // var args = doc.Args.Aggregate((a,b) => $"{a},{b}");
            str += $"throws an exception of type: {mock.Throws.GetType().Name};\n";
            Console.Write(str);
        }

        public void Write()
        {
            var doc1 = Docs.First();
            StringBuilder sb = new StringBuilder();
            void append(string str) => sb.AppendLine(str);

            append($"# Documentation of class `{doc1.Class.Name}`");
            append($"## Method `{doc1.Method}`");
            append(doc1.Fact);
            append("");
            doc1.DocTextBlocks.ForEach(append);
            OnOutput(sb.ToString());
        }

        public void AssertAndDoc<T>(T expected, T actual, DocumentedAssert doctest)
        {
            // Assert 
            doctest.Insert(expected.ClassToYaml());

        }

        public static DocumentedAssert New(MethodBase methodBase)
        {
            T GetAttrText<T>() where T : class
            {
                var classAttr = methodBase.DeclaringType
                    .GetCustomAttributes()
                    .First(x => x.GetType() == typeof(T));
                var c = classAttr as T;
                return c;
            };
            var doctest = new DocumentedAssert();
            DocMe.Instance().Docs.Add(doctest);


            doctest.Class = GetAttrText<DocClassAttribute>().ClassType;

            var attr = (DocMethodAttribute)methodBase
                .GetCustomAttributes(typeof(DocMethodAttribute), true)[0];
            doctest.Method = attr.Method;

            doctest.Fact = ((DocMeFactAttribute)methodBase
                .GetCustomAttributes(typeof(DocMeFactAttribute), true).First()).Fact;
            doctest.Fact = doctest.Fact.Trim();
            return doctest;
        }
    }


}