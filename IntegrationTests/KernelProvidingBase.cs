namespace AsyncInitialization.IntegrationTests
{
    using System;
    using Ninject;

    public class KernelProvidingBase : IDisposable
    {
        public KernelProvidingBase()
        {
            this.Kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = false });
        }

        public StandardKernel Kernel
        {
            get;
            private set;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.Kernel.IsDisposed)
                {
                    this.Kernel.Dispose();
                }
            }
        }
    }
}