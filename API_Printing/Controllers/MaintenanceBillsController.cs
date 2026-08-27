using API_Printing.Reports;
using API_Printing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.UI;
using System;
using System.IO;
using System.Threading.Tasks;

namespace API_Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceBillController : ControllerBase
    {
        [HttpGet("GetMBill")]
        public async Task<IActionResult> GetMBill(
            [FromQuery] string? block,
            [FromQuery] string? Category,
            [FromQuery] string? month,
            [FromQuery] string? year,
            [FromQuery] string? Project,
            [FromServices] BillingService billingService)
        {
            try
            {
                // ✅ Initialize your DevExpress report
                var report = new MaintenanceBill();

                if (report == null)
                {
                    return StatusCode(500, "Failed to initialize the MaintenanceBill report.");
                }

                // ✅ Helper to safely assign parameters
                void SetParameter(string paramName, object? value)
                {
                    if (report.Parameters[paramName] != null)
                    {
                        report.Parameters[paramName].Value = value;
                        report.Parameters[paramName].Visible = false;
                    }
                }

                // ✅ Set all report parameters
                SetParameter("Category", Category);
                SetParameter("Block", block);
                SetParameter("BillingMonth", month);
                SetParameter("BillingYear", year);
                SetParameter("Project", Project);

                // ✅ Filter: Exclude AMI Meter bills (where AMI != 'AMI')
                report.FilterString = "[AMI] != 'AMI'";

                // ✅ Increase SQL command timeout to 120s
                if (report.DataSource is SqlDataSource sqlDataSource)
                {
                    sqlDataSource.ConnectionOptions.CommandTimeout = 120;
                }

                // ✅ Export to PDF
                using var stream = new MemoryStream();
                report.ExportToPdf(stream);
                stream.Seek(0, SeekOrigin.Begin);

                return File(stream.ToArray(), "application/pdf", "MaintenanceBill.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating Maintenance Bill report: {ex.Message}");
            }
        }
    }
}
