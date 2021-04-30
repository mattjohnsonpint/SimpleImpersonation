using System;
using System.Security.Principal;
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
        /// Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="action">The action to perform.</param>
        public static void RunAsUser(UserCredentials credentials, LogonType logonType, Action action)
        {
            using (var tokenHandle = credentials.Impersonate(logonType))
            {
                RunImpersonated(tokenHandle, _ => action());
            }
        }

        /// <summary>
        /// Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        /// <param name="action">The action to perform.</param>
        public static void RunAsUser(UserCredentials credentials, LogonType logonType, LogonProvider logonProvider, Action action)
        {
            using (var tokenHandle = credentials.Impersonate(logonType, logonProvider))
            {
                RunImpersonated(tokenHandle, _ => action());
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
                RunImpersonated(tokenHandle, action);
            }
        }

        /// <summary>
        /// Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        /// <param name="action">The action to perform, which accepts a <see cref="SafeAccessTokenHandle"/> to the user account as its only parameter.</param>
        public static void RunAsUser(UserCredentials credentials, LogonType logonType, LogonProvider logonProvider, Action<SafeAccessTokenHandle> action)
        {
            using (var tokenHandle = credentials.Impersonate(logonType, logonProvider))
            {
                RunImpersonated(tokenHandle, action);
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
                return RunImpersonated(tokenHandle, _ => function());
            }
        }

        /// <summary>
        /// Impersonates a specific user account to execute the specified function.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        /// <param name="function">The function to execute, which accepts a <see cref="SafeAccessTokenHandle"/> to the user account as its only parameter.</param>
        /// <returns>The result of executing the function.</returns>
        public static T RunAsUser<T>(UserCredentials credentials, LogonType logonType, LogonProvider logonProvider, Func<T> function)
        {
            using (var tokenHandle = credentials.Impersonate(logonType, logonProvider))
            {
                return RunImpersonated(tokenHandle, _ => function());
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
                return RunImpersonated(tokenHandle, function);
            }
        }

        /// <summary>
        /// Impersonates a specific user account to execute the specified function.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        /// <param name="function">The function to execute.</param>
        /// <returns>The result of executing the function.</returns>
        public static T RunAsUser<T>(UserCredentials credentials, LogonType logonType, LogonProvider logonProvider, Func<SafeAccessTokenHandle, T> function)
        {
            using (var tokenHandle = credentials.Impersonate(logonType, logonProvider))
            {
                return RunImpersonated(tokenHandle, function);
            }
        }

        private static void RunImpersonated(SafeAccessTokenHandle tokenHandle, Action<SafeAccessTokenHandle> action)
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

        private static T RunImpersonated<T>(SafeAccessTokenHandle tokenHandle, Func<SafeAccessTokenHandle, T> function)
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
