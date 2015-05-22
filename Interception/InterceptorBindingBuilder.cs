namespace AsyncInitialization.Interception
{
    using Castle.DynamicProxy;
    using Ninject.Syntax;

    internal class InterceptorBindingBuilder<TTarget> : IInterceptorConfigurationSyntax<TTarget>
    {
        private readonly IBindingRoot bindingRoot;

        public InterceptorBindingBuilder(IBindingRoot bindingRoot)
        {
            this.bindingRoot = bindingRoot;
        }

        public IInterceptorConfigurationSyntax<TTarget> InterceptedBy<TInterceptor>()
            where TInterceptor : IInterceptor
        {
            this.bindingRoot.BindInterceptor<TTarget, TInterceptor>();
            return this;
        }

        public IInterceptorConfigurationSyntax<TTarget> InterceptedBy<TInterceptor>(int order)
            where TInterceptor : IInterceptor
        {
            this.bindingRoot.BindInterceptor<TTarget, TInterceptor>(order);
            return this;
        }
    }
}