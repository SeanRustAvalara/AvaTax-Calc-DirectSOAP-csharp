using System;
using System.Configuration;
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
            string accountNumber = ConfigurationManager.AppSettings["AvaTax:AccountNumber"];
            string licenseKey = ConfigurationManager.AppSettings["AvaTax:LicenseKey"];
            string serviceUrl = ConfigurationManager.AppSettings["AvaTax:ServiceUrl"];
            string endpoint = "/tax/taxsvc.asmx";

            try
            {
                TaxSvc taxSvc = new TaxSvc();

                taxSvc.Url = serviceUrl + endpoint;

                UsernameToken token = new UsernameToken(
                    accountNumber, licenseKey, PasswordOption.SendPlainText);
                SoapContext requestContext = taxSvc.RequestSoapContext;
                requestContext.Security.Tokens.Add(token);
                requestContext.Security.Timestamp.TtlInSeconds = 300;

                Profile profile = new Profile();
                profile.Client = "AvaTaxSample";
                taxSvc.ProfileValue = profile;
                
                PingResult pingResult = taxSvc.Ping("");
                      
                Console.WriteLine("PingTest Result: {0}", pingResult.ResultCode.ToString());

                if (pingResult.ResultCode != SeverityLevel.Success)
                {
                    foreach (Message message in pingResult.Messages)
                    {
                        Console.WriteLine("{0}\n{1}\n{2}",
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
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }
    }
}
