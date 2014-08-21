using System;
using System.Configuration;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Security.Tokens;
using AvaTax_Calc_DirectSOAP_csharp.TaxService;

namespace AvaTax_Calc_DirectSOAP_csharp
{
    class AdjustTaxTest
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

                GetTaxRequest getTaxRequest = new GetTaxRequest();

                // Document Level Parameters

                // Required Request Parameters
                getTaxRequest.CustomerCode = "ABC4335";
                getTaxRequest.DocDate = DateTime.Parse("2014-01-01");
                // getTaxRequest.Lines is also required, 
                // and is presented later in this file.

                // Best Practice Request Parameters
                getTaxRequest.CompanyCode = "APITrialCompany";
                getTaxRequest.DocCode = "INV001";
                getTaxRequest.DetailLevel = DetailLevel.Tax;
                getTaxRequest.DocType = DocumentType.SalesInvoice;
                getTaxRequest.Commit = true;

                // Situational Request Parameters	
                // getTaxRequest.BusinessIdentificationNo = "234243";
                // getTaxRequest.CustomerUsageType = "G";
                // getTaxRequest.ExemptionNo = "12345";
                // getTaxRequest.Discount = 50;
                // getTaxRequest.LocationCode = "01";
                // getTaxRequest.ServiceMode = ServiceMode.Automatic;	
                // getTaxRequest.TaxOverride.TaxOverrideType = TaxOverrideType.TaxDate;
                // getTaxRequest.TaxOverride.Reason = "Adjustment for return";
                // getTaxRequest.TaxOverride.TaxDate = DateTime.Parse("2013-07-01");    
                

                // Optional Request Parameters
                getTaxRequest.PurchaseOrderNo = "PO123456";
                getTaxRequest.ReferenceCode = "ref123456";
                getTaxRequest.PosLaneCode = "09";
                getTaxRequest.CurrencyCode = "USD";
                getTaxRequest.ExchangeRate = Convert.ToDecimal(1.0);
                getTaxRequest.ExchangeRateEffDate = DateTime.Parse("2013-01-01");
                getTaxRequest.SalespersonCode = "Bill Sales";

                BaseAddress[] addresses = new BaseAddress[3];
                BaseAddress address1 = new BaseAddress();
                address1.AddressCode = "01";
                address1.Line1 = "45 Fremont Street";
                address1.City = "San Francisco";
                address1.Region = "CA";
                addresses[0] = address1;

                BaseAddress address2 = new BaseAddress();
                address2.AddressCode = "02";
                address2.Line1 = "118 N Clark St";
                address2.Line2 = "Suite 100";
                address2.Line3 = "ATTN Accounts Payable";
                address2.City = "Chicago";
                address2.Region = "IL";
                address2.PostalCode = "60602";
                address2.Country = "US";
                addresses[1] = address2;

                BaseAddress address3 = new BaseAddress();
                address3.AddressCode = "03";
                address3.Latitude = "47.627935";
                address3.Longitude = "-122.51702";
                addresses[2] = address3;

                getTaxRequest.Addresses = addresses;

                getTaxRequest.OriginCode = "01";
                getTaxRequest.DestinationCode = "03";

                // Line Data

                // Required Parameters
                Line[] lines = new Line[3];
                Line line1 = new Line();
                line1.No = "0001";
                line1.ItemCode = "N543";
                line1.Qty = Convert.ToDecimal(1);
                line1.Amount = Convert.ToDecimal(10);
                line1.OriginCode = "01";
                line1.DestinationCode = "02";

                // Best Practice Request Parameters
                line1.Description = "Red Size 7 Widget";
                line1.TaxCode = "NT";

                // Situational Request Parameters
                // line1.CustomerUsageType = "L";
                // line1.ExemptionNo = "12345";
                // line1.Discounted = true;
                // line1.TaxIncluded = true;
                // line1.TaxOverride.TaxOverrideType = TaxOverrideType.TaxDate;
                // line1.TaxOverride.Reason = "Adjustment for return";
                // line1.TaxOverride.TaxDate = DateTime.Parse("2013-07-01");

                // Optional Request Parameters
                line1.Ref1 = "ref123";
                line1.Ref2 = "ref456";
                lines[0] = line1;

                Line line2 = new Line();
                line2.No = "0002";
                line2.ItemCode = "T345";
                line2.Qty = Convert.ToDecimal(4);
                line2.Amount = Convert.ToDecimal(200);
                line2.OriginCode = "01";
                line2.DestinationCode = "03";
                line2.Description = "Size 10 Green Running Shoe";
                line2.TaxCode = "PC030147";
                lines[1] = line2;

                Line line3 = new Line();
                line3.No = "0002-FR";
                line3.ItemCode = "FREIGHT";
                line3.Qty = Convert.ToDecimal(1);
                line3.Amount = Convert.ToDecimal(15);
                line3.OriginCode = "01";
                line3.DestinationCode = "03";
                line3.Description = "Shipping Charge";
                line3.TaxCode = "FR";
                lines[2] = line3;
                getTaxRequest.Lines = lines;

                AdjustTaxRequest adjustTaxRequest = new AdjustTaxRequest();
		        adjustTaxRequest.GetTaxRequest = getTaxRequest;
		        adjustTaxRequest.AdjustmentReason = 4; // quantity change
		        //adjustTaxRequest.AdjustmentDescription = "Transaction Adjusted for Testing";

                AdjustTaxResult adjustTaxResult = taxSvc.AdjustTax(adjustTaxRequest);

                Console.WriteLine("AdjustTaxTest Result: {0}", adjustTaxResult.ResultCode.ToString());

                if (adjustTaxResult.ResultCode != SeverityLevel.Success)
                {
                    foreach (Message message in adjustTaxResult.Messages)
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
                    Console.WriteLine("Document Code: {0}\nTotal Tax: {1}\nDocument Version: {2}",
                        getTaxRequest.DocCode,
                        adjustTaxResult.TotalTax.ToString(),
                        adjustTaxResult.Version.ToString());

                    foreach (TaxLine taxLine in adjustTaxResult.TaxLines)
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
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }
    }
}
