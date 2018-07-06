using System;
using System.DirectoryServices.AccountManagement;
using System.Security;

namespace SimpleImpersonation.UnitTests
{
    public class UserPrincipalFixture : IDisposable
    {
        private bool _disposed;
        private readonly PrincipalContext _context;
        private readonly UserPrincipal _principal;

        public string Username { get; }
        public string Password { get; }
        public SecureString PasswordAsSecureString { get; }

        public string FullUsername => Environment.MachineName + "\\" + Username;

        public UserPrincipalFixture()
        {
            Username = "Test" + DateTime.Now.ToString("yyyyMMdd'T'HHmmss");
            Password = Guid.NewGuid().ToString();

            var ss = new SecureString();
            foreach (var c in Password)
                ss.AppendChar(c);
            PasswordAsSecureString = ss;

            _context = new PrincipalContext(ContextType.Machine);
            _principal = new UserPrincipal(_context, Username, Password, true);
            _principal.Save();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _principal.Delete();
            _principal.Dispose();
            _context.Dispose();
            PasswordAsSecureString.Dispose();
            _disposed = true;
        }
    }
}
