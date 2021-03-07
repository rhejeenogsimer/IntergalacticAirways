using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("--Intergalactic Airways--" + "\r\n");
            Console.WriteLine("Input number of passengers:" + "\r\n");

            int passengers = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine(@"Starships - Pilots:" + "\r\n");
            Console.WriteLine(getSuitablestarShips(passengers));
            Console.WriteLine("--Intergalactic Airways--");
        }

        public static string getSuitablestarShips(int passengers)
        {
            var outputString = "";
            using (WebClient wc = new WebClient())
            {
                var starShipsJson = GetApi("https://swapi.dev/api/starships/");
                JObject parsedObj = JObject.Parse(starShipsJson);
                var results = parsedObj["results"].AsEnumerable();

                int starShipPassengers;
                foreach (var res in results)
                {
                    if (Int32.TryParse(res["passengers"].Value<string>(), out starShipPassengers) && res["pilots"].ToArray().Length != 0)
                    {
                        if (starShipPassengers >= passengers)
                        {
                            foreach (var pilotUrl in res["pilots"])
                            {
                                outputString += res["name"] + " - " + getPilotName(pilotUrl.ToString()) + "\r\n";
                            }
                        }
                    }
                }

            }
            return outputString;
        }

        public static string getPilotName(string url)
        {
            using (WebClient wc = new WebClient())
            {
                var pilotJson = GetApi(url);
                var pilot = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(pilotJson);

                return pilot["name"];
            }
        }

        public static string GetApi(string ApiUrl)
        {
            var responseString = "";
            var request = (HttpWebRequest)WebRequest.Create(ApiUrl);
            request.Method = "GET";
            request.ContentType = "application/json";

            using (var response1 = request.GetResponse())
            {
                using (var reader = new StreamReader(response1.GetResponseStream()))
                {
                    responseString = reader.ReadToEnd();
                }
            }
            return responseString;
        }
    }
}
