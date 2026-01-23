using API_Printing.Reports;
using API_Printing.Services;
using Microsoft.AspNetCore.Mvc;
using DevExpress.DataAccess.Sql;
using System;
using System.IO;
using System.Threading.Tasks;

namespace API_Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SSQCursorMaintenanceController : ControllerBase
    {


        [HttpGet("GetMBill")]
        public async Task<IActionResult> GetMBill(
            [FromQuery] List<string> BillingMonths,
            [FromQuery] List<string> BillingYears,
            [FromQuery] List<string> BTNo,
            [FromServices] BillingService billingService)
        {
            try
            {
                // Validation
                if (BTNo == null || BTNo.Count == 0)
                    return BadRequest("At least one BTNo is required.");
                if (BillingMonths == null || BillingMonths.Count == 0)
                    return BadRequest("At least one BillingMonth is required.");
                if (BillingYears == null || BillingYears.Count == 0)
                    return BadRequest("At least one BillingYear is required.");

                // Check if all lists have same count (for combination matching)
                if (BTNo.Count != BillingMonths.Count || BTNo.Count != BillingYears.Count)
                {
                    // If counts don't match, we'll use unique combinations
                    Console.WriteLine($"Warning: Count mismatch - BTNo: {BTNo.Count}, Months: {BillingMonths.Count}, Years: {BillingYears.Count}");
                }

                var report = new MaintenanceBillCursor();

                void SetParameter(string name, object value)
                {
                    if (report.Parameters[name] != null)
                    {
                        report.Parameters[name].Value = value;
                        report.Parameters[name].Visible = false;
                        Console.WriteLine($"Parameter {name} set to: {value}");
                    }
                }

                // Convert lists to comma-separated strings for SQL
                // Use Distinct() to remove duplicates if needed
                SetParameter("BillingMonth", string.Join(",", BillingMonths.Distinct()));
                SetParameter("BillingYear", string.Join(",", BillingYears.Distinct()));
                SetParameter("BTNo", string.Join(",", BTNo.Distinct()));

                if (report.DataSource is SqlDataSource ds)
                {
                    ds.ConnectionOptions.CommandTimeout = 120;
                }

                using var stream = new MemoryStream();
                report.ExportToPdf(stream);
                stream.Position = 0;

                return File(stream.ToArray(), "application/pdf", "MaintenanceBill.pdf");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
