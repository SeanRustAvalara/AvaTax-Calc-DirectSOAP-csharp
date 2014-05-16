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
    public class PostTaxTest
    {
        public static void Test()
        {
            const string ACCOUNTNUMBER = "1234567890";
            const string LICENSEKEY = "A1B2C3D4E5F6G7H8";
            const string SERVICEURL = "https://development.avalara.net";
            const string ENDPOINT = "/tax/taxsvc.asmx";

            Console.WriteLine("Begin PostTaxTest...");
            
            try
            {
                TaxSvc taxSvc = new TaxSvc();
                taxSvc.Url = SERVICEURL + ENDPOINT;

                UsernameToken token = new UsernameToken(
                    ACCOUNTNUMBER, LICENSEKEY, PasswordOption.SendPlainText);
                SoapContext requestContext = taxSvc.RequestSoapContext;
                requestContext.Security.Tokens.Add(token);
                requestContext.Security.Timestamp.TtlInSeconds = 300;

                Profile profile = new Profile();
                profile.Client = "AvaTaxSample";
                taxSvc.ProfileValue = profile;

                PostTaxRequest postTaxRequest = new PostTaxRequest();

                // Required Request Parameters
                postTaxRequest.CompanyCode = "APITrialCompany";
                postTaxRequest.DocType = DocumentType.SalesInvoice;
                postTaxRequest.DocCode = "INV001";
                postTaxRequest.DocDate = DateTime.Parse("2014-01-01");
                postTaxRequest.TotalTax = Convert.ToDecimal(14.27);
                postTaxRequest.TotalAmount = 175;

                // Optional Request Parameters
                // postTaxRequest.NewDocCode = "INV001-1";
                // postTaxRequest.Commit = true;

                PostTaxResult postTaxResult = taxSvc.PostTax(postTaxRequest);

                Console.WriteLine("Result: {0}", postTaxResult.ResultCode.ToString());

                if (postTaxResult.ResultCode != SeverityLevel.Success)
                {
                    foreach (Message message in postTaxResult.Messages)
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
                    Console.WriteLine("Document Posted.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}\n{1}", e.Message, e.StackTrace);
            }
            Console.WriteLine("End PostTaxTest. Press enter to continue.");
            Console.ReadLine();
        }
    }
}
