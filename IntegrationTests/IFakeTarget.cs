namespace AsyncInitialization.IntegrationTests
{
    using System;

    public interface IFakeTarget
    {
        Guid Id { get; }

        void Method1();

        void Method2();

        void Method3();
    }
}