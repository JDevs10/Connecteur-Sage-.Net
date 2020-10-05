using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Update
{
    public class DataObject
    {
        public string Name { get; set; }
    }

    class Program
    {
        private static string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private const string URL = "http://82.253.71.109/prod/bdc_v11_04/api/index.php/orders/668/lines";
        private static string urlParameters = "?DOLAPIKEY=ecee5974867c0d45c5b8475a0cc2b9db2182f080";

        static void Main(string[] args)
        {
            /*
            // To call and get response from api
            RestClient restClient = new RestClient();
            restClient.endPoint = URL + urlParameters;

            Console.WriteLine("Rest Client created!");

            string strResponse = "";
            strResponse = restClient.makeRequest();

            Console.WriteLine("Result : \n"+strResponse);
            Console.WriteLine("\n");
            */

            /*
            // To donwload a file from server
            WebClient webClient = new WebClient();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            webClient.DownloadFileAsync(new Uri("http://82.253.71.109/prod/bdc_v11_04/api/test/test.txt"), localPath + "/_test_.txt");
            */

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        private static void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.WriteLine("Download completed");
        }

        private static void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int progress = 0;
            double receive = double.Parse(e.BytesReceived.ToString());
            double total = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = receive / total * 100;

            Console.WriteLine(string.Format("Downloaded {0:0.##}", percentage));
            Console.WriteLine("Value : "+int.Parse(Math.Truncate(percentage).ToString()));
        }

        // Pretty Print any Class Object JSON
        public static string FormatJson(Object obj)
        {
            var f = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            return f;
        }

        // Pretty Print any Class List Object JSON
        public static string FormatJson(List<Object> objList)
        {
            var f = Newtonsoft.Json.JsonConvert.SerializeObject(objList, Newtonsoft.Json.Formatting.Indented);
            return f;
        }
    }
}
