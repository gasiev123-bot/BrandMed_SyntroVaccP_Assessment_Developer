using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SyntroVaccPApp.Models;

namespace SyntroVaccPApp.Data
{
    public static class SeedData
    {
        public static void Initialize(SyntroVaccPAppDbContext context)
        {
            context.Database.EnsureCreated();

            // Seed Countries
            if (!context.Countries.Any())
            {
                context.Countries.AddRange(
                    new Country { CountryCode = "AD", CountryName = "Andorra" },
                    new Country { CountryCode = "AE", CountryName = "United Arab Emirates" },
                    new Country { CountryCode = "AF", CountryName = "Afghanistan" },
                    new Country { CountryCode = "AG", CountryName = "Antigua and Barbuda" },
                    new Country { CountryCode = "AL", CountryName = "Albania" },
                    new Country { CountryCode = "AM", CountryName = "Armenia" },
                    new Country { CountryCode = "AO", CountryName = "Angola" },
                    new Country { CountryCode = "AR", CountryName = "Argentina" },
                    new Country { CountryCode = "AT", CountryName = "Austria" },
                    new Country { CountryCode = "AU", CountryName = "Australia" },
                    new Country { CountryCode = "AZ", CountryName = "Azerbaijan" },
                    new Country { CountryCode = "BA", CountryName = "Bosnia and Herzegovina" },
                    new Country { CountryCode = "BB", CountryName = "Barbados" },
                    new Country { CountryCode = "BD", CountryName = "Bangladesh" },
                    new Country { CountryCode = "BE", CountryName = "Belgium" },
                    new Country { CountryCode = "BF", CountryName = "Burkina Faso" },
                    new Country { CountryCode = "BG", CountryName = "Bulgaria" },
                    new Country { CountryCode = "BH", CountryName = "Bahrain" },
                    new Country { CountryCode = "BI", CountryName = "Burundi" },
                    new Country { CountryCode = "BJ", CountryName = "Benin" },
                    new Country { CountryCode = "BN", CountryName = "Brunei" },
                    new Country { CountryCode = "BO", CountryName = "Bolivia" },
                    new Country { CountryCode = "BR", CountryName = "Brazil" },
                    new Country { CountryCode = "BS", CountryName = "Bahamas" },
                    new Country { CountryCode = "BT", CountryName = "Bhutan" },
                    new Country { CountryCode = "BW", CountryName = "Botswana" },
                    new Country { CountryCode = "BY", CountryName = "Belarus" },
                    new Country { CountryCode = "BZ", CountryName = "Belize" },
                    new Country { CountryCode = "CA", CountryName = "Canada" },
                    new Country { CountryCode = "CF", CountryName = "Central African Republic" },
                    new Country { CountryCode = "CG", CountryName = "Congo" },
                    new Country { CountryCode = "CH", CountryName = "Switzerland" },
                    new Country { CountryCode = "CL", CountryName = "Chile" },
                    new Country { CountryCode = "CM", CountryName = "Cameroon" },
                    new Country { CountryCode = "CN", CountryName = "China" },
                    new Country { CountryCode = "CO", CountryName = "Colombia" },
                    new Country { CountryCode = "CR", CountryName = "Costa Rica" },
                    new Country { CountryCode = "CU", CountryName = "Cuba" },
                    new Country { CountryCode = "CV", CountryName = "Cape Verde" },
                    new Country { CountryCode = "CY", CountryName = "Cyprus" },
                    new Country { CountryCode = "CZ", CountryName = "Czech Republic" },
                    new Country { CountryCode = "DE", CountryName = "Germany" },
                    new Country { CountryCode = "DJ", CountryName = "Djibouti" },
                    new Country { CountryCode = "DK", CountryName = "Denmark" },
                    new Country { CountryCode = "DM", CountryName = "Dominica" },
                    new Country { CountryCode = "DO", CountryName = "Dominican Republic" },
                    new Country { CountryCode = "DZ", CountryName = "Algeria" },
                    new Country { CountryCode = "EC", CountryName = "Ecuador" },
                    new Country { CountryCode = "EE", CountryName = "Estonia" },
                    new Country { CountryCode = "EG", CountryName = "Egypt" },
                    new Country { CountryCode = "ER", CountryName = "Eritrea" },
                    new Country { CountryCode = "ES", CountryName = "Spain" },
                    new Country { CountryCode = "ET", CountryName = "Ethiopia" },
                    new Country { CountryCode = "FI", CountryName = "Finland" },
                    new Country { CountryCode = "FJ", CountryName = "Fiji" },
                    new Country { CountryCode = "FM", CountryName = "Micronesia" },
                    new Country { CountryCode = "FR", CountryName = "France" },
                    new Country { CountryCode = "GA", CountryName = "Gabon" },
                    new Country { CountryCode = "GB", CountryName = "United Kingdom" },
                    new Country { CountryCode = "GD", CountryName = "Grenada" },
                    new Country { CountryCode = "GE", CountryName = "Georgia" },
                    new Country { CountryCode = "GH", CountryName = "Ghana" },
                    new Country { CountryCode = "GM", CountryName = "Gambia" },
                    new Country { CountryCode = "GN", CountryName = "Guinea" },
                    new Country { CountryCode = "GQ", CountryName = "Equatorial Guinea" },
                    new Country { CountryCode = "GR", CountryName = "Greece" },
                    new Country { CountryCode = "GT", CountryName = "Guatemala" },
                    new Country { CountryCode = "GW", CountryName = "Guinea-Bissau" },
                    new Country { CountryCode = "GY", CountryName = "Guyana" },
                    new Country { CountryCode = "HN", CountryName = "Honduras" },
                    new Country { CountryCode = "HR", CountryName = "Croatia" },
                    new Country { CountryCode = "HT", CountryName = "Haiti" },
                    new Country { CountryCode = "HU", CountryName = "Hungary" },
                    new Country { CountryCode = "ID", CountryName = "Indonesia" },
                    new Country { CountryCode = "IE", CountryName = "Ireland" },
                    new Country { CountryCode = "IL", CountryName = "Israel" },
                    new Country { CountryCode = "IN", CountryName = "India" },
                    new Country { CountryCode = "IQ", CountryName = "Iraq" },
                    new Country { CountryCode = "IR", CountryName = "Iran" },
                    new Country { CountryCode = "IS", CountryName = "Iceland" },
                    new Country { CountryCode = "IT", CountryName = "Italy" },
                    new Country { CountryCode = "JM", CountryName = "Jamaica" },
                    new Country { CountryCode = "JO", CountryName = "Jordan" },
                    new Country { CountryCode = "JP", CountryName = "Japan" },
                    new Country { CountryCode = "KE", CountryName = "Kenya" },
                    new Country { CountryCode = "KG", CountryName = "Kyrgyzstan" },
                    new Country { CountryCode = "KH", CountryName = "Cambodia" },
                    new Country { CountryCode = "KI", CountryName = "Kiribati" },
                    new Country { CountryCode = "KM", CountryName = "Comoros" },
                    new Country { CountryCode = "KN", CountryName = "Saint Kitts and Nevis" },
                    new Country { CountryCode = "KP", CountryName = "North Korea" },
                    new Country { CountryCode = "KR", CountryName = "South Korea" },
                    new Country { CountryCode = "KW", CountryName = "Kuwait" },
                    new Country { CountryCode = "KZ", CountryName = "Kazakhstan" },
                    new Country { CountryCode = "LA", CountryName = "Laos" },
                    new Country { CountryCode = "LB", CountryName = "Lebanon" },
                    new Country { CountryCode = "LC", CountryName = "Saint Lucia" },
                    new Country { CountryCode = "LI", CountryName = "Liechtenstein" },
                    new Country { CountryCode = "LK", CountryName = "Sri Lanka" },
                    new Country { CountryCode = "LR", CountryName = "Liberia" },
                    new Country { CountryCode = "LS", CountryName = "Lesotho" },
                    new Country { CountryCode = "LT", CountryName = "Lithuania" },
                    new Country { CountryCode = "LU", CountryName = "Luxembourg" },
                    new Country { CountryCode = "LV", CountryName = "Latvia" },
                    new Country { CountryCode = "LY", CountryName = "Libya" },
                    new Country { CountryCode = "MA", CountryName = "Morocco" },
                    new Country { CountryCode = "MC", CountryName = "Monaco" },
                    new Country { CountryCode = "MD", CountryName = "Moldova" },
                    new Country { CountryCode = "ME", CountryName = "Montenegro" },
                    new Country { CountryCode = "MG", CountryName = "Madagascar" },
                    new Country { CountryCode = "MH", CountryName = "Marshall Islands" },
                    new Country { CountryCode = "ML", CountryName = "Mali" },
                    new Country { CountryCode = "MM", CountryName = "Myanmar" },
                    new Country { CountryCode = "MN", CountryName = "Mongolia" },
                    new Country { CountryCode = "MR", CountryName = "Mauritania" },
                    new Country { CountryCode = "MT", CountryName = "Malta" },
                    new Country { CountryCode = "MU", CountryName = "Mauritius" } 
                );

                context.SaveChanges();
            }

            // Seed Clinicians
            if (!context.Clinicians.Any())
            {
                context.Clinicians.AddRange(
                    new Clinician { ClinicianId = 2, RegistrationNumber = "MP10001", FirstName = "Nuno", LastName = "Ramoz", Title = "MD", Specialty = "Technology / IT", FacilityId = 10, Email = "nuno.ramoz@brandmed.co.za", Phone = "+27 11 1234", IsActive = true },
                    new Clinician { ClinicianId = 3, RegistrationNumber = "MP10002", FirstName = "Sarah", LastName = "Botha", Title = "MD", Specialty = "General Practitioner", FacilityId = 10, Email = "sarah.botha@brandmed.co.za", Phone = "+27 11 5678", IsActive = true },
                    new Clinician { ClinicianId = 4, RegistrationNumber = "RN20001", FirstName = "Thabo", LastName = "Mokoena", Title = "RN", Specialty = "Nursing", FacilityId = 11, Email = "thabo.mokoena@brandmed.co.za", Phone = "+27 21 3456", IsActive = true },
                    new Clinician { ClinicianId = 5, RegistrationNumber = "MP10003", FirstName = "Linda", LastName = "van der Merwe", Title = "MD", Specialty = "Pediatrics", FacilityId = 11, Email = "linda.vdmerwe@brandmed.co.za", Phone = "+27 21 7890", IsActive = true },
                    new Clinician { ClinicianId = 6, RegistrationNumber = "MP10004", FirstName = "Johan", LastName = "Kruger", Title = "MD", Specialty = "Internal Medicine", FacilityId = 10, Email = "johan.kruger@brandmed.co.za", Phone = "+27 11 9012", IsActive = true }
                );

                context.SaveChanges();
            }

            // Seed Administrations
            if (!context.Administrations.Any())
            {
                context.Administrations.AddRange(
                    new Administration { AdministrationId = 1, PatientId = 1, VaccineId = 3, FacilityId = 23, ClinicianId = 4, DoseNumber = 1, AdministeredOn = DateTime.Parse("2026-02-14"), BatchNumber = "SAN-001-260214", CreatedUtc = DateTime.Parse("2026-02-14T19:51:46"), CreatedBy = "", BatchId = 2013, UpdatedUtc = DateTime.Parse("2026-02-14T22:30:19"), UpdatedBy = null, Notes = "Edit Test" },
                    new Administration { AdministrationId = 2, PatientId = 1004, VaccineId = 3, FacilityId = 23, ClinicianId = 4, DoseNumber = 1, AdministeredOn = DateTime.Parse("2026-02-14"), BatchNumber = "SAN-001-260214", CreatedUtc = DateTime.Parse("2026-02-14T19:53:24"), CreatedBy = "", BatchId = 2013, UpdatedUtc = DateTime.Parse("2026-02-14T19:53:24"), UpdatedBy = null, Notes = "dddd" },
                    new Administration { AdministrationId = 1002, PatientId = 2022, VaccineId = 7, FacilityId = 23, ClinicianId = 4, DoseNumber = 1, AdministeredOn = DateTime.Parse("2025-01-01"), BatchNumber = "HEPB-001", CreatedUtc = DateTime.Parse("2026-02-16T11:19:43"), CreatedBy = "", BatchId = 3010, UpdatedUtc = DateTime.Parse("2026-02-16T11:19:43"), UpdatedBy = null, Notes = "Test Overdue" }
               
                );

                context.SaveChanges();
            }

            // Seed AuditLogs
            if (!context.AuditLogs.Any())
            {
                context.AuditLogs.AddRange(
                    new AuditLog { AuditLogId = 1046, EntityName = "VaccineBatch", EntityId = 0, Action = "INSERT", OldValue = null, NewValue = null, ActionDate = DateTime.Parse("2026-02-12T00:22:12"), ChangedBy = "admin@syntrop.com", ChangedUtc = null, ChangeSummary = "Added new batch Batch-2024-WHYVELE for VaccineId 7" },
                    new AuditLog { AuditLogId = 1047, EntityName = "Vaccine", EntityId = 20, Action = "Update", OldValue = null, NewValue = null, ActionDate = DateTime.Parse("2026-02-13T12:23:32"), ChangedBy = "AdminUser", ChangedUtc = null, ChangeSummary = "Updated vaccine definition: BCG (Tuberculosis)" }
                  
                );

                context.SaveChanges();
            }
        }
    }
}
