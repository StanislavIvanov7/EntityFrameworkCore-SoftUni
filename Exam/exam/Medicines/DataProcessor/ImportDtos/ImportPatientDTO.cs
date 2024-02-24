using Medicines.Data.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.DataProcessor.ImportDtos
{
    public class ImportPatientDTO
    {
        [Required]
        [MaxLength(100)]
        [MinLength (5)]
        [JsonProperty  ("FullName")]
        public string FullName { get; set; }
        [Required]
        [Range (0, 2)]
        [JsonProperty("AgeGroup")]
        public string AgeGroup { get; set; }
        [Required]
        [Range (0, 1)]
        [JsonProperty("Gender")]
        public string Gender { get; set; }
        [JsonProperty("Medicines")]
        public int[] Medicines { get; set; }
    }
}
//•	FullName – text with length [5, 100] (required)
//•	AgeGroup – AgeGroup enum (Child = 0, Adult, Senior)(required)
//•	Gender – Gender enum (Male = 0, Female)(required)
//•	PatientsMedicines - collection of type PatientMedicine