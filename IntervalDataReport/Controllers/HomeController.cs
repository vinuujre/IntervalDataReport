using IntervalDataReport.Helpers;
using System.Web.Mvc;

namespace IntervalDataReport.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public void ExportToExcel()
        {
            //Calling the bridge pattern and assigning the excel export class.
            IExporter exporter = new ExcelExporter();
            DataForIntervals dataForIntervals = new ExportData(Response);   //Injecting Response object for processing
            dataForIntervals.Exporter = exporter;   //Injecting the Excel class object
            dataForIntervals.Export();  //Calling the excel export method
        }
    }
}