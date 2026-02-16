using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SyntroVaccPApp.Models;

public class ReportService
{
    public byte[] GenerateImmunisationReport(Patient patient, List<Administration> administrations)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);

                page.Header()
                    .Text("Patient Immunisation Report")
                    .FontSize(20)
                    .Bold()
                    .AlignCenter();

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text($"Patient: {patient.FirstName} {patient.LastName}");
                    col.Item().Text($"Date of Birth: {patient.DateOfBirth:yyyy-MM-dd}");
                    col.Item().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm}");

                    col.Item().LineHorizontal(1);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Vaccine").Bold();
                            header.Cell().Text("Dose").Bold();
                            header.Cell().Text("Date").Bold();
                            header.Cell().Text("Facility").Bold();
                        });

                        foreach (var admin in administrations)
                        {
                            table.Cell().Text(admin.Vaccine?.Name);
                            table.Cell().Text(admin.DoseNumber.ToString());
                            table.Cell().Text(admin.AdministeredOn.ToString("yyyy-MM-dd"));
                            table.Cell().Text(admin.Facility?.FacilityName);
                        }
                    });
                });

                page.Footer()
                    .AlignCenter()
                    .Text("Confidential Medical Record");
            });
        }).GeneratePdf();
    }

    public byte[] GenerateBatchStockReport(List<VaccineBatch> batches)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontColor(Colors.Black));

                // HEADER
                page.Header()
                    .Text("Vaccine Batch Stock Report")
                    .SemiBold()
                    .FontSize(18)
                    .FontColor("#004d5d")
                    .AlignCenter();

                // CONTENT
                page.Content().Column(col =>
                {
                    col.Spacing(5);

                    col.Item().Table(table =>
                    {
                        // Columns: Batch, Vaccine, Expiry, Status
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        // HEADER ROW
                        table.Header(header =>
                        {
                            IContainer HeaderStyle(IContainer c)
                            => c.Background("#004d5d").
                            Padding(5).
                            AlignCenter().
                            DefaultTextStyle(x=> x.FontColor(Colors.White).SemiBold());


                            header.Cell().Element(HeaderStyle).Text("Batch Number");
                            header.Cell().Element(HeaderStyle).Text("Vaccine");
                            header.Cell().Element(HeaderStyle).Text("Expiry Date");
                            header.Cell().Element(HeaderStyle).Text("Status");
                        });

                        // ROWS
                        foreach (var batch in batches)
                        {
                            var status = batch.ExpiryDate < DateTime.Today ? "Expired" :
                                         batch.ExpiryDate <= DateTime.Today.AddDays(30) ? "Expiring Soon" : "Active";

                            table.Cell().AlignCenter().Text(batch.BatchNumber);
                            table.Cell().AlignCenter().Text(batch.Vaccine?.Name ?? "General");
                            table.Cell().AlignCenter().Text(batch.ExpiryDate.ToString("dd MMM yyyy"));
                            table.Cell().AlignCenter().Text(status);
                        }
                    });
                });

                // FOOTER
                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Generated on: ").SemiBold().FontColor("#004d5d");                  

                    txt.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm"));

                    txt.Span("\nCopyright of ");
                    txt.Span("BrandMed Group").SemiBold().FontColor("#004d5d");
                });
            });
        }).GeneratePdf();
    }

}
