using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDog.WpfApp
{
    public static class ServiceBusUtilities
    {
        public static InitializationRequest GetServiceBusCredentials()
        {
            return new InitializationRequest
            {
                IssuerKey = "XGOuQ6M2hgmkzhDKlJR6AVhbtIEPZF6gRJb1fbMO9Do=",
                Issuer = "owner",
                Namespace = "testingNotHub-ns"
            };
        }
    }
}
