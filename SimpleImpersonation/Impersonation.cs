using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace SimpleImpersonation
{
    /// <summary>
    /// Impersonates a specific user for the lifetime of this object.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed class Impersonation : IDisposable
    {
        private readonly SafeTokenHandle _handle;
        private readonly WindowsImpersonationContext _context;

        /// <summary>
        /// Attempts to impersonate the user with the supplied information.
        /// Call from a <c>using</c> block, or ensure that <see cref="Dispose"/> is called on
        /// the resulting <see cref="Impersonation"/> object upon completion.
        /// </summary>
        /// <param name="domain">The domain name or machine name, or <c>.</c> for the local machine.</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="logonType">The logon type.</param>
        /// <returns>An <see cref="Impersonation"/> object, which should be disposed when done impersonating the user.</returns>
        public static Impersonation LogonUser(string domain, string username, string password, LogonType logonType)
        {
            return new Impersonation(domain, username, password, logonType);
        }

        /// <summary>
        /// Attempts to impersonate the user with the supplied information.
        /// Call from a <c>using</c> block, or ensure that <see cref="Dispose"/> is called on
        /// the resulting <see cref="Impersonation"/> object upon completion.
        /// </summary>
        /// <param name="domain">The domain name or machine name, or <c>.</c> for the local machine.</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The password, as a <see cref="SecureString"/>.</param>
        /// <param name="logonType">The logon type.</param>
        /// <returns>An <see cref="Impersonation"/> object, which should be disposed when done impersonating the user.</returns>
        public static Impersonation LogonUser(string domain, string username, SecureString password, LogonType logonType)
        {
            return new Impersonation(domain, username, password, logonType);
        }

        private Impersonation(string domain, string username, SecureString password, LogonType logonType)
        {
            IntPtr token;
            IntPtr passPtr = Marshal.SecureStringToGlobalAllocUnicode(password);

            bool success;
            try
            {
                success = NativeMethods.LogonUser(username, domain, passPtr, (int)logonType, 0, out token);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(passPtr);
            }

            CompleteImpersonation(success, token, out _handle, out _context);
        }

        private Impersonation(string domain, string username, string password, LogonType logonType)
        {
            IntPtr token;
            bool success = NativeMethods.LogonUser(username, domain, password, (int)logonType, 0, out token);
            CompleteImpersonation(success, token, out _handle, out _context);
        }

        private void CompleteImpersonation(bool success, IntPtr token, out SafeTokenHandle handle, out WindowsImpersonationContext context)
        {
            if (!success)
            {
                var errorCode = Marshal.GetLastWin32Error();

                if (token != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(token);
                }
                
                throw new Win32Exception(errorCode);
            }

            handle = new SafeTokenHandle(token);
            context = WindowsIdentity.Impersonate(_handle.DangerousGetHandle());
        }

        /// <summary>
        /// Disposes the <see cref="Impersonation"/> object, ending impersonation and restoring the original user.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            _handle.Dispose();
        }
    }
}
