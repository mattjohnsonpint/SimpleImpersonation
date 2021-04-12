using System;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace SimpleImpersonation
{
    /// <summary>
    /// Provides ability to get safe access token handle and run code within the context of a token.
    /// This class gives the user more fine-grained control over disposal of the safe access token handle.
    /// </summary>
    public class LowLevelImpersonation
    {
        internal LowLevelImpersonation()
        {
        }

        /// <summary>
        /// Impersonates a specific user account to get a safe access token handle.
        /// </summary>
        /// <param name="userCredentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <returns>The safe access token handle.</returns>
        public SafeAccessTokenHandle Impersonate(UserCredentials userCredentials, LogonType logonType)
        {
            return userCredentials.Impersonate(logonType);
        }
        
        /// <summary>
        /// Impersonates a specific user account to execute the specified action.
        /// </summary>
        /// <param name="tokenHandle">The credentials of the user account to impersonate.</param>
        /// <param name="action">The action to execute, which accepts a <see cref="SafeAccessTokenHandle"/> to the user account as its only parameter.</param>
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

        /// <summary>
        /// Impersonates a specific user account to execute the specified action.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="tokenHandle">The credentials of the user account to impersonate.</param>
        /// <param name="function">The function to execute, which accepts a <see cref="SafeAccessTokenHandle"/> to the user account as its only parameter.</param>
        /// <returns>The result of executing the function.</returns>
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