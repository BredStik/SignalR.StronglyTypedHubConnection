using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.StronglyTypedHubConnection
{
    public static class HubConnectionExtensions
    {
        public static async Task InvokeAsync<THubType>(this HubConnection hubConnection, Expression<Action<THubType>> call, CancellationToken cancellationToken)
        {
            var methodCall = (MethodCallExpression)call.Body;
            var methodName = methodCall.Method.Name;
            var arguments = methodCall.Arguments.Select(GetValue).ToArray();

            await hubConnection.InvokeCoreAsync(methodName, arguments, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<TResult> InvokeAsync<THubType, TResult>(this HubConnection hubConnection, Expression<Func<THubType, TResult>> call, CancellationToken cancellationToken)
        {
            var methodCall = (MethodCallExpression)call.Body;
            var methodName = methodCall.Method.Name;
            var arguments = methodCall.Arguments.Select(GetValue).ToArray();

            return (TResult)await hubConnection.InvokeCoreAsync(methodName, typeof(TResult), arguments, cancellationToken).ConfigureAwait(false);
        }

        private static object GetValue(Expression valueExpression)
        {
            var objectMember = Expression.Convert(valueExpression, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            return getterLambda.Compile()();
        }
    }
}
