namespace AsyncInitialization.IntegrationTests
{
    using System;
    using System.Linq;
    using DependencyInjection;
    using FluentAssertions;
    using Interception;
    using Ninject;
    using Ninject.Extensions.ContextPreservation;
    using Xunit;

    public class InterceptorSequenceIntegrationTest : KernelProvidingBase
    {
        private static readonly Guid TargetId = Guid.NewGuid();

        private readonly IInterceptionTracker interceptionTracker;

        public InterceptorSequenceIntegrationTest()
        {
            this.Kernel.Load<ContextPreservationModule>();

            this.Kernel.Bind<IInterceptionTracker>().To<InterceptionTracker>()
                        .InSingletonScope();

            this.Kernel.Bind<IFakeTarget>().ToInterceptedProxy<IFakeTarget, FakeTarget>(x => x
                        .InterceptedBy<FakeInterceptor3>(3)
                        .InterceptedBy<FakeInterceptor>(1)
                        .InterceptedBy<FakeInterceptor2>(2));

            this.interceptionTracker = this.Kernel.Get<IInterceptionTracker>();

            var target = this.Kernel.Get<IFakeTarget>(new TypedConstructorArgument(typeof(Guid), TargetId, true));
            target.Method1();
            target.Method2();
        }

        [Fact]
        public void Should_instantiate_all_interceptors()
        {
            this.interceptionTracker.Instanciations.Should()
                .HaveCount(3)
                .And.Contain(x => x is FakeInterceptor)
                .And.Contain(x => x is FakeInterceptor2)
                .And.Contain(x => x is FakeInterceptor3);
        }

        [Fact]
        public void Should_intercept_all_invocations_using_correct_interceptor_sequence()
        {
            var interceptor1 = this.DetermineInterceptorInstance<FakeInterceptor>();
            var interceptor2 = this.DetermineInterceptorInstance<FakeInterceptor2>();
            var interceptor3 = this.DetermineInterceptorInstance<FakeInterceptor3>();

            var expectedInvocations = new[]
            {
                new InterceptedInvocation(interceptor1, TargetId, FakeTarget.MethodInfo1),
                new InterceptedInvocation(interceptor2, TargetId, FakeTarget.MethodInfo1),
                new InterceptedInvocation(interceptor3, TargetId, FakeTarget.MethodInfo1),
                new InterceptedInvocation(interceptor1, TargetId, FakeTarget.MethodInfo2),
                new InterceptedInvocation(interceptor2, TargetId, FakeTarget.MethodInfo2),
                new InterceptedInvocation(interceptor3, TargetId, FakeTarget.MethodInfo2)
            };

            this.interceptionTracker.Invocations.Should()
                .HaveSameCount(expectedInvocations)
                .And.ContainInOrder(expectedInvocations);
        }

        private IFakeInterceptor DetermineInterceptorInstance<TInterceptorType>()
            where TInterceptorType : IFakeInterceptor
        {
            return this.interceptionTracker.Instanciations.Single(x => x.GetType() == typeof(TInterceptorType));
        }

        public class FakeInterceptor2 : FakeInterceptor
        {
            public FakeInterceptor2(IInterceptionTracker interceptorRegistry, Guid id)
                : base(interceptorRegistry, id)
            {
            }
        }

        public class FakeInterceptor3 : FakeInterceptor
        {
            public FakeInterceptor3(IInterceptionTracker interceptorRegistry, Guid id)
                : base(interceptorRegistry, id)
            {
            }
        }
    }
}