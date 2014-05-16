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
    public class CommitTaxTest
    {
        public static void Test()
        {
            const string ACCOUNTNUMBER = "1234567890";
            const string LICENSEKEY = "A1B2C3D4E5F6G7H8";
            const string SERVICEURL = "https://development.avalara.net";
            const string ENDPOINT = "/tax/taxsvc.asmx";

            Console.WriteLine("Begin CommitTaxTest...");

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

                CommitTaxRequest commitTaxRequest = new CommitTaxRequest();

                // Required Request Parameters
                commitTaxRequest.CompanyCode = "APITrialCompany";
                commitTaxRequest.DocType = DocumentType.SalesInvoice;
                commitTaxRequest.DocCode = "INV001";
                
                CommitTaxResult commitTaxResult = taxSvc.CommitTax(commitTaxRequest);

                Console.WriteLine("Result: {0}", commitTaxResult.ResultCode.ToString());

                if (commitTaxResult.ResultCode != SeverityLevel.Success)
                {
                    foreach (Message message in commitTaxResult.Messages)
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
                    Console.WriteLine("Document Committed.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}\n{1}", e.Message, e.StackTrace);
            }
            Console.WriteLine("End CommitTaxTest. Press enter to continue.");
            Console.ReadLine();
        }
    }
}
