using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessIMC.Models
{
    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public double IMC { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
