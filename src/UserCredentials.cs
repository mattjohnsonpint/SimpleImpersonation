using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace SimpleImpersonation
{
    /// <summary>
    /// Represents the credentials to be used for impersonation.
    /// </summary>
    public class UserCredentials
    {
        private readonly string _domain;
        private readonly string _username;
        private readonly string _password;
        private readonly SecureString _securePassword;

        /// <summary>
        /// Credentials for "NT AUTHORITY\NETWORK SERVICE"
        /// </summary>
        public static UserCredentials NetworkService => new UserCredentials("NETWORK SERVICE");

        /// <summary>
        /// Credentials for "NT AUTHORITY\SYSTEM"
        /// </summary>
        public static UserCredentials LocalSystem => new UserCredentials("SYSTEM");

        /// <summary>
        /// Credentials for "NT AUTHORITY\LOCAL SERVICE"
        /// </summary>
        public static UserCredentials LocalService => new UserCredentials("LOCAL SERVICE");

        /// <summary>
        /// Creates a <see cref="UserCredentials"/> class based on a username and plaintext password.
        /// The username can contain a domain name if specified in <c>domain\user</c> or <c>user@domain</c> form.
        /// If no domain is provided, a local computer user account is assumed.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public UserCredentials(string username, string password)
        {
            ValidateUserWithoutDomain(username);
            ValidatePassword(password);

            SplitDomainFromUsername(ref username, out var domain);

            _domain = domain;
            _username = username;
            _password = password;
        }

        /// <summary>
        /// Creates a <see cref="UserCredentials"/> class based on a domain, username, and plaintext password.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public UserCredentials(string domain, string username, string password)
        {
            ValidateDomainAndUser(domain, username);
            ValidatePassword(password);

            _domain = domain;
            _username = username;
            _password = password;
        }

        /// <summary>
        /// Creates a <see cref="UserCredentials"/> class based on a username and password, where the password is provided as a <see cref="SecureString"/>.
        /// The username can contain a domain name if specified in <c>domain\user</c> or <c>user@domain</c> form.
        /// If no domain is provided, a local computer user account is assumed.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public UserCredentials(string username, SecureString password)
        {
            ValidateUserWithoutDomain(username);
            ValidatePassword(password);

            SplitDomainFromUsername(ref username, out var domain);

            _domain = domain;
            _username = username;
            _securePassword = password;
        }

        /// <summary>
        /// Creates a <see cref="UserCredentials"/> class based on a domain, username, and password, where the password is provided as a <see cref="SecureString"/>.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public UserCredentials(string domain, string username, SecureString password)
        {
            ValidateDomainAndUser(domain, username);
            ValidatePassword(password);

            _domain = domain;
            _username = username;
            _securePassword = password;
        }

        private UserCredentials(string builtInAccount)
        {
            _domain = "NT AUTHORITY";
            _username = builtInAccount;
            _password = string.Empty;
        }

#if NETFRAMEWORK
        /// <summary>
        /// Invokes the Win32 <a href="https://docs.microsoft.com/windows/win32/api/winbase/nf-winbase-logonusera">LogonUser</a>
        /// API to log on with the credentials specified in the creation of this object.
        /// The result is a <see cref="SafeAccessTokenHandle"/> which can subsequently be used with methods such as 
        /// <see cref="WindowsIdentity.RunImpersonated"/>.
        /// The handle should be used with a <c>using</c> block or statement, to log out the user when done.
        /// </summary>
        /// <param name="logonType">
        /// The Windows logon type.  Typically <see cref="LogonType.NewCredentials"/> for simple network access,
        /// or <see cref="LogonType.Interactive"/> for full logon in desktop applications.
        /// </param>
        /// <param name="logonProvider">The Windows logon provider.  Leave as default if uncertain.</param>
        /// <returns>A <see cref="SafeAccessTokenHandle"/> for the newly logged in user.</returns>
#else
        /// <summary>
        /// Invokes the Win32 <a href="https://docs.microsoft.com/windows/win32/api/winbase/nf-winbase-logonusera">LogonUser</a>
        /// API to log on with the credentials specified in the creation of this object.
        /// The result is a <see cref="SafeAccessTokenHandle"/> which can subsequently be used with methods such as 
        /// <see cref="WindowsIdentity.RunImpersonated"/> or <see cref="WindowsIdentity.RunImpersonatedAsync"/>.
        /// The handle should be used with a <c>using</c> block or statement, to log out the user when done.
        /// </summary>
        /// <param name="logonType">
        /// The Windows logon type.  Typically <see cref="LogonType.NewCredentials"/> for simple network access,
        /// or <see cref="LogonType.Interactive"/> for full logon in desktop applications.
        /// </param>
        /// <param name="logonProvider">The Windows logon provider.  Leave as default if uncertain.</param>
        /// <returns>A <see cref="SafeAccessTokenHandle"/> for the newly logged in user.</returns>
#endif
        public SafeAccessTokenHandle LogonUser(LogonType logonType, LogonProvider logonProvider = LogonProvider.Default)
        {
            if (_securePassword == null)
            {
                if (!NativeMethods.LogonUser(_username, _domain, _password, (int)logonType, (int)logonProvider, out var tokenHandle))
                    HandleError(tokenHandle);

                return new SafeAccessTokenHandle(tokenHandle);
            }

            var passPtr = Marshal.SecureStringToGlobalAllocUnicode(_securePassword);

            try
            {
                if (!NativeMethods.LogonUser(_username, _domain, passPtr, (int)logonType, (int)logonProvider, out var tokenHandle))
                    HandleError(tokenHandle);

                return new SafeAccessTokenHandle(tokenHandle);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(passPtr);
            }
        }

        private static void HandleError(IntPtr tokenHandle)
        {
            var errorCode = Marshal.GetLastWin32Error();

            if (tokenHandle != IntPtr.Zero)
                NativeMethods.CloseHandle(tokenHandle);

            throw new ImpersonationException(new Win32Exception(errorCode));
        }

        private static void ValidateDomainAndUser(string domain, string username)
        {
            if (domain == null)
                throw new ArgumentNullException(nameof(domain), "Domain cannot be null.");

            if (username == null)
                throw new ArgumentNullException(nameof(username), "Username cannot be null.");

            if (domain.Trim() == string.Empty)
                throw new ArgumentException("Domain cannot be empty or consist solely of whitespace characters.", nameof(domain));

            if (username.Trim() == string.Empty)
                throw new ArgumentException("Username cannot be empty or consist solely of whitespace characters.", nameof(username));

            if (domain.IndexOfAny(new[] { '\\', '@' }) != -1)
                throw new ArgumentException("Domain cannot contain \\ or @ characters.", nameof(domain));

            if (username.IndexOfAny(new[] { '\\', '@' }) != -1)
                throw new ArgumentException("Username cannot contain \\ or @ characters when domain is provided separately.", nameof(username));
        }

        private static void ValidateUserWithoutDomain(string username)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), "Username cannot be null.");

            if (username.Trim() == string.Empty)
                throw new ArgumentException("Username cannot be empty or consist solely of whitespace characters.", nameof(username));

            int separatorCount = 0;
            foreach (var c in username)
            {
                if (c == '\\' || c == '@')
                    separatorCount++;
            }

            if (separatorCount == 0)
                return;

            if (separatorCount > 1)
                throw new ArgumentException("Username cannot contain more than one \\ or @ character.", nameof(username));

            var firstChar = username[0];
            var lastChar = username[username.Length - 1];
            if (firstChar == '\\' || firstChar == '@' || lastChar == '\\' || lastChar == '@')
                throw new ArgumentException("Username cannot start or end with a \\ or @ character.");
        }

        private static void ValidatePassword(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password), "Password cannot be null.");

            if (password.Trim() == string.Empty)
                throw new ArgumentException("Password cannot be empty or consist solely of whitespace characters.", nameof(password));
        }

        private static void ValidatePassword(SecureString password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password), "Password cannot be null.");

            if (password.Length == 0)
                throw new ArgumentException("Password cannot be empty.", nameof(password));
        }

        private static void SplitDomainFromUsername(ref string username, out string domain)
        {
            // Note: We only split for domain\user form, because user@domain form is accepted by LogonUser.

            var parts = username.Split('\\');
            if (parts.Length == 2)
            {
                domain = parts[0];
                username = parts[1];
            }
            else
            {
                domain = null;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _domain == null ? _username : _username + "@" + _domain;
        }
    }
}
