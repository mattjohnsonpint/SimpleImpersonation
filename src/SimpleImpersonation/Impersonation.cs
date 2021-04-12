using System;
using Microsoft.Win32.SafeHandles;

#if !NETSTANDARD
using System.Security.Permissions;
#endif

namespace SimpleImpersonation
{
    /// <summary>
    /// Provides ability to run code within the context of a specific user.
    /// </summary>
#if !NETSTANDARD
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
#endif
    public static class Impersonation
    {
        /// <summary>
        /// Provides access to <see cref="LowLevelImpersonation"/> for more fine-grained control.
        /// </summary>
        public static readonly ILowLevelImpersonation LowLevel = new LowLevelImpersonation(); 
        
        /// <summary>
        /// Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="action">The action to perform.</param>
        public static void RunAsUser(UserCredentials credentials, LogonType logonType, Action action)
        {
            using (var tokenHandle = credentials.Impersonate(logonType))
            {
                LowLevel.RunImpersonated(tokenHandle, _ => action());
            }
        }

        /// <summary>
        /// Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="action">The action to perform, which accepts a <see cref="SafeAccessTokenHandle"/> to the user account as its only parameter.</param>
        public static void RunAsUser(UserCredentials credentials, LogonType logonType, Action<SafeAccessTokenHandle> action)
        {
            using (var tokenHandle = credentials.Impersonate(logonType))
            {
                LowLevel.RunImpersonated(tokenHandle, action);
            }
        }

        /// <summary>
        /// Impersonates a specific user account to execute the specified function.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="function">The function to execute, which accepts a <see cref="SafeAccessTokenHandle"/> to the user account as its only parameter.</param>
        /// <returns>The result of executing the function.</returns>
        public static T RunAsUser<T>(UserCredentials credentials, LogonType logonType, Func<T> function)
        {
            using (var tokenHandle = credentials.Impersonate(logonType))
            {
                return LowLevel.RunImpersonated(tokenHandle, _ => function());
            }
        }

        /// <summary>
        /// Impersonates a specific user account to execute the specified function.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="function">The function to execute.</param>
        /// <returns>The result of executing the function.</returns>
        public static T RunAsUser<T>(UserCredentials credentials, LogonType logonType, Func<SafeAccessTokenHandle, T> function)
        {
            using (var tokenHandle = credentials.Impersonate(logonType))
            {
                return LowLevel.RunImpersonated(tokenHandle, function);
            }
        }
    }
}
