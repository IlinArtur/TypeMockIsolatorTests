namespace TypeMockIsolatorTests
{
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
}
