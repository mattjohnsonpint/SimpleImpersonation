namespace SimpleImpersonation
{
    /// <summary>
    /// Specifies the type of login used.
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa378184.aspx
    /// </summary>
    public enum LogonType
    {
        /// <summary>
        /// This logon type is intended for users who will be interactively using the computer, such as a user being logged
        /// on by a terminal server, remote shell, or similar process. This logon type has the additional expense of caching
        /// logon information for disconnected operations; therefore, it is inappropriate for some client/server applications,
        /// such as a mail server.
        /// </summary>
        Interactive = 2,

        /// <summary>
        /// This logon type is intended for high performance servers to authenticate plaintext passwords.
        /// The LogonUser function does not cache credentials for this logon type.
        /// </summary>
        Network = 3,

        /// <summary>
        /// This logon type is intended for batch servers, where processes may be executing on behalf of a user
        /// without their direct intervention. This type is also for higher performance servers that process many
        /// plaintext authentication attempts at a time, such as mail or web servers.
        /// </summary>
        Batch = 4,

        /// <summary>
        /// Indicates a service-type logon. The account provided must have the service privilege enabled. 
        /// </summary>
        Service = 5,

        /// <summary>
        /// GINAs are no longer supported.
        /// Windows Server 2003 and Windows XP:  This logon type is for GINA DLLs that log on users who will be
        /// interactively using the computer. This logon type can generate a unique audit record that shows when
        /// the workstation was unlocked.
        /// </summary>
        Unlock = 7,

        /// <summary>
        /// This logon type preserves the name and password in the authentication package, which allows the server
        /// to make connections to other network servers while impersonating the client. A server can accept plaintext
        /// credentials from a client, call LogonUser, verify that the user can access the system across the network,
        /// and still communicate with other servers.
        /// </summary>
        NetworkCleartext = 8,

        /// <summary>
        /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
        /// The new logon session has the same local identifier but uses different credentials for other network connections.
        /// This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
        /// </summary>
        NewCredentials = 9,
    }
}
