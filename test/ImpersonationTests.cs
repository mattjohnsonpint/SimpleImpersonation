using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace SimpleImpersonation.UnitTests
{
    // NOTE: Unit tests for this project must be executed as an administrator,
    //       because they create a temporary user for testing successful impersonation.

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

        [Fact]
        public async Task Impersonate_RunAsUser_Async()
        {
            var userNameBefore = WindowsIdentity.GetCurrent().Name;

            var credentials = new UserCredentials(_fixture.Username, _fixture.Password);

            var rnd = new Random();

            async Task<(string, string, string)> TaskFactory()
            {
                var name1 = WindowsIdentity.GetCurrent().Name;

                await Task.Delay(rnd.Next(5, 100));

                var name2 = await Impersonation.RunAsUser(credentials, LogonType.Interactive,
                    async () =>
                    {
                        await Task.Delay(rnd.Next(5, 100));
                        var name = WindowsIdentity.GetCurrent().Name;
                        await Task.Delay(rnd.Next(5, 100));
                        return name;
                    });

                await Task.Delay(rnd.Next(5, 100));

                var name3 = WindowsIdentity.GetCurrent().Name;

                return (name1, name2, name3);
            }

            await Task.Delay(500);

            var tasks = Enumerable.Range(1, 50).Select(x => Task.Run(TaskFactory));
            var userNamesDuring = await Task.WhenAll(tasks);

            Assert.All(userNamesDuring, actual =>
            {
                var expected = (userNameBefore, _fixture.FullUsername, userNameBefore);
                Assert.Equal(expected, actual);
            });
        }
    }
}
