namespace AsyncInitialization.IntegrationTests
{
    using System.Collections.Generic;
    using Castle.DynamicProxy;

    public interface IInterceptionTracker
    {
        IEnumerable<IFakeInterceptor> Instanciations { get; }

        IEnumerable<IFakeInterceptor> Disposals { get; }

        IEnumerable<InterceptedInvocation> Invocations { get; }

        void TrackInstantiation(IFakeInterceptor interceptor);

        void TrackDisposal(IFakeInterceptor interceptor);

        void TrackInterception(IFakeInterceptor interceptor, IInvocation invocation);
    }
}