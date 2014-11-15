using System;
using System.Security;
using System.Security.Permissions;

namespace UnitTests
{
    internal static class PartialTrustTestHelper
    {
        public static void Run<T>() where T : PartialTrustTest
        {
            AppDomain domain = CreateParitalTrustDomain();
            try
            {
                object instance = domain.CreateInstanceAndUnwrap(
                    typeof(T).Assembly.FullName,
                    typeof(T).FullName);

                ((PartialTrustTest)instance).Run();
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

        private static AppDomain CreateParitalTrustDomain()
        {
            PermissionSet permissions = new PermissionSet(null);
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = ".";

            return AppDomain.CreateDomain("Sandbox", null, setup, permissions);
        }
    }
}
