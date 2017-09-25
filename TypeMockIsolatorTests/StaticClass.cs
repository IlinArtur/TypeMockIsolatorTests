using System;

namespace TypeMockIsolatorTests
{
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
}
