using System;
using System.Security;
using Xunit;

namespace SimpleImpersonation.UnitTests
{
    public class UserCredentialsTests
    {
        [Fact]
        public void UserCredentials_Constructor_Valid_PlainPassword_1()
        {
            var creds = new UserCredentials("user", "password");
            Assert.Equal("user", creds.ToString());
        }

        [Fact]
        public void UserCredentials_Constructor_Valid_SecurePassword_1()
        {
            using (var password = CreateSecureStringPasswordForTesting())
            {
                var creds = new UserCredentials("user", password);
                Assert.Equal("user", creds.ToString());
            }
        }

        [Fact]
        public void UserCredentials_Constructor_Valid_PlainPassword_2()
        {
            var creds = new UserCredentials("user@domain", "password");
            Assert.Equal("user@domain", creds.ToString());
        }

        [Fact]
        public void UserCredentials_Constructor_Valid_SecurePassword_2()
        {
            using (var password = CreateSecureStringPasswordForTesting())
            {
                var creds = new UserCredentials("user@domain", password);
                Assert.Equal("user@domain", creds.ToString());
            }
        }

        [Fact]
        public void UserCredentials_Constructor_Valid_PlainPassword_3()
        {
            var creds = new UserCredentials(@"domain\user", "password");
            Assert.Equal("user@domain", creds.ToString());
        }

        [Fact]
        public void UserCredentials_Constructor_Valid_SecurePassword_3()
        {
            using (var password = CreateSecureStringPasswordForTesting())
            {
                var creds = new UserCredentials(@"domain\user", password);
                Assert.Equal("user@domain", creds.ToString());
            }
        }

        [Fact]
        public void UserCredentials_Constructor_Valid_PlainPassword_4()
        {
            var creds = new UserCredentials("domain", "user", "password");
            Assert.Equal("user@domain", creds.ToString());
        }

        [Fact]
        public void UserCredentials_Constructor_Valid_SecurePassword_4()
        {
            using (var password = CreateSecureStringPasswordForTesting())
            {
                var creds = new UserCredentials("domain", "user", password);
                Assert.Equal("user@domain", creds.ToString());
            }
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_DomainInBoth_1()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var _ = new UserCredentials("domain", "user@domain", "password");
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_DomainInBoth_2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var _ = new UserCredentials("domain", @"domain\user", "password");
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_DomainInBoth_3()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (var password = new SecureString())
                {
                    var _ = new UserCredentials("domain", "user@domain", password);
                }
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_DomainInBoth_4()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (var password = new SecureString())
                {
                    var _ = new UserCredentials("domain", @"domain\user", password);
                }
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_EmptyDomain()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var _ = new UserCredentials("", "user", "password");
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_EmptyUsername_1()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var _ = new UserCredentials("domain", "", "password");
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_EmptyUsername_2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var _ = new UserCredentials("", "password");
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_EmptyPassword_1()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var _ = new UserCredentials("domain", "user", "");
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_EmptyPassword_2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var _ = new UserCredentials("user", "");
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_EmptySecurePassword_1()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (var password = new SecureString())
                {
                    var _ = new UserCredentials("domain", "user", password);
                }
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_EmptySecurePassword_2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (var password = new SecureString())
                {
                    var _ = new UserCredentials("user", password);
                }
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_NullDomain()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new UserCredentials(null, "user", "password");
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_NullUsername_1()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new UserCredentials("domain", null, "password");
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_NullUsername_2()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new UserCredentials(null, "password");
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_NullPassword_1()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new UserCredentials("domain", "user", (string)null);
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_NullPassword_2()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new UserCredentials("user", (string)null);
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_NullSecurePassword_1()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new UserCredentials("domain", "user", (SecureString)null);
            });
        }

        [Fact]
        public void UserCredentials_Constructor_Invalid_NullSecurePassword_2()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new UserCredentials("user", (SecureString)null);
            });
        }


        private static SecureString CreateSecureStringPasswordForTesting()
        {
            // Note: This is obviously not really a secure password.  We just need something to test the API with.

            var password = new SecureString();

            foreach (var c in "password")
                password.AppendChar(c);

            return password;
        }
    }
}
