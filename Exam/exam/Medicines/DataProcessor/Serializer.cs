namespace Medicines.DataProcessor
{
    using Artillery.Utilities;
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ExportDtos;
    using Newtonsoft.Json;
    using System.Diagnostics;
    using System.Diagnostics.Metrics;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.Intrinsics.X86;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            
            DateTime producerDatePrimer;
            bool isProducerDatePrimerValid = DateTime.TryParseExact
                (date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out producerDatePrimer);
            var patient = context.Patients
                .Where(x => x.PatientsMedicines.Any(x => x.Medicine.ProductionDate > producerDatePrimer))
                .ToArray()
               //.OrderByDescending(x => x.PatientsMedicines.Count()).ThenBy(x => x.FullName)
                .Select(x => new ExportPatientDTO
                {
                    Gender = x.Gender.ToString().ToLower(),
                    FullName = x.FullName,
                    AgeGroup = x.AgeGroup.ToString(),
                    Medicines = x.PatientsMedicines
                    .Where (x=>x.Medicine .ProductionDate > producerDatePrimer)
                    .OrderByDescending(x => x.Medicine.ExpiryDate)
                    .ThenBy(x => x.Medicine .Price)
                    .Select( x=> new ExportMedicineDTO2
                    {
                        Category = x.Medicine .Category .ToString().ToLower(),
                        Name = x.Medicine .Name,
                        Price = x.Medicine .Price.ToString("f2") ,
                        Producer = x.Medicine .Producer ,
                        ExpiryDate = x.Medicine .ExpiryDate.ToString("yyyy-MM-dd",CultureInfo .InvariantCulture),
                    })
                    .ToArray ()
                })
                .OrderByDescending (x=>x.Medicines .Length)
                .ThenBy (x=>x.FullName)
                .ToArray ();

            var result = XmlHelper.Serialize<ExportPatientDTO[]>(patient, "Patients");
            return result;

        }

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            //Select all the medicines, from a specific category(for this task the category is hardcoded in the
            //StartUp class and passed to the method), that can be found in pharmacies working 24/7 (non-stop).
            //Select them with their name, price, pharmacy.For the pharmacy, export its name and phone number.
            //Order the medicines by price (ascending) and then by name (alphabetically).
            //In the exported document, the price should be formatted to the second decimal place and exported to string format.
            var medicines = context.Medicines
                .Where(x => x.Category == (Category)medicineCategory && x.Pharmacy.IsNonStop == true)
                .OrderBy(x => x.Price).ThenBy(x => x.Name)
                .Select(x => new
                {


                    Name = x.Name,
                    Price = x.Price.ToString("f2"),
                    Pharmacy = new
                    {
                        Name = x.Pharmacy.Name,
                        PhoneNumber = x.Pharmacy.PhoneNumber
                    }




                }).ToArray ();
             var result = JsonConvert .SerializeObject(medicines, Formatting.Indented);
            return result;
        }
    }
}

