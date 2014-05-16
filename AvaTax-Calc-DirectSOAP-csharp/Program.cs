using System;

namespace AvaTax_Calc_DirectSOAP_csharp
{
    class Program
    {
        public static void Main(string[] args)
        {
            PingTest.Test();
            GetTaxTest.Test();
            PostTaxTest.Test();
            CommitTaxTest.Test();
            GetTaxHistoryTest.Test();
            AdjustTaxTest.Test();
            CancelTaxTest.Test();
            ValidateAddressTest.Test();
        }
    }
}
