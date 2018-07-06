using System.Security.Principal;
using Xunit;

namespace SimpleImpersonation.UnitTests
{
    // TODO: user@domain form, test all functions, warn if not elevated

    public class ImpersonationTests : IClassFixture<UserPrincipalFixture>
    {
        private readonly UserPrincipalFixture _fixture;

        public ImpersonationTests(UserPrincipalFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Impersonate_RunAsUser_PlainPassword_Function()
        {
            var userNameBefore = WindowsIdentity.GetCurrent().Name;

            var credentials = new UserCredentials(_fixture.Username, _fixture.Password);

            var userNameDuring = Impersonation.RunAsUser(credentials, LogonType.Interactive,
                () => WindowsIdentity.GetCurrent().Name);

            var userNameAfter = WindowsIdentity.GetCurrent().Name;

            Assert.Equal(userNameBefore, userNameAfter);
            Assert.Equal(_fixture.FullUsername, userNameDuring);
        }

        [Fact]
        public void Impersonate_RunAsUser_PlainPassword_FunctionWithTokenHandle()
        {
            var userNameBefore = WindowsIdentity.GetCurrent().Name;

            var credentials = new UserCredentials(_fixture.Username, _fixture.Password);

            var (userNameDuring, tokenIsValid) = Impersonation.RunAsUser(credentials, LogonType.Interactive,
                tokenHandle => (WindowsIdentity.GetCurrent().Name, !tokenHandle.IsInvalid));

            var userNameAfter = WindowsIdentity.GetCurrent().Name;

            Assert.Equal(userNameBefore, userNameAfter);
            Assert.Equal(_fixture.FullUsername, userNameDuring);
            Assert.True(tokenIsValid);
        }

        [Fact]
        public void Impersonate_RunAsUser_PlainPassword_Action()
        {
            var userNameBefore = WindowsIdentity.GetCurrent().Name;

            var credentials = new UserCredentials(_fixture.Username, _fixture.Password);

            string userNameDuring = null;
            Impersonation.RunAsUser(credentials, LogonType.Interactive,
                () => { userNameDuring = WindowsIdentity.GetCurrent().Name; });

            var userNameAfter = WindowsIdentity.GetCurrent().Name;

            Assert.Equal(userNameBefore, userNameAfter);
            Assert.Equal(_fixture.FullUsername, userNameDuring);
        }

        [Fact]
        public void Impersonate_RunAsUser_PlainPassword_ActionWithTokenHandle()
        {
            var userNameBefore = WindowsIdentity.GetCurrent().Name;

            var credentials = new UserCredentials(_fixture.Username, _fixture.Password);

            bool tokenIsValid = false;
            string userNameDuring = null;
            Impersonation.RunAsUser(credentials, LogonType.Interactive,
                tokenHandle =>
                {
                    userNameDuring = WindowsIdentity.GetCurrent().Name;
                    tokenIsValid = !tokenHandle.IsInvalid;
                });

            var userNameAfter = WindowsIdentity.GetCurrent().Name;

            Assert.Equal(userNameBefore, userNameAfter);
            Assert.Equal(_fixture.FullUsername, userNameDuring);
            Assert.True(tokenIsValid);
        }

        [Fact]
        public void Impersonate_RunAsUser_SecurePassword_Function()
        {
            var userNameBefore = WindowsIdentity.GetCurrent().Name;

            var credentials = new UserCredentials(_fixture.Username, _fixture.PasswordAsSecureString);

            var userNameDuring = Impersonation.RunAsUser(credentials, LogonType.Interactive,
                () => WindowsIdentity.GetCurrent().Name);

            var userNameAfter = WindowsIdentity.GetCurrent().Name;

            Assert.Equal(userNameBefore, userNameAfter);
            Assert.Equal(_fixture.FullUsername, userNameDuring);
        }

        [Fact]
        public void Impersonate_RunAsUser_SecurePassword_FunctionWithTokenHandle()
        {
            var userNameBefore = WindowsIdentity.GetCurrent().Name;

            var credentials = new UserCredentials(_fixture.Username, _fixture.PasswordAsSecureString);

            var (userNameDuring, tokenIsValid) = Impersonation.RunAsUser(credentials, LogonType.Interactive,
                tokenHandle => (WindowsIdentity.GetCurrent().Name, !tokenHandle.IsInvalid));

            var userNameAfter = WindowsIdentity.GetCurrent().Name;

            Assert.Equal(userNameBefore, userNameAfter);
            Assert.Equal(_fixture.FullUsername, userNameDuring);
            Assert.True(tokenIsValid);
        }

        [Fact]
        public void Impersonate_RunAsUser_SecurePassword_Action()
        {
            var userNameBefore = WindowsIdentity.GetCurrent().Name;

            var credentials = new UserCredentials(_fixture.Username, _fixture.PasswordAsSecureString);

            string userNameDuring = null;
            Impersonation.RunAsUser(credentials, LogonType.Interactive,
                () => { userNameDuring = WindowsIdentity.GetCurrent().Name; });

            var userNameAfter = WindowsIdentity.GetCurrent().Name;

            Assert.Equal(userNameBefore, userNameAfter);
            Assert.Equal(_fixture.FullUsername, userNameDuring);
        }

        [Fact]
        public void Impersonate_RunAsUser_SecurePassword_ActionWithTokenHandle()
        {
            var userNameBefore = WindowsIdentity.GetCurrent().Name;

            var credentials = new UserCredentials(_fixture.Username, _fixture.PasswordAsSecureString);

            bool tokenIsValid = false;
            string userNameDuring = null;
            Impersonation.RunAsUser(credentials, LogonType.Interactive,
                tokenHandle =>
                {
                    userNameDuring = WindowsIdentity.GetCurrent().Name;
                    tokenIsValid = !tokenHandle.IsInvalid;
                });

            var userNameAfter = WindowsIdentity.GetCurrent().Name;

            Assert.Equal(userNameBefore, userNameAfter);
            Assert.Equal(_fixture.FullUsername, userNameDuring);
            Assert.True(tokenIsValid);
        }
    }
}
