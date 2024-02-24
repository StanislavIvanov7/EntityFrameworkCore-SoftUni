using Medicines.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medicines.Data.Models
{
    public class PatientMedicine
    {
        [Required]
        public int PatientId { get; set; }
        [ForeignKey (nameof(PatientId))]
        public virtual Patient Patient { get; set; }
        [Required]
        public int MedicineId { get; set; }
        [ForeignKey(nameof(MedicineId))]
        public virtual Medicine Medicine { get; set; }
    }
}
//•	PatientId – integer, Primary Key, foreign key (required)
//•	Patient – Patient
//•	MedicineId – integer, Primary Key, foreign key (required)
//•	Medicine – Medicine
