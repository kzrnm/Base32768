using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#if !NETCOREAPP3_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        public string ParameterName { get; }
    }
}
#endif

namespace Kzrnm.Convert.Base32768
{
    internal static class Utils
    {
        public static void ThrowArgumentNullExceptionIfNull(object argument, [CallerArgumentExpression("argument")] string paramName = null)
        {
            if (argument is null)
                throw new ArgumentNullException(paramName);
        }
    }
}
