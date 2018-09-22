using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TamaChanBot.API;

namespace TamaChanBot.Core
{
    public sealed class Command
    {
        public readonly string name;
        public readonly MethodInfo method;
        public readonly Permission permissionFlag;
        public readonly bool botOwnerOnly;
        public readonly bool isNsfw;
        public readonly TamaChanModule module;
        private readonly string example;
        private readonly Func<object[], Task<object>> invocationDelegate;

        public string Description { get; set; }

        public string Example
        {
            get => $"{TamaChan.Instance.botSettings.commandPrefix}{name} {example}";
        }

        internal Command(string name, MethodInfo method, Permission permissionFlag, bool botOwnerOnly, bool isNsfw, string example, TamaChanModule module)
        {
            this.name = name;
            this.method = method;
            this.permissionFlag = permissionFlag;
            this.botOwnerOnly = botOwnerOnly;
            this.isNsfw = isNsfw;
            this.example = example;
            this.module = module;

            invocationDelegate = CreateInvocationDelegate();
        }

        public Task<object> Invoke(params object[] pars) => invocationDelegate(pars);

        private Func<object[], Task<object>> CreateInvocationDelegate()
        {
            //This represents the array of objects we pass to the delegate
            ParameterExpression arguments_array = Expression.Parameter(typeof(object[]));

            //We need the method's parameter information to properly convert the arguments
            ParameterInfo[] method_parameters = method.GetParameters();

            //The parameters we will be sending to the method, after casting and converting as necessary
            Expression[] calling_parameters = new Expression[method_parameters.Length];

            for (int i = 0; i < calling_parameters.Length; i++)
            {
                //Fetch the information for the specific parameter we're working with
                ParameterInfo parameter_info = method_parameters[i];

                //Pull the argument out from the arguments array
                Expression argument = Expression.ArrayIndex(arguments_array, Expression.Constant(i));

                //A bit of casting/converting to make calling the command's method actually work, can't pass an object as an int after all >.>
                calling_parameters[i] = CreateConversionExpression(argument, parameter_info.ParameterType);
            }

            //The actual part that calls the command's method! \o/
            MethodCallExpression call_expression = Expression.Call(Expression.Constant(module), method, calling_parameters);

            //Now that we've set up the method call, wrap it in some converstion methods to make it return a Task<object>
            Expression conversion_expression = ConvertReturnType(call_expression);

            //Finally wrap it up in a lambda expression, and then compile it into a delegate
            return Expression.Lambda<Func<object[], Task<object>>>(conversion_expression, arguments_array).Compile();
        }

        private Expression CreateConversionExpression(Expression original, Type t)
        {
            //If it's a generic type
            if (t.IsConstructedGenericType)
            {
                //Find out what the base type for it is
                Type base_type = t.GetGenericTypeDefinition();

                //If the base type is nullable, in other words if this parameter type is a nullable
                if (base_type == typeof(Nullable<>))
                {
                    //Cast directly to the nullable type, Works if the passed value is null already, or was passed as a nullable
                    Expression direct_convert = Expression.Convert(original, t);

                    //Cast to the wrapped type first, and then convert to a nullable. This is required for if the parameter is passed as the base type,
                    // converting an integer that's seen as an object to an int? throws an exception otherwise.
                    Expression workaround_convert = Expression.Convert(Expression.Convert(original, t.GetGenericArguments()[0]), t);

                    //Make it conditional, so that we type check it first. If it's a nullable already, convert it directly. If it isn't, then do our workaround method.
                    return Expression.Condition(Expression.TypeIs(original, t), direct_convert, workaround_convert);
                }
            }

            return Expression.Convert(original, t);
        }

        private Expression ConvertReturnType(Expression original)
        {
            Type return_type = original.Type;

            //If the method returns a simple task without a value
            if (return_type == typeof(Task))
            {
                //Fetches the function below as a delegate
                Func<Task, Task<object>> converter = AwaitAndReturnNull;

                //Calls that function, which handles returning null
                return Expression.Call(converter.Method, original);
            }

            //If the method returns a task value, since we've already checked valueless, this one would be a task with a value.
            if (typeof(Task).IsAssignableFrom(return_type))
            {
                //The type the task returns
                Type generic_parameter = return_type.GetGenericArguments()[0];

                //Fetches the function below as a delegate
                Func<Task<object>, Task<object>> converter_base = AwaitAndReturnAsObject;

                //Takes our base method, fetches the generic definition of it, and sets T to the type we will pass to it
                MethodInfo converter_method = converter_base.Method.GetGenericMethodDefinition().MakeGenericMethod(generic_parameter);

                //Calls the function, which handles returning the value as an object
                return Expression.Call(converter_method, original);
            }

            Func<Func<object>, Task<object>> task_run = Task.Run;

            Expression<Func<object>> lambda;

            if (return_type == typeof(void))
            {
                //Make a block expression that returns null
                Expression block = Expression.Block(original, Expression.Constant(null, typeof(object)));

                //Wrap that block expression in a lambda for passing to Task.Run
                lambda = Expression.Lambda<Func<object>>(block);
            }
            else
                //Wrap the original in a lambda expression, a lambda that returns an object
                lambda = Expression.Lambda<Func<object>>(original);

            //Finally call Task.Run to make the method run asynchronously, as well as return as a task
            return Expression.Call(task_run.Method, lambda);
        }

        private static async Task<object> AwaitAndReturnAsObject<T>(Task<T> task)
            => await task;

        private static async Task<object> AwaitAndReturnNull(Task task)
        {
            await task;
            return null;
        }
    }
}
