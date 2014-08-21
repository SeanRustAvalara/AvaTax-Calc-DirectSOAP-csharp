using System;
using System.Configuration;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Security.Tokens;
using AvaTax_Calc_DirectSOAP_csharp.TaxService;

namespace AvaTax_Calc_DirectSOAP_csharp
{
    public class GetTaxHistoryTest
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

                GetTaxHistoryRequest getTaxHistoryRequest = new GetTaxHistoryRequest();

                // Required Request Parameters
                getTaxHistoryRequest.CompanyCode = "APITrialCompany";
                getTaxHistoryRequest.DocCode = "INV001";
                getTaxHistoryRequest.DetailLevel = DetailLevel.Tax;
                getTaxHistoryRequest.DocType = DocumentType.SalesInvoice;

                // Optional Request Parameters
                // getTaxHistoryRequest->DocId = "123412341234";
                
                GetTaxHistoryResult getTaxHistoryResult = taxSvc.GetTaxHistory(getTaxHistoryRequest);

                Console.WriteLine("GetTaxHistoryTest Result: {0}", getTaxHistoryResult.ResultCode.ToString());

                if (getTaxHistoryResult.ResultCode != SeverityLevel.Success)
                {
                    foreach (Message message in getTaxHistoryResult.Messages)
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
                    Console.WriteLine("Document Code: {0}\nTotal Tax: {1}",
                        getTaxHistoryRequest.DocCode,
                        getTaxHistoryResult.GetTaxResult.TotalTax.ToString());

                    foreach (TaxLine taxLine in getTaxHistoryResult.GetTaxResult.TaxLines)
                    {
                        Console.WriteLine("    Line {0} Tax: {1}",
                            taxLine.No,
                            taxLine.Tax.ToString());

                        foreach (TaxDetail taxDetail in taxLine.TaxDetails)
                        {
                            Console.WriteLine("        {0} Tax: {1}",
                                taxDetail.JurisName,
                                taxDetail.Tax.ToString());
                        }
                    }
                    //to display diagnostic details in the console
                    if (getTaxHistoryRequest.DetailLevel == DetailLevel.Diagnostic)
                    {
                        foreach (Message message in getTaxHistoryResult.GetTaxResult.Messages)
                        {
                            Console.WriteLine("{0}\n{1}\n{2}",
                                message.Name,
                                message.Summary,
                                message.Details);
                            Console.ReadLine();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }
    }
}
