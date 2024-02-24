namespace Medicines.DataProcessor
{
    using Artillery.Utilities;
   
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            var patientsDtos = JsonConvert.DeserializeObject<ImportPatientDTO[]>(jsonString);
            StringBuilder sb = new StringBuilder();
            var existId = context.Medicines .Select (x=>x.Id).ToArray();
            var validPatient = new List<Patient>();
            foreach (var patientDto in patientsDtos)
            {
                if (!IsValid(patientDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Patient patient = new Patient()
                {
                    FullName = patientDto.FullName,
                    AgeGroup = (AgeGroup)int.Parse(patientDto .AgeGroup),
                    Gender = (Gender)int.Parse(patientDto .Gender )
                };

                foreach (var ids in patientDto .Medicines)
                {
                    //if(!existId .Contains(ids))
                    //{
                    //    sb.AppendLine(ErrorMessage);
                    //    continue;
                    //}
                    if(patient.PatientsMedicines.Any(x=>x.MedicineId == ids))
                    {
                        sb.AppendLine (ErrorMessage); continue;
                    }
                    PatientMedicine patientMedicine = new PatientMedicine()
                    {
                        MedicineId = ids,
                    };
                    //if (patient.PatientsMedicines.Any(x => x.MedicineId == ids)) //!
                    //{
                    //    sb.AppendLine(ErrorMessage);
                    //    continue;
                    //}
                    patient.PatientsMedicines.Add(patientMedicine);
                }

               validPatient .Add(patient);
                sb.AppendLine(string.Format(SuccessfullyImportedPatient, patient.FullName, patient.PatientsMedicines.Count()));
            }
            context.Patients.AddRange(validPatient);
            context .SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            var pharmaciesDtos = XmlHelper.Deserialize<ImportPharmacyDTO[]>(xmlString, "Pharmacies");
            StringBuilder sb = new StringBuilder();
            var validPharmacies = new List<Pharmacy>();
            foreach (var pharmacyDto in pharmaciesDtos)
            {
                if(!IsValid(pharmacyDto))
                {
                    sb.AppendLine (ErrorMessage );
                    continue;
                }
                if(pharmacyDto .IsNonStop != "true" && pharmacyDto .IsNonStop !="false")
                {
                    sb.AppendLine (ErrorMessage );
                    continue;
                }
                Pharmacy pharmacy = new Pharmacy()
                {
                    IsNonStop = bool.Parse (pharmacyDto.IsNonStop),
                    Name = pharmacyDto.Name,
                   PhoneNumber = pharmacyDto.PhoneNumber,
                };
                foreach (var medicineDto in pharmacyDto .Medicines)
                {
                    if (!IsValid(medicineDto))
                    {
                        sb.AppendLine (ErrorMessage );
                        continue;
                    }
                   
                    DateTime producerDatePrimer;
                    bool isProducerDatePrimerValid = DateTime.TryParseExact
                        (medicineDto.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                        out producerDatePrimer);
                    if(!isProducerDatePrimerValid)
                    {
                        sb .AppendLine (ErrorMessage );
                        continue;
                    }
                    DateTime expiryDatePrimer;
                    bool isExpiryDatePrimerValid = DateTime.TryParseExact
                        (medicineDto.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                        out expiryDatePrimer);
                    if (!isExpiryDatePrimerValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    if(producerDatePrimer >= expiryDatePrimer )
                    {
                        sb.AppendLine (ErrorMessage ); 
                        continue;
                    }
                    
                    
                    if(pharmacy .Medicines.Any (x=> x.Name == medicineDto .Name && x.Producer== medicineDto.Producer))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    };
                    Medicine medicine = new Medicine()
                    {
                        Category = (Category)int.Parse(medicineDto.Category),
                        Name = medicineDto.Name,
                        Price = medicineDto.Price,
                        ProductionDate = producerDatePrimer,
                        ExpiryDate = expiryDatePrimer,
                        Producer = medicineDto.Producer,
                    };
                    pharmacy .Medicines .Add(medicine);
                   
                }
                validPharmacies.Add( pharmacy );
                sb.AppendLine(string.Format(SuccessfullyImportedPharmacy, pharmacy.Name, pharmacy.Medicines.Count()));
            }
            context .Pharmacies.AddRange (validPharmacies );
            context .SaveChanges ();
            return sb.ToString ().TrimEnd ();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
