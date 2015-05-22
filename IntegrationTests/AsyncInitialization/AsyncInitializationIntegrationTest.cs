namespace AsyncInitialization.IntegrationTests.AsyncInitialization
{
    using System;
    using System.Threading;
    using DependencyInjection;
    using FluentAssertions;
    using Interception;
    using Ninject;
    using Ninject.Extensions.ContextPreservation;
    using Ninject.Parameters;
    using Xunit;

    public class AsyncInitializationIntegrationTest : KernelProvidingBase
    {
        private const int TargetReturnValue = 5;

        private readonly ManualResetEventSlim initializationWaitHandle;

        public AsyncInitializationIntegrationTest()
        {
            this.Kernel.Load<ContextPreservationModule>();

            this.Kernel.Bind<IInterceptionTracker>().To<InterceptionTracker>()
                .InSingletonScope();

            this.Kernel.Bind<IFakeAsyncInitializedTarget>()
                            .ToInterceptedProxy<IFakeAsyncInitializedTarget, FakeAsyncInitializedTarget>(x => x
                                .InterceptedBy<Interception.AsyncInitialization.AsyncInitializationInterceptor<FakeAsyncInitializedTarget>>());

            this.Kernel.Bind<Interception.AsyncInitialization.AsyncInitializationInterceptor<FakeAsyncInitializedTarget>>().ToSelf()
                .OnActivation(x => x.InitializationTimeout = TimeSpan.FromSeconds(0));

            this.initializationWaitHandle = new ManualResetEventSlim();
        }

        [Fact]
        public void AccessingTarget_WhenTargetInitialized_MustNotThrow()
        {
            var target = this.CreateTarget();
            this.initializationWaitHandle.Set();

            target.Invoking(x => x.RetrieveSomeValue())
                .ShouldNotThrow();
        }

        [Fact]
        public void AccessingTarget_WhenTargetInitialized_MustReturnValue()
        {
            var target = this.CreateTarget();
            this.initializationWaitHandle.Set();

            target.RetrieveSomeValue().Should().Be(TargetReturnValue);
        }
        
        [Fact]
        public void AccessingTarget_MustWaitForInitializationToEnd()
        {
            var target = this.CreateTarget();

            target.Invoking(x => x.RetrieveSomeValue())
                    .ShouldThrow<TimeoutException>();

            // don't leave wait task hanging
            this.initializationWaitHandle.Set(); 
        }

        [Fact]
        public void AccessingTarget_WhenInitializationThrows_MustRethrow()
        {
            var expectedException = new InvalidOperationException("something went wrong @ initialization");
            var target = this.CreateTarget(expectedException);

            target.Invoking(x => x.RetrieveSomeValue())
                .ShouldThrow<Exception>()
                .Where(ex => ex == expectedException);
        }

        private IFakeAsyncInitializedTarget CreateTarget(Exception initializationException = null)
        {
            return this.Kernel.Get<IFakeAsyncInitializedTarget>(
                new TypedConstructorArgument(typeof(ManualResetEventSlim), this.initializationWaitHandle, true),
                new TypedConstructorArgument(typeof(int), TargetReturnValue, true),
                new ConstructorArgument("initializationException", initializationException, true));
        }
    }
}