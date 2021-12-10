using System;

namespace SimpleImpersonation
{
    /// <summary>
    /// Specifies the type of login provider used.
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa378184.aspx
    /// </summary>
    public enum LogonProvider
    {
        /// <summary>
        /// Use the standard logon provider for the system. The default provider is "Negotiate".
        /// However, if you pass NULL for the domain name and the user name is not in UPN format, then the default provider is "NTLM".
        /// </summary>
        Default = 0,

        /// <summary>
        /// Use the NTLM logon provider.
        /// </summary>
        NTLM = 2,

        /// <summary>
        /// Use the Negotiate logon provider.
        /// </summary>
        Negotiate = 3,

        /// <summary>
        /// Use the WINNT35 logon provider.
        /// </summary>
        [Obsolete("This logon provider has been deprecated and should no longer be used.")]
        WINNT35 = 1,

        /// <summary>
        /// Use the WINNT40 logon provider.
        /// </summary>
        [Obsolete("This enumeration value is obsolete.  To use this provider, specify LogonProvider.NTLM instead.")]
        WINNT40 = 2,

        /// <summary>
        /// Use the WINNT50 logon provider.
        /// </summary>
        [Obsolete("This enumeration value is obsolete.  To use this provider, specify LogonProvider.Negotiate instead.")]
        WINNT50 = 3,
    }
}
