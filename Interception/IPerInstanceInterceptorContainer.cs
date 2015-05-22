namespace AsyncInitialization.Interception
{
    using System;

    internal interface IPerInstanceInterceptorContainer<TTargetInterface>
    {
        Type Interceptor { get; }

        int Order { get; }
    }
}