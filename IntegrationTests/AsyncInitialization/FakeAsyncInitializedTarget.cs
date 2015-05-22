namespace AsyncInitialization.IntegrationTests.AsyncInitialization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class FakeAsyncInitializedTarget : Interception.AsyncInitialization.IAsyncInitialization, IFakeAsyncInitializedTarget
    {
        private readonly ManualResetEventSlim initializationWaitHandle;
        private readonly int someValue;
        private readonly Exception initializationException;

        public FakeAsyncInitializedTarget(ManualResetEventSlim initializationWaitHandle, int someValue, Exception initializationException)
        {
            this.initializationWaitHandle = initializationWaitHandle;
            this.someValue = someValue;
            this.initializationException = initializationException;
        }

        public int RetrieveSomeValue()
        {
            return this.someValue;
        }

        public Task InitializeAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                if (this.initializationException != null)
                {
                    throw this.initializationException;
                }

                if (!this.initializationWaitHandle.Wait(TimeSpan.FromSeconds(30)))
                {
                    throw new TimeoutException("unit test timeout: we can't run forever...");
                }
            });
        }
    }
}