namespace AsyncInitialization.IntegrationTests
{
    using System;
    using Castle.DynamicProxy;

    public interface IFakeInterceptor : IInterceptor
    {
        Guid Id { get; }
    }
}