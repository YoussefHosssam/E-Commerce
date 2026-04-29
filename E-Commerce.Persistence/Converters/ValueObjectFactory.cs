using System.Reflection;
using System.Runtime.ExceptionServices;
using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.Infrastructure.Persistence.Converters;

internal static class ValueObjectFactory
{
    public static T FromString<T>(string value) where T : struct
    {
        var type = typeof(T);
        var create = type.GetMethod("Create", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });

        try
        {
            if (create is not null)
            {
                return (T)create.Invoke(null, new object[] { value })!;
            }

            return (T)Activator.CreateInstance(type, value)!;
        }
        catch (TargetInvocationException exception) when (exception.InnerException is AppException inner)
        {
            ExceptionDispatchInfo.Capture(inner).Throw();
            throw;
        }
    }
}
