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
            [FromQuery] string BillingMonth,
            [FromQuery] string BillingYear,
            [FromQuery] List<string> BTNo,
            [FromServices] BillingService billingService)
        {
            try
            {
                if (BTNo == null || BTNo.Count == 0)
                    return BadRequest("At least one BTNo is required.");

                var report = new MaintenanceBillCursor();

                void SetParameter(string name, object value)
                {
                    if (report.Parameters[name] != null)
                    {
                        report.Parameters[name].Value = value;
                        report.Parameters[name].Visible = false;
                    }
                }

                SetParameter("BillingMonth", BillingMonth);
                SetParameter("BillingYear", BillingYear);

                // 👇 Pass comma-separated BTNos to SQL
                SetParameter("BTNo", string.Join(",", BTNo));

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
                return StatusCode(500, ex.Message);
            }
        }








        //[HttpGet("GetMBill")]
        //public async Task<IActionResult> GetMBill(
        //    [FromQuery] string BillingMonth,
        //    [FromQuery] string BillingYear,
        //    [FromQuery] string BTNo,
        //    [FromServices] BillingService billingService)
        //{
        //    try
        //    {
        //        // Initialize DevExpress report
        //        var report = new MaintenanceBill();

        //        if (report == null)
        //        {
        //            return StatusCode(500, "Failed to initialize the MaintenanceBill report.");
        //        }

        //        // Helper to set report parameters safely
        //        void SetParameter(string paramName, object value)
        //        {
        //            if (report.Parameters[paramName] != null)
        //            {
        //                report.Parameters[paramName].Value = value;
        //                report.Parameters[paramName].Visible = false;
        //            }
        //        }

        //        // Set required parameters
        //        SetParameter("BillingMonth", BillingMonth);
        //        SetParameter("BillingYear", BillingYear);
        //        SetParameter("BTNo", BTNo);

        //        // Increase SQL timeout
        //        if (report.DataSource is SqlDataSource sqlDataSource)
        //        {
        //            sqlDataSource.ConnectionOptions.CommandTimeout = 120;
        //        }

        //        // Export PDF
        //        using var stream = new MemoryStream();
        //        report.ExportToPdf(stream);
        //        stream.Seek(0, SeekOrigin.Begin);

        //        return File(stream.ToArray(), "application/pdf", "MaintenanceBill.pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error generating Maintenance Bill report: {ex.Message}");
        //    }
        //}
    }
}
