using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace SimpleImpersonation
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed class Impersonation : IDisposable
    {
        private readonly SafeTokenHandle _handle;
        private readonly WindowsImpersonationContext _context;

        public static Impersonation LogonUser(string domain, string username, string password, LogonType logonType)
        {
            return new Impersonation(domain, username, password, logonType);
        }

        private Impersonation(string domain, string username, string password, LogonType logonType)
        {
            var ok = NativeMethods.LogonUser(username, domain, password, (int)logonType, 0, out _handle);
            if (!ok)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new ApplicationException(string.Format("Could not impersonate the elevated user.  LogonUser returned error code {0}.", errorCode));
            }

            _context = WindowsIdentity.Impersonate(_handle.DangerousGetHandle());
        }

        public void Dispose()
        {
            _context.Dispose();
            _handle.Dispose();
        }
    }
}