namespace AsyncInitialization.IntegrationTests
{
    using System;
    using System.Reflection;

    public class InterceptedInvocation
    {
        public InterceptedInvocation(IFakeInterceptor interceptor, Guid targetId, MethodInfo interceptedMethod)
        {
            this.Interceptor = interceptor;
            this.TargetId = targetId;
            this.InterceptedMethod = interceptedMethod;
        }

        public IFakeInterceptor Interceptor { get; private set; }

        public Guid TargetId { get; private set; }

        public MethodInfo InterceptedMethod { get; private set; }

        public override bool Equals(object obj)
        {
            var otherInvocation = obj as InterceptedInvocation;
            if (otherInvocation == null)
            {
                return false;
            }

            return this.Equals(otherInvocation);
        }

        public bool Equals(InterceptedInvocation otherInvocation)
        {
            return
                this.Interceptor == otherInvocation.Interceptor
                && this.TargetId == otherInvocation.TargetId
                && this.InterceptedMethod == otherInvocation.InterceptedMethod;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}