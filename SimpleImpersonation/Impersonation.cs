﻿using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace SimpleImpersonation
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed class Impersonation : IDisposable
    {
        private readonly SafeTokenHandle _handle;
        private WindowsImpersonationContext _context;

        public static Impersonation LogonUser(string domain, string username, string password, LogonType logonType)
        {
            var impersonation = new Impersonation(domain, username, password, logonType);
            impersonation.Impersonate();
            return impersonation;
        }

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

        public void Dispose()
        {
            Revert();
            _handle.Dispose();
        }

        public void Impersonate()
        {
            if (_context != null) {
                return;
            }
            _context = WindowsIdentity.Impersonate(_handle.DangerousGetHandle());
        }

        public void Revert()
        {
            if (_context != null) {
                _context.Dispose();
                _context = null;
            }
        }
    }
}