namespace AsyncInitialization.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;
    using Appccelerate;

    public static class Reflector<TTarget>
    {
        private const string MethodIsNotCall = "Specified method is not a method call expression.";

        /// <summary>
        /// Gets the method represented by the lambda expression.
        /// </summary>
        /// <param name="methodSelector">An expression that invokes a method.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="methodSelector"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="methodSelector"/> does not represent a method call.</exception>
        /// <returns>The method info represented by the lambda expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Required for simplified usage of methods.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Require concrete type to ensure it is a correct lambda expression.")]
        public static MethodInfo GetMethod(Expression<Action<TTarget>> methodSelector)
        {
            return Reflector<TTarget>.GetMethodInfo(methodSelector);
        }

        /// <summary>
        /// Gets the method info represented by the lambda expression.
        /// </summary>
        /// <param name="methodSelector">An expression that invokes a method.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="methodSelector"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="methodSelector"/> does not represent a method call.</exception>
        /// <returns>The method info represented by the lambda expression.</returns>
        private static MethodInfo GetMethodInfo(LambdaExpression methodSelector)
        {
            Ensure.ArgumentNotNull(methodSelector, "methodSelector");
            Ensure.ArgumentMatches(methodSelector.Body.NodeType == ExpressionType.Call, Reflector<TTarget>.MethodIsNotCall);

            var callExpression = (MethodCallExpression)methodSelector.Body;
            return callExpression.Method;
        }
    }
}