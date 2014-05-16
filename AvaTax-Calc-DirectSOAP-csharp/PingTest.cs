using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Security.Tokens;
using AvaTax_Calc_DirectSOAP_csharp.TaxService;

namespace AvaTax_Calc_DirectSOAP_csharp
{
    public class PingTest
    {
        public static void Test()
        {
            const string ACCOUNTNUMBER = "1234567890";
            const string LICENSEKEY = "A1B2C3D4E5F6G7H8";
            const string SERVICEURL = "https://development.avalara.net";
            const string ENDPOINT = "/tax/taxsvc.asmx";

            Console.WriteLine("Begin PingTest...");

            try
            {
                TaxSvc taxSvc = new TaxSvc();
                taxSvc.Url = SERVICEURL + ENDPOINT;

                UsernameToken token = new UsernameToken(ACCOUNTNUMBER, LICENSEKEY, PasswordOption.SendPlainText);
                SoapContext requestContext = taxSvc.RequestSoapContext;
                requestContext.Security.Tokens.Add(token);
                requestContext.Security.Timestamp.TtlInSeconds = 300;

                Profile profile = new Profile();
                profile.Client = "AvaTaxSample";
                taxSvc.ProfileValue = profile;
                
                PingResult pingResult = taxSvc.Ping("");
                      
                Console.WriteLine("Result: {0}", pingResult.ResultCode.ToString());

                if (pingResult.ResultCode != SeverityLevel.Success)
                {
                    foreach (Message message in pingResult.Messages)
                    {
                        Console.WriteLine("{0}{1}{2}",
                            message.Name,
                            message.Summary,
                            message.Details);
                    }
                }
                else
                {
                    Console.WriteLine("Service Version: {0}", pingResult.Version);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}", e.Message);
            }
            Console.WriteLine("End PingTest. Press enter to continue.");
            Console.ReadLine();
        }
    }
}
