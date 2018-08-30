using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfideHome.Models
{
    public class Twodsphere
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("coordinates")]
        public IList<double> coordinates { get; set; }
    }

    public class ForMap 
    {
        public string _id { get; set; }
        public string JmlsNo { get; set; }
        public string MlsNo { get; set; }
        public string Address { get; set; }
        public string Bedrooms { get; set; }
        public string Washrooms { get; set; }
        public string Garage { get; set; }
        public string Footage { get; set; }
        public string HouseType { get; set; }
        public string ListingDate { get; set; }
        public string ListingPrice { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionPrice { get; set; }
        public string LastStatus { get; set; }
        public string City { get; set; }
        public string Community { get; set; }
        public string District { get; set; }
        public string MCity { get; set; }
        public string MAddress1 { get; set; }
        public string MAddress2 { get; set; }
        [JsonProperty("Location")]
        public Twodsphere Location { get; set; }

        public bool Active { get; set; }
    }
}
