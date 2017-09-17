using System;
using System.Reflection;
using System.Linq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.IO;
using Shouldly;
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
            Add_Test();
            DocMe.Instance().Write();

        }

        public void Geofence_Test()
        {
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

        }

        [DocMethod(nameof(Calculator.Add))]
        [DocMeFact(@"
        The sum of the first two numbers will be the age of the person.
        The DateTime parameter is of no use.
        ")]
        public void Add_Test()
        {
            var doc = DocMe.New(MethodBase.GetCurrentMethod());
            
            var now = DateTime.Now.Date;
            var p = new Person("Dennis", 28);

            doc.Call(1, 2, p, now);

            Calculator.Add(1, 2, p, now);

            doc.Insert("The targetPerson is passed as reference and its state will be modified.");
            doc.State(p);
        }

        public void ShouldDocBe(Func<int> a , int b){
            var m = a.Method;
            a().ShouldBe(b);

        }
    }
}