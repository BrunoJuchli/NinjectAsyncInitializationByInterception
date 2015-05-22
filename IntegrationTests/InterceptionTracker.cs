namespace AsyncInitialization.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using Castle.DynamicProxy;

    public class InterceptionTracker : IInterceptionTracker
    {
        private readonly ICollection<InterceptedInvocation> invocations;
        private readonly ICollection<IFakeInterceptor> instanciations;
        private readonly ICollection<IFakeInterceptor> disposals;

        private readonly object locker;

        public InterceptionTracker()
        {
            this.invocations = new Collection<InterceptedInvocation>();
            this.instanciations = new Collection<IFakeInterceptor>();
            this.disposals = new Collection<IFakeInterceptor>();

            this.locker = new object();
        }

        public IEnumerable<IFakeInterceptor> Instanciations
        {
            get { return this.instanciations; }
        }

        public IEnumerable<IFakeInterceptor> Disposals
        {
            get { return this.disposals; }
        }

        public IEnumerable<InterceptedInvocation> Invocations
        {
            get { return this.invocations; }
        }

        public void TrackInstantiation(IFakeInterceptor interceptor)
        {
            lock (this.locker)
            {
                this.instanciations.Add(interceptor);
            }
        }

        public void TrackDisposal(IFakeInterceptor interceptor)
        {
            lock (this.locker)
            {
                this.disposals.Add(interceptor);
            }
        }

        public void TrackInterception(IFakeInterceptor interceptor, IInvocation invocation)
        {
            Guid id = ((IFakeTarget)invocation.InvocationTarget).Id;
            MethodInfo method = invocation.Method;

            lock (this.locker)
            {
                this.invocations.Add(new InterceptedInvocation(interceptor, id, method));
            }
        }
    }
}