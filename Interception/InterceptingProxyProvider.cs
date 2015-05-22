namespace AsyncInitialization.Interception
{
    using System;
    using System.Linq;
    using Castle.DynamicProxy;
    using DependencyInjection;
    using Ninject;
    using Ninject.Activation;
    using Ninject.Extensions.ContextPreservation;
    using Ninject.Parameters;

    public class InterceptingProxyProvider<TInterface, TImplementation> : IProvider<TInterface>
        where TImplementation : TInterface
    {
        private readonly ProxyGenerator proxyGenerator;

        public InterceptingProxyProvider(ProxyGenerator proxyGenerator)
        {
            this.proxyGenerator = proxyGenerator;
        }

        public Type Type
        {
            get { return typeof(TInterface); }
        }

        public object Create(IContext context)
        {
            var target = context.ContextPreservingGet<TImplementation>();
            var targetArgument = new TypedConstructorArgument(typeof(TInterface), target, false);
            IInterceptor[] interceptors = this.RetrieveInterceptors(context,  targetArgument);
            return this.proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), target, interceptors);
        }

        private IInterceptor[] RetrieveInterceptors(IContext context, IParameter targetArgument)
        {
            IInterceptor[] interceptors = context.GetContextPreservingResolutionRoot()
                .GetAll<IPerInstanceInterceptorContainer<TInterface>>()
                .OrderBy(x => x.Order)
                .Select(x => this.CreateInterceptor(x.Interceptor, context, targetArgument))
                .ToArray();
            return interceptors;
        }

        private IInterceptor CreateInterceptor(Type interceptorType, IContext context, IParameter targetArgument)
        {
            return (IInterceptor)context.GetContextPreservingResolutionRoot().Get(interceptorType, targetArgument);
        }
    }
}