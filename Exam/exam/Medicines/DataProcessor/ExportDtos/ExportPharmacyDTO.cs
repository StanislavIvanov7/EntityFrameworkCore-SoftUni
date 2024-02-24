﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.DataProcessor.ExportDtos
{
    public class ExportPharmacyDTO
    {
        [JsonProperty("Name")]
        public string  Name { get; set; }
        [JsonProperty("PhoneNumber")]
       
        public string PhoneNumber { get; set; }

    }
}
