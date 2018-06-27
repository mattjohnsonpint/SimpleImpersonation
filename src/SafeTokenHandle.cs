using System;
using Microsoft.Win32.SafeHandles;

namespace SimpleImpersonation
{
    /// <summary>
    /// provides a managed wrapper for a token handle
    /// </summary>
    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeTokenHandle(IntPtr handle)
            : base(true)
        {
            this.handle = handle;
        }

        /// <inheritdoc/>
        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(handle);
        }
    }
}