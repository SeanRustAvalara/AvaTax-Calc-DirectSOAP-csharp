﻿using System;
using System.Configuration;
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

                Console.WriteLine("PostTaxTest Result: {0}", postTaxResult.ResultCode.ToString());

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
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }
    }
}
