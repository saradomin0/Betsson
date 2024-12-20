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
    }
}
