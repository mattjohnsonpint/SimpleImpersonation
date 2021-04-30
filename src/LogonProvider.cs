namespace SimpleImpersonation
{
    /// <summary>
    /// Specifies the type of login provider used.
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa378184.aspx
    /// </summary>
    public enum LogonProvider
    {
        /// <summary>
        /// Use the standard logon provider for the system.
        /// The default security provider is negotiate, unless you pass NULL for the domain name and the user name
        /// is not in UPN format. In this case, the default provider is NTLM.
        /// NOTE: Windows 2000/NT:   The default security provider is NTLM.
        /// </summary>
        Default = 0,
        WINNT35 = 1,
        WINNT40 = 2,
        WINNT50 = 3,
    }
}
