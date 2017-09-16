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
        public string Args { get; set; }
        public string ParameterTable { get; set; }
        public object Results { get; set; }
        public void SetArg(params object[] args)
        {

            var ms =typeof(Calculator).GetMethod("Add");
            
            var parameter = Class
                .GetMethod(Method)
                .GetParameters();

            var formattedArgs = String.Join(",", 
                args.Zip(parameter, (a,p) =>{
                    if(!a.GetType().IsPrimitive)
                        return p.Name;
                    else
                        return a;
                }));

                  
    
            Args = $"({formattedArgs})";

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

            this.ParameterTable = $@"<table>
    <tr>
        <th>Parameter</th>
        <th>Type</th>
        <th>Content</th>
    </tr>
{String.Concat(rows)}
</table>";
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



        public void SetupMocks<T>(Mock<T> mock) where T : class
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
            append($"When called `{doc1.Method}` of the class `{doc1.Class.Name}` with:");
            // var args = doc.Args.Aggregate((a,b) => $"{a},{b}");
            append("```csharp");
            append($"{doc1.Method}{doc1.Args};");
            append("```");            
            append($"{doc1.ParameterTable}");
            append($"Now the {doc1.Results.GetType().Name} object has the following content:");
            append("");
            append("```yaml");
            append($"{doc1.Results.ClassToYaml()}"); 
            append("```");
            OnOutput(sb.ToString());
        }

        public void AssertAndDoc<T>(T expected, T actual, DocumentedAssert doctest)
        {
            // Assert 
            doctest.Results = expected;

        }

        public DocumentedAssert SetupDocTest(MethodBase methodBase)
        {
            var doctest = new DocumentedAssert();
            Docs.Add(doctest);

            var classAttr = methodBase.DeclaringType
                .GetCustomAttributes()
                .First(x => x.GetType() == typeof(DocClassAttribute));
            var c = classAttr as DocClassAttribute;
            doctest.Class = c.ClassType;

            var attr = (DocMethodAttribute)methodBase
                .GetCustomAttributes(typeof(DocMethodAttribute), true)[0];
            doctest.Method = attr.Method;
            return doctest;
        }
    }


}