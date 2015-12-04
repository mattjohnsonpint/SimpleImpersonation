using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace SimpleImpersonation
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed class Impersonation : IDisposable
    {
        private SafeTokenHandle _handle;
        private WindowsImpersonationContext _context;

        public static Impersonation LogonUser(string domain, string username, string password, LogonType logonType)
        {
            return new Impersonation(domain, username, password, logonType);
        }

        public static Impersonation LogonUser(string domain, string username, SecureString password, LogonType logonType)
        {
            return new Impersonation(domain, username, password, logonType);
        }

        private void CompleteImpersonation(bool ok, IntPtr handle)
        {
            if (!ok)
            {
                var errorCode = Marshal.GetLastWin32Error();

                if (handle != IntPtr.Zero)
                   NativeMethods.CloseHandle(handle);

                throw new ApplicationException(string.Format("Could not impersonate the elevated user.  LogonUser returned error code {0}.", errorCode));
            }

            _handle = new SafeTokenHandle(handle);
            _context = WindowsIdentity.Impersonate(_handle.DangerousGetHandle());
        }

        private Impersonation(string domain, string username, SecureString password, LogonType logonType)
        {
            IntPtr handle;
            var passPtr = Marshal.SecureStringToGlobalAllocUnicode(password);
            bool ok;
            try
            {
                ok = NativeMethods.LogonUser(username, domain, passPtr, (int)logonType, 0, out handle);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(passPtr);
            }

            CompleteImpersonation(ok, handle);
        }

        private Impersonation(string domain, string username, string password, LogonType logonType)
        {
            IntPtr handle;
            var ok = NativeMethods.LogonUser(username, domain, password, (int)logonType, 0, out handle);
            CompleteImpersonation(ok, handle);
        }

        public void Dispose()
        {
            _context.Dispose();
            _handle.Dispose();
        }
    }
}