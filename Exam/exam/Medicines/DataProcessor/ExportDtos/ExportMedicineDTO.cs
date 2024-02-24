using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.DataProcessor.ExportDtos
{
    public class ExportMedicineDTO
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Price")]
        public decimal Price { get; set; }
        [JsonProperty ("Pharmacy")]
        public ExportPharmacyDTO [] Pharmacy { get; set; }
        
    }
}
