using System;
using System.IO;
using System.Runtime.CompilerServices;

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
#if !NETSTANDARD1_0_OR_GREATER && !NET45_OR_GREATER
        public static void CopyTo(this Stream stream, Stream dest)
        {
            var buffer = new byte[81920];
            int writedSize;
            while ((writedSize = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                dest.Write(buffer, 0, writedSize);
            }
        }
#endif
        public static void ThrowArgumentNullExceptionIfNull(object argument, [CallerArgumentExpression("argument")] string paramName = null)
        {
            if (argument is null)
                ThrowArgumentNullException(paramName);

            static void ThrowArgumentNullException(string paramName)
                => throw new ArgumentNullException(paramName);
        }

        public static void ThrowArgumentOutOfRangeException(string paramName)
            => throw new ArgumentOutOfRangeException(paramName);
        public static void ThrowArgumentOutOfRangeException(string paramName, string message)
            => throw new ArgumentOutOfRangeException(paramName, message);
    }
}
