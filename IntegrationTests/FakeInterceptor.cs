namespace AsyncInitialization.IntegrationTests
{
    using System;
    using Castle.DynamicProxy;

    public class FakeInterceptor : IFakeInterceptor
    {
        private readonly IInterceptionTracker interceptorRegistry;

        public FakeInterceptor(IInterceptionTracker interceptorRegistry, Guid id)
        {
            this.interceptorRegistry = interceptorRegistry;
            this.Id = id;
            this.interceptorRegistry.TrackInstantiation(this);
        }

        public Guid Id { get; private set; }

        public void Intercept(IInvocation invocation)
        {
            this.interceptorRegistry.TrackInterception(this, invocation);

            invocation.Proceed();
        }

        public void Dispose()
        {
            this.interceptorRegistry.TrackDisposal(this);
        }
    }
}