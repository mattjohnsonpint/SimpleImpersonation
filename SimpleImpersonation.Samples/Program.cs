using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleImpersonation.Samples
{
    public static class Program
    {
        private const string Domain = ".";
        private const string User = "administrator";
        private const string Password = "123";

        private const LogonType Logon = LogonType.NewCredentials;


        public static void Main()
        {
            const int maxDegreeOfParallelism = 4;
            const string path = @"\\192.168.30.150\TestFolder";

            var credentials = new UserCredentials(Domain, User, Password);
            
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };
            using var token = Impersonation.LowLevel.Impersonate(credentials, Logon);
            var files = Impersonation.LowLevel.RunImpersonated(token, _ => Directory.GetFiles(path));
            Parallel.ForEach(files, parallelOptions, file =>
            {
                Impersonation.LowLevel.RunImpersonated(token, _ =>
                {
                    var info = new FileInfo(file);
                    Console.WriteLine(info.Exists);
                });
            });
        }
    }
}