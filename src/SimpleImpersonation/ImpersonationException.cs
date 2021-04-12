﻿using System;
using System.ComponentModel;

namespace SimpleImpersonation
{
    /// <summary>
    /// Exception thrown when impersonation fails.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="ApplicationException"/> for backwards compatibility reasons.
    /// </remarks>
    public class ImpersonationException : ApplicationException
    {
        private readonly Win32Exception _win32Exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpersonationException"/> class from a specific <see cref="Win32Exception"/>.
        /// </summary>
        /// <param name="win32Exception">The exception to base this exception on.</param>
        public ImpersonationException(Win32Exception win32Exception)
            : base(win32Exception.Message, win32Exception)
        {
            // Note that the Message is generated inside the Win32Exception class via the Win32 FormatMessage function.

            _win32Exception = win32Exception;
        }

        /// <summary>
        /// Returns the Win32 error code handle for the exception.
        /// </summary>
        public int ErrorCode => _win32Exception.ErrorCode;

        /// <summary>
        /// Returns the Win32 native error code for the exception.
        /// </summary>
        public int NativeErrorCode => _win32Exception.NativeErrorCode;
    }
}
