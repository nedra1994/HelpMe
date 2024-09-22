using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.eShopOnContainers.Services.Identity.API.Certificates
{
    static class Certificate
    {
        public static X509Certificate2 Get(string pathcert)
        {
            var assembly = typeof(Certificate).GetTypeInfo().Assembly;
            var names = assembly.GetManifestResourceNames();



            var stream = File.ReadAllBytes(pathcert);// @"E:\01-Work\Projects\HelpMe.Reseller\HelpMe.Commun\HelpMe.Commun.Security.Identity\Certificate\idsrv3test.pfx");
            {
                return new X509Certificate2(stream, "idsrv3test");
            }
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}