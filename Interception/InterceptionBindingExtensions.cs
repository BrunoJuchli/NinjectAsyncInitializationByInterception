namespace AsyncInitialization.Interception
{
    using System;
    using Appccelerate;
    using Castle.DynamicProxy;
    using Ninject.Syntax;

    public static class InterceptionBindingExtensions
    {
        public static IBindingWhenInNamedWithOrOnSyntax<TInterface> ToInterceptedProxy<TInterface, TImplementation>(this IBindingToSyntax<TInterface> syntax, Action<IInterceptorConfigurationSyntax<TInterface>> configuration)
            where TImplementation : TInterface
        {
            Ensure.ArgumentNotNull(syntax, "syntax");
            Ensure.ArgumentNotNull(configuration, "configuration");

            configuration(new InterceptorBindingBuilder<TInterface>(syntax.Kernel));

            return syntax.ToProvider<InterceptingProxyProvider<TInterface, TImplementation>>();
        }

        public static void BindInterceptor<TTarget, TInterceptor>(this IBindingRoot bindingRoot, int order)
            where TInterceptor : IInterceptor
        {
            Ensure.ArgumentNotNull(bindingRoot, "bindingRoot");

            bindingRoot
                .Bind<IPerInstanceInterceptorContainer<TTarget>>()
                .To<PerInstanceInterceptorContainer<TTarget, TInterceptor>>()
                .OnActivation(x => x.Order = order);
        }

        public static void BindInterceptor<TTarget, TInterceptor>(this IBindingRoot bindingRoot)
            where TInterceptor : IInterceptor
        {
            Ensure.ArgumentNotNull(bindingRoot, "bindingRoot");

            bindingRoot.BindInterceptor<TTarget, TInterceptor>(int.MaxValue);
        }
    }
}