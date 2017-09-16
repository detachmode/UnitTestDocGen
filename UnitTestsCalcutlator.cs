using System;
using System.Reflection;
using System.Linq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.IO;

namespace DocGenerator
{



    public interface IDataAccess
    {
        int CreatePerson(int id, Person person);
    }


    [DocClass(typeof(Calculator))]
    public class CalculatorTest
    {

        public void Init()
        {
            DocMe.Instance().OnOutput = (s) =>
            {
                File.AppendAllText("Markdown.md", s);
            };
        }

        [DocMethod(nameof(Calculator.Add))]
        public void Add_Test()
        {
            Init();
            var doc = DocMe.Instance().SetupDocTest(MethodBase.GetCurrentMethod());


            var dataAccess = Substitute.For<IDataAccess>();

            var mock = new Mock<IDataAccess>
            {
                Method = nameof(IDataAccess.CreatePerson),
                Throws = new Exception("Hello World!")
            };

            dataAccess.CreatePerson(Arg.Any<int>(), Arg.Any<Person>())
            .ThrowsForAnyArgs(mock.Throws);


            //  dataAccess
            //     .When( info => Caller.CallWithDefaults(typeof(IDataAccess), "CreatePerson"))
            //     .ThrowsForAnyArgs(new Exception("peter"));

            var now =  DateTime.Now.Date;
            var p = new Person("Dennis", 28);
            // SetupMocks(mock);
            doc.SetArg(1, 2, p, now);
            var result = Calculator.Add(1, 2, p, now);


            doc.Results = p;
            // DocMock(mock);            
            // WriteDoc(doc);
            DocMe.Instance().Write();
        }





    }
}