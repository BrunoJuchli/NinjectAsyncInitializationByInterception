namespace AsyncInitialization.Interception.AsyncInitialization
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Castle.DynamicProxy;

    public class AsyncInitializationInterceptor<TProxied> : IInterceptor
        where TProxied : IAsyncInitialization
    {
        private readonly Task initializationTask;

        public AsyncInitializationInterceptor(TProxied proxiedObject)
        {
            this.initializationTask = proxiedObject.InitializeAsync();
            this.InitializationTimeout = TimeSpan.FromSeconds(5); // default
        }

        public TimeSpan InitializationTimeout { get; set; }

        public void Intercept(IInvocation invocation)
        {
            if (!this.initializationTask.Wait(this.InitializationTimeout))
            {
                throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Initialization of async initialized object timed out after {0}", this.InitializationTimeout));
            }

            invocation.Proceed();
        }
    }
}