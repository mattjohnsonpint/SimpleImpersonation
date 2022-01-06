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
            using var userHandle = credentials.LogonUser(LogonType.Interactive);
            var userNameDuring = WindowsIdentity.RunImpersonated(userHandle, () => WindowsIdentity.GetCurrent().Name);
            var userNameAfter = WindowsIdentity.GetCurrent().Name;

            Assert.Equal(userNameBefore, userNameAfter);
            Assert.Equal(_fixture.FullUsername, userNameDuring);
        }

        [Fact]
        public void Impersonate_RunAsUser_PlainPassword_Action()
        {
            var userNameBefore = WindowsIdentity.GetCurrent().Name;

            var credentials = new UserCredentials(_fixture.Username, _fixture.Password);
            using var userHandle = credentials.LogonUser(LogonType.Interactive);

            string userNameDuring = null;
            WindowsIdentity.RunImpersonated(userHandle,
                () => { userNameDuring = WindowsIdentity.GetCurrent().Name; });

            var userNameAfter = WindowsIdentity.GetCurrent().Name;

            Assert.Equal(userNameBefore, userNameAfter);
            Assert.Equal(_fixture.FullUsername, userNameDuring);
        }

        [Fact]
        public void Impersonate_RunAsUser_SecurePassword_Function()
        {
            var userNameBefore = WindowsIdentity.GetCurrent().Name;

            var credentials = new UserCredentials(_fixture.Username, _fixture.PasswordAsSecureString);
            using var userHandle = credentials.LogonUser(LogonType.Interactive);

            var userNameDuring = WindowsIdentity.RunImpersonated(userHandle,
                () => WindowsIdentity.GetCurrent().Name);

            var userNameAfter = WindowsIdentity.GetCurrent().Name;

            Assert.Equal(userNameBefore, userNameAfter);
            Assert.Equal(_fixture.FullUsername, userNameDuring);
        }

        [Fact]
        public void Impersonate_RunAsUser_SecurePassword_Action()
        {
            var userNameBefore = WindowsIdentity.GetCurrent().Name;

            var credentials = new UserCredentials(_fixture.Username, _fixture.PasswordAsSecureString);
            using var userHandle = credentials.LogonUser(LogonType.Interactive);

            string userNameDuring = null;
            WindowsIdentity.RunImpersonated(userHandle,
                () => { userNameDuring = WindowsIdentity.GetCurrent().Name; });

            var userNameAfter = WindowsIdentity.GetCurrent().Name;

            Assert.Equal(userNameBefore, userNameAfter);
            Assert.Equal(_fixture.FullUsername, userNameDuring);
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

                using var userHandle = credentials.LogonUser(LogonType.Interactive);
#if NETFRAMEWORK
                var name2 = await WindowsIdentity.RunImpersonated(userHandle,
#else
                var name2 = await WindowsIdentity.RunImpersonatedAsync(userHandle,
#endif
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
