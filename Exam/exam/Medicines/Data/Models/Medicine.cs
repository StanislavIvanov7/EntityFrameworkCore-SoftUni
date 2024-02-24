using Medicines.Data.Models;
using Medicines.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;

namespace Medicines.Data.Models
{
    public class Medicine
    {
        public Medicine()
        {
            PatientsMedicines = new List<PatientMedicine>();
        }
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength (150)]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public Category Category { get; set; }
        [Required]
        public DateTime ProductionDate { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Required]
        [MaxLength(100)]
        public string Producer { get; set; }
        [Required]
        public int PharmacyId { get; set; }
        [ForeignKey (nameof(PharmacyId ))]
        public virtual Pharmacy Pharmacy { get; set; }
        public virtual ICollection<PatientMedicine> PatientsMedicines { get; set; }

    }
}
//•	Id – integer, Primary Key
//•	Name – text with length [3, 150] (required)
//•	Price – decimal in range [0.01…1000.00] (required)
//•	Category – Category enum (Analgesic = 0, Antibiotic, Antiseptic, Sedative, Vaccine)(required)
//•	ProductionDate – DateTime (required)
//•	ExpiryDate – DateTime (required)
//•	Producer – text with length [3, 100] (required)
//•	PharmacyId – integer, foreign key (required)
//•	Pharmacy – Pharmacy
//•	PatientsMedicines - collection of type PatientMedicine
