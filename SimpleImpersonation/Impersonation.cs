using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace SimpleImpersonation
{
    /// <summary>
    /// Wrapper for Win32's LogonUser function and the WindowsIdentity function Impersonate.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed class Impersonation : IDisposable
    {
        private readonly SafeTokenHandle _handle;
        private WindowsImpersonationContext _context;

        /// <summary>
        /// Creates a new Impersonation object and impersonates the given user.
        /// </summary>
        public static Impersonation LogonUser(string domain, string username, string password, LogonType logonType)
        {
            var impersonation = new Impersonation(domain, username, password, logonType);
            impersonation.Impersonate();
            return impersonation;
        }

        /// <summary>
        /// Creates a new Impersonation object with a login for the given account.
        /// </summary>
        public Impersonation(string domain, string username, string password, LogonType logonType)
        {
            IntPtr handle;
            var ok = NativeMethods.LogonUser(username, domain, password, (int)logonType, 0, out handle);
            if (!ok)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new ApplicationException(string.Format("Could not impersonate the elevated user.  LogonUser returned error code {0}.", errorCode));
            }

            _handle = new SafeTokenHandle(handle);
            _context = null;
        }

        /// <summary>
        /// Disposes of the Impersonation, reverting to the previous user, if necessary.
        /// </summary>
        public void Dispose()
        {
            Revert();
            _handle.Dispose();
        }

        /// <summary>
        /// Starts impersonation of the object's account.  If impersonation is already active, nothing happens.
        /// </summary>
        public void Impersonate()
        {
            if (_context != null) {
                return;
            }
            _context = WindowsIdentity.Impersonate(_handle.DangerousGetHandle());
        }

        /// <summary>
        /// Reverts to the previous user, if currently impersonating.
        /// </summary>
        public void Revert()
        {
            if (_context != null) {
                _context.Dispose();
                _context = null;
            }
        }
    }
}