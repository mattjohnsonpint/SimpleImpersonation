using System;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace SimpleImpersonation
{
    public class LowLevelImpersonation
    {
        internal LowLevelImpersonation()
        {
        }

        public SafeAccessTokenHandle Impersonate(UserCredentials userCredentials, LogonType logonType)
        {
            return userCredentials.Impersonate(logonType);
        }
        
        public void RunImpersonated(SafeAccessTokenHandle tokenHandle, Action<SafeAccessTokenHandle> action)
        {
#if NETSTANDARD || NET46
            WindowsIdentity.RunImpersonated(tokenHandle, () => action(tokenHandle));
#else
            using (var context = WindowsIdentity.Impersonate(tokenHandle.DangerousGetHandle()))
            {
                action(tokenHandle);
            }
#endif
        }

        public T RunImpersonated<T>(SafeAccessTokenHandle tokenHandle, Func<SafeAccessTokenHandle, T> function)
        {
#if NETSTANDARD || NET46
            return WindowsIdentity.RunImpersonated(tokenHandle, () => function(tokenHandle));
#else
            using (var context = WindowsIdentity.Impersonate(tokenHandle.DangerousGetHandle()))
            {
                return function(tokenHandle);
            }
#endif
        }
    }
}