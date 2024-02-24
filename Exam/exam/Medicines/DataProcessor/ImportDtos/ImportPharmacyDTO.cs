using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Pharmacy")]
    public class ImportPharmacyDTO
    {
        [Required]
        [XmlAttribute ("non-stop")]
        public string IsNonStop { get; set; }
        [Required]
        [MaxLength(50)]
        [XmlElement ("Name")]
        public string Name { get; set; }
        [Required]
        [MaxLength(14)]
        [XmlElement ("PhoneNumber")]
        [RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$")]
        public string PhoneNumber { get; set; }
        [XmlArray ("Medicines")]
        public ImportMedicineDTO [] Medicines { get; set; }
    }
}
