using System;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace SimpleImpersonation
{
    /// <summary>
    /// Provides ability to run code within the context of a specific user.
    /// </summary>
    [Obsolete("This class is no longer needed.  Instead, use the result of UserCredentials.LogonUser with the WindowsIdentity.RunImpersontated or RunImpersonatedAsync methods.")]
    public static class Impersonation
    {
        /// <summary>
        /// Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="action">The action to perform.</param>
        [Obsolete("This method is no longer needed.  Instead, use the result of UserCredentials.LogonUser with the WindowsIdentity.RunImpersontated or RunImpersonatedAsync methods.")]
        public static void RunAsUser(UserCredentials credentials, LogonType logonType, Action action)
        {
            using (var tokenHandle = credentials.LogonUser(logonType))
            {
                WindowsIdentity.RunImpersonated(tokenHandle, action);
            }
        }

        /// <summary>
        /// Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        /// <param name="action">The action to perform.</param>
        [Obsolete("This method is no longer needed.  Instead, use the result of UserCredentials.LogonUser with the WindowsIdentity.RunImpersontated or RunImpersonatedAsync methods.")]
        public static void RunAsUser(UserCredentials credentials, LogonType logonType, LogonProvider logonProvider, Action action)
        {
            using (var tokenHandle = credentials.LogonUser(logonType, logonProvider))
            {
                WindowsIdentity.RunImpersonated(tokenHandle, action);
            }
        }

        /// <summary>
        /// Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="action">The action to perform, which accepts a <see cref="SafeAccessTokenHandle"/> to the user account as its only parameter.</param>
        [Obsolete("This method is no longer needed.  Instead, use the result of UserCredentials.LogonUser with the WindowsIdentity.RunImpersontated or RunImpersonatedAsync methods.")]
        public static void RunAsUser(UserCredentials credentials, LogonType logonType, Action<SafeAccessTokenHandle> action)
        {
            using (var tokenHandle = credentials.LogonUser(logonType))
            {
                WindowsIdentity.RunImpersonated(tokenHandle, () => action(tokenHandle));
            }
        }

        /// <summary>
        /// Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="logonProvider">The logon provider used when impersonating the user account.</param>
        /// <param name="action">The action to perform, which accepts a <see cref="SafeAccessTokenHandle"/> to the user account as its only parameter.</param>
        [Obsolete("This method is no longer needed.  Instead, use the result of UserCredentials.LogonUser with the WindowsIdentity.RunImpersontated or RunImpersonatedAsync methods.")]
        public static void RunAsUser(UserCredentials credentials, LogonType logonType, LogonProvider logonProvider, Action<SafeAccessTokenHandle> action)
        {
            using (var tokenHandle = credentials.LogonUser(logonType, logonProvider))
            {
                WindowsIdentity.RunImpersonated(tokenHandle, () => action(tokenHandle));
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
        [Obsolete("This method is no longer needed.  Instead, use the result of UserCredentials.LogonUser with the WindowsIdentity.RunImpersontated or RunImpersonatedAsync methods.")]
        public static T RunAsUser<T>(UserCredentials credentials, LogonType logonType, Func<T> function)
        {
            using (var tokenHandle = credentials.LogonUser(logonType))
            {
                return WindowsIdentity.RunImpersonated(tokenHandle, function);
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
        [Obsolete("This method is no longer needed.  Instead, use the result of UserCredentials.LogonUser with the WindowsIdentity.RunImpersontated or RunImpersonatedAsync methods.")]
        public static T RunAsUser<T>(UserCredentials credentials, LogonType logonType, LogonProvider logonProvider, Func<T> function)
        {
            using (var tokenHandle = credentials.LogonUser(logonType, logonProvider))
            {
                return WindowsIdentity.RunImpersonated(tokenHandle, function);
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
        [Obsolete("This method is no longer needed.  Instead, use the result of UserCredentials.LogonUser with the WindowsIdentity.RunImpersontated or RunImpersonatedAsync methods.")]
        public static T RunAsUser<T>(UserCredentials credentials, LogonType logonType, Func<SafeAccessTokenHandle, T> function)
        {
            using (var tokenHandle = credentials.LogonUser(logonType))
            {
                return WindowsIdentity.RunImpersonated(tokenHandle, () => function(tokenHandle));
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
        [Obsolete("This method is no longer needed.  Instead, use the result of UserCredentials.LogonUser with the WindowsIdentity.RunImpersontated or RunImpersonatedAsync methods.")]
        public static T RunAsUser<T>(UserCredentials credentials, LogonType logonType, LogonProvider logonProvider, Func<SafeAccessTokenHandle, T> function)
        {
            using (var tokenHandle = credentials.LogonUser(logonType, logonProvider))
            {
                return WindowsIdentity.RunImpersonated(tokenHandle, () => function(tokenHandle));
            }
        }
    }
}
