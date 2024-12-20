using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTestBetssonConstans
{
    public class BetssonConstans()
    {
        /// <summary>
        /// URL for RestClient
        /// </summary>
        public static string URL => "http://localhost:8080";
        /// <summary>
        /// RestRequest for Balance
        /// </summary>
        public static string Balance => "/onlinewallet/balance";
        /// <summary>
        /// RestRequest for Deposit
        /// </summary>
        public static string Deposit => "/onlinewallet/deposit";
        /// <summary>
        /// RestRequest for Withdraw
        /// </summary>
        public static string Withdraw => "/onlinewallet/withdraw";
        /// <summary>
        /// Null value for amount
        /// </summary>
        public static string Value => null;
        /// <summary>
        /// EmptyJsonBody
        /// </summary>
        public static object EmptyJsonBody => new { };
        /// <summary>
        /// EmptyJson
        /// </summary>
        public static object EmptyBody => "";
 
        // Error message

        /// <summary>
        /// EmptyJson
        /// </summary>
        public static string EmptyBodyErrorMessage => "A non-empty request body is required.";
        /// <summary>
        /// EmptyJson
        /// </summary>
        public static string InvalidWithdrawErrorMessage => "Invalid withdrawal amount. There are insufficient funds.";
        /// <summary>
        /// EmptyJson
        /// </summary>
        public static string InvalidStringErrorMessage => "The JSON value could not be converted to System.Decimal.";



    }
}
