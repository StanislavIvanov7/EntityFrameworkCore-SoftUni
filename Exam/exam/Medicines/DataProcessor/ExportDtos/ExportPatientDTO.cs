using Medicines.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType ("Patient")]
    public class ExportPatientDTO
    {
        [XmlElement ("Name")]
        public string FullName { get; set; }
        [XmlElement ("AgeGroup")]
        public string AgeGroup { get; set; }
        [XmlAttribute ("Gander")]
        public string Gender { get; set; }
        [XmlArray ("Medicines")]
        public ExportMedicineDTO2 [] Medicines { get; set; }

    }
}
