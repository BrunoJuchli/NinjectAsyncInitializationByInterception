namespace AsyncInitialization.Interception
{
    using System;
    using Castle.DynamicProxy;

    internal class PerInstanceInterceptorContainer<TTargetInterface, TInterceptor> : IPerInstanceInterceptorContainer<TTargetInterface>
        where TInterceptor : IInterceptor
    {
        public PerInstanceInterceptorContainer()
        {
            this.Order = int.MaxValue;
        }

        public Type Interceptor
        {
            get { return typeof(TInterceptor); }
        }

        public int Order { get; set; }
    }
}