using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Security.Tokens;
using AvaTax_Calc_DirectSOAP_csharp.AddressService;

namespace AvaTax_Calc_DirectSOAP_csharp
{
    public class ValidateAddressTest
    {
        public static void Test()
        {
            string accountNumber = "1234567890";
            string licenseKey = "A1B2C3D4E5F6G7H8";
            string serviceUrl = "https://development.avalara.net";
            string endpoint = "/address/addresssvc.asmx";

            try
            {
                AddressSvc addressSvc = new AddressSvc();
                addressSvc.Url = serviceUrl + endpoint;

                UsernameToken token = new UsernameToken(
                    accountNumber, licenseKey, PasswordOption.SendPlainText);
                SoapContext requestContext = addressSvc.RequestSoapContext;
                requestContext.Security.Tokens.Add(token);
                requestContext.Security.Timestamp.TtlInSeconds = 300;

                Profile profile = new Profile();
                profile.Client = "AvaTaxSample";
                addressSvc.ProfileValue = profile;

                BaseAddress address = new BaseAddress();

                // Required Address Parameters
                address.Line1 = "118 N Clark St";
                address.City = "Chicago";
                address.Region = "IL";

                // Optional Address Parameters
                address.Line2 = "Suite 100";
                address.Line3 = "ATTN Accounts Payable";
                address.Country = "US";
                address.PostalCode = "60602";

                ValidateRequest validateRequest = new ValidateRequest();                

                // Required Request Parameters
                validateRequest.Address = address;
                
                // Optional Request Parameters
                validateRequest.Coordinates = true;
                validateRequest.Taxability = true;
                validateRequest.TextCase = TextCase.Upper;

                ValidateResult validateResult = addressSvc.Validate(validateRequest);

                Console.WriteLine("ValidateAddressTest Result: {0}", validateResult.ResultCode.ToString());
                Console.WriteLine();

                if (validateResult.ResultCode != SeverityLevel.Success)
                {
                    foreach (Message message in validateResult.Messages)
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
                    ValidAddress validatedAddress = validateResult.ValidAddresses[0];
                    Console.WriteLine("Validated Address: ");
			        Console.WriteLine("{0}\n{1}\n{2}\n{3}, {4} {5}\nCountry: {6}", 
				        validatedAddress.Line1,
				        validatedAddress.Line2,
				        validatedAddress.Line3,
				        validatedAddress.City,
				        validatedAddress.Region,
				        validatedAddress.PostalCode,
				        validatedAddress.Country);
			        Console.WriteLine("Latitude: {0}\nLongitude: {1}", 
				        validatedAddress.Latitude,
				        validatedAddress.Longitude);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }
    }
}
