// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToLambdaExpression
namespace AsyncInitialization.IntegrationTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using DependencyInjection;
    using FluentAssertions;
    using Interception;
    using Ninject;
    using Ninject.Extensions.ContextPreservation;
    using Xunit;

    public class OneInterceptorPerInstanceIntegrationTest : KernelProvidingBase
    {
        private static readonly Guid TargetId1 = Guid.NewGuid();
        private static readonly Guid TargetId2 = Guid.NewGuid();

        private readonly IInterceptionTracker interceptionTracker;

        public OneInterceptorPerInstanceIntegrationTest()
        {
            this.Kernel.Load<ContextPreservationModule>();

            this.Kernel.Bind<IInterceptionTracker>().To<InterceptionTracker>()
                .InSingletonScope();

            this.Kernel.Bind<IFakeTarget>().ToInterceptedProxy<IFakeTarget, FakeTarget>(x => x
                            .InterceptedBy<FakeInterceptor>());

            this.interceptionTracker = this.Kernel.Get<IInterceptionTracker>();

            var target1 = this.InstantiateTarget(TargetId1);
            var target2 = this.InstantiateTarget(TargetId2);

            target1.Method1();

            target1.Method2();

            target2.Method2();

            target1.Method3();

            target2.Method3();

            target2.Method1();
        }

        [Fact]
        public void Should_instanciate_one_interceptor_per_target_instance()
        {
            this.interceptionTracker.Instanciations.Should().HaveCount(2);
        }

        [Fact]
        public void Should_instanciate_interceptor_on_context_of_target()
        {
            this.interceptionTracker.Instanciations
                .Select(x => x.Id)
                .Should()
                    .Contain(TargetId1)
                    .And.Contain(TargetId2);
        }

        [Fact]
        public void Should_use_instance_specific_interceptor()
        {
            this.interceptionTracker.Invocations
                .Where(x => x.Interceptor.Id != x.TargetId)
                .Should()
                .BeEmpty("When the interceptor's and target's Ids mismatch then the wrong interceptor was used.");
        }

        [Fact]
        public void Should_intercept_all_invocations()
        {
            this.VerifyInterceptedInvocations(TargetId1, FakeTarget.MethodInfo1, FakeTarget.MethodInfo2, FakeTarget.MethodInfo3);
            this.VerifyInterceptedInvocations(TargetId2, FakeTarget.MethodInfo2, FakeTarget.MethodInfo3, FakeTarget.MethodInfo1);
        }

        private IFakeTarget InstantiateTarget(Guid id)
        {
            return this.Kernel.Get<IFakeTarget>(new TypedConstructorArgument(typeof(Guid), id, true));
        }

        private void VerifyInterceptedInvocations(Guid targetId, params MethodInfo[] expectedInterceptions)
        {
            this.interceptionTracker
                .Invocations
                .Where(x => x.TargetId == targetId)
                .Select(x => x.InterceptedMethod).ToList()
                .Should()
                .HaveSameCount(expectedInterceptions)
                .And.ContainInOrder(expectedInterceptions);
        }
    }
}