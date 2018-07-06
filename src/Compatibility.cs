#if !(NETSTANDARD || NET46)
using System;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Provides safe access to a token handle.
    /// </summary>
    public sealed class SafeAccessTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeAccessTokenHandle(IntPtr handle)
            : base(true)
        {
            this.handle = handle;
        }

        /// <inheritdoc/>
        protected override bool ReleaseHandle()
        {
            return SimpleImpersonation.NativeMethods.CloseHandle(handle);
        }
    }
}
#endif

#if NET20
namespace System
{
    /// <summary>
    /// Encapsulates a method that has no parameters and does not return a value.
    /// </summary>
    public delegate void Action();

    /// <summary>
    /// Encapsulates a method that has no parameters and returns a value of the type
    /// specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult Func<TResult>();

    /// <summary>
    /// Encapsulates a method that has one parameter and returns a value of the type
    /// specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <param name="arg">The parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult Func<T, TResult>(T arg);
}
#endif
