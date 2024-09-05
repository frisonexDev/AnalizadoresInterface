using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Online.Model
{
    public class ResultadoCFX
    {

        [JsonProperty("Well")]
        public string Well { get; set; }

        [JsonProperty("Fluor")]
        public string Fluor { get; set; }

        [JsonProperty("Target")]
        public string Target { get; set; }

        [JsonProperty("Content")]
        public string Content { get; set; }

        [JsonProperty("Sample")]
        public string Sample { get; set; }

        [JsonProperty("Cq")]
        public string Cq { get; set; }

        [JsonProperty("Starting Quantity (SQ)")]
        public string StartingQuantitySQ { get; set; }

       public decimal? Cqvalue() {

            if (Cq == "NaN")
                return null;
            else {
                string texto = Cq.Replace(",", ".");

                return Convert.ToDecimal(texto);

            }


        }
    }

}
