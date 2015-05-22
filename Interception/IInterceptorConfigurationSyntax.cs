namespace AsyncInitialization.Interception
{
    using Castle.DynamicProxy;

    public interface IInterceptorConfigurationSyntax<TInterface>
    {
        /// <summary>
        /// Configures the proxy of <c>TInterface</c> to be intercepted by <c>TInterceptor</c>
        /// </summary>
        /// <typeparam name="TInterceptor">Type of the interceptor.</typeparam>
        /// <param name="order">Order of the interceptor. An interceptor with lower order will be called before an interceptor with higher value.</param>
        /// <returns>Configuration syntax.</returns>
        IInterceptorConfigurationSyntax<TInterface> InterceptedBy<TInterceptor>(int order)
            where TInterceptor : IInterceptor;

        IInterceptorConfigurationSyntax<TInterface> InterceptedBy<TInterceptor>()
            where TInterceptor : IInterceptor;
    }
}