using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeMock.ArrangeActAssert;

namespace TypeMockIsolatorTests
{
    interface IPerson
    {
        string Name { get; set; }
        IValue Value { get; set; }
    }

    public interface IValue
    {
        void DoValue();
    }

    class StaticClass
    {
        public void DoSomething()
        {
            throw new Exception("Do Something else :)");
        }

        public static void ExecuteQuery()
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    [Category("Learning Isolator")]
    public class IsolatorTests
    {
        private const string ConnectionString = "Server=NotExists;initial catatlog=any;integrated security=true";

        [Test]
        public void Isolator_ByDefault_HasMassisolation()
        {
            var fakePerson = Isolate.Fake.Instance<IPerson>();

            Action callingValueMethod = () => fakePerson.Value.DoValue();

            callingValueMethod.ShouldNotThrow<Exception>();
        }

        [Test]
        public void Isolator_Always_IsolateStaticMethods()
        {
            Isolate.WhenCalled(() => StaticClass.ExecuteQuery()).IgnoreCall();

            Action callingStaticMethod = () => StaticClass.ExecuteQuery();

            callingStaticMethod.ShouldNotThrow<Exception>();
        }

        [Test]
        public void Isloator_SystemDateTimeNow_ShouldReturnSep15th2016()
        {
            Isolate.WhenCalled(() => DateTime.Now).WillReturn(new DateTime(2016, 09, 15));

            var now = DateTime.Now;

            now.Should().Be(new DateTime(2016, 09, 15));
        }

        [Test]
        public void Isloator_WithAdoDotNet_ShouldSwapCallToFake()
        {
            var adoNetClass = new AdoNetClass(ConnectionString);
            var fakeConnection = Isolate.Fake.Instance<SqlConnection>();
            Isolate.Swap.AllInstances<SqlConnection>().WithRecursiveFake();

            Action executingQuery = () => adoNetClass.Execute();

            executingQuery.ShouldNotThrow<Exception>();
        }

        [Test]
        public void Isolator_TightlyCoupledCode_IsolatedToo()
        {
            Isolate.Swap.AllInstances<AdoNetClass>().WithRecursiveFake();
            var adoNetDependency = new AdoNetDependency();

            Action addingPerson = () => adoNetDependency.AddPerson();

            addingPerson.ShouldNotThrow<Exception>();
        }
    }

    internal class AdoNetDependency
    {
        public AdoNetDependency()
        {
        }

        public void AddPerson()
        {
            var adoNet = new AdoNetClass("someConnectionstring");
            adoNet.Execute();
        }
    }

    internal class AdoNetClass
    {
        private string connectionString;

        public AdoNetClass(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Execute()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select 'artur', 'ilin', '1991-05-08' into #persons";
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
