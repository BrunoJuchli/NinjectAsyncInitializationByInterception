namespace AsyncInitialization.IntegrationTests
{
    using System;
    using System.Reflection;

    public class FakeTarget : IFakeTarget
    {
        public static readonly MethodInfo MethodInfo1 = Reflector<IFakeTarget>.GetMethod(x => x.Method1());
        public static readonly MethodInfo MethodInfo2 = Reflector<IFakeTarget>.GetMethod(x => x.Method2());
        public static readonly MethodInfo MethodInfo3 = Reflector<IFakeTarget>.GetMethod(x => x.Method3());

        public FakeTarget(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; private set; }

        public void Method1()
        {
        }

        public void Method2()
        {
        }

        public void Method3()
        {
        }

        public void Dispose()
        {
        }
    }
}