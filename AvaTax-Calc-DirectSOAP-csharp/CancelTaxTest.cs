using System;
using System.Configuration;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Security.Tokens;
using AvaTax_Calc_DirectSOAP_csharp.TaxService;

namespace AvaTax_Calc_DirectSOAP_csharp
{
    public class CancelTaxTest
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

                CancelTaxRequest cancelTaxRequest = new CancelTaxRequest();

                // Required Request Parameters
                cancelTaxRequest.CompanyCode = "APITrialCompany";
                cancelTaxRequest.DocType = DocumentType.SalesInvoice;
                cancelTaxRequest.DocCode = "INV001";
                cancelTaxRequest.CancelCode = CancelCode.DocVoided;

                // Optional Request Parameters
                // cancelTaxRequest.DocId = "123412341234";

                CancelTaxResult cancelTaxResult = taxSvc.CancelTax(cancelTaxRequest);

                Console.WriteLine("CancelTaxTest Result: {0}", cancelTaxResult.ResultCode.ToString());

                if (cancelTaxResult.ResultCode != SeverityLevel.Success)
                {
                    foreach (Message message in cancelTaxResult.Messages)
                    {
                        Console.WriteLine("{0}\n{1}\n{2}\n{3}",
                            message.Name,
                            message.Summary,
                            message.RefersTo,
                            message.Details);
                    }
                }
                else
                {
                    Console.WriteLine("Document Voided.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }
    }
}
