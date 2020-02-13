using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Web;
using Models;

namespace IntervalDataReport.Helpers
{
    //Implementing Bridge pattern so that the export method can be extended to other types of files.

    public interface IExporter
    {
        void ExportData(HttpResponseBase Response, List<IntervalDataVM> intervalData);
    }

    public abstract class DataForIntervals
    {
        public IExporter Exporter { get; set; }
        readonly string baseurl = ConfigurationManager.AppSettings["webApiBaseUrl"].ToString();
        readonly string HourlyIntervalDataUrl = ConfigurationManager.AppSettings["webApiGetHourlyIntervalDataUrl"].ToString();
        private HttpClient client;
        public abstract void Export();

        //Generic method to get the data
        public List<IntervalDataVM> GetHourlyIntervalData()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(baseurl);
            client.DefaultRequestHeaders.Accept.Clear();
            var result = new List<IntervalDataVM>();
            try
            {
                HttpResponseMessage responseMessage = client.GetAsync(HourlyIntervalDataUrl).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = responseMessage.Content.ReadAsStringAsync().Result;
                    result = JsonConvert.DeserializeObject<List<IntervalDataVM>>(response);
                }
                return result;
            }
            catch (Exception ex)
            {
                //Log error and return empty list object - Log not implemented here.
                string error = ex.Message;
                return result;
            }
        }
    }

    //Concrete implementation of abstract class
    public class ExportData : DataForIntervals
    {
        HttpResponseBase _response = null;
        public ExportData(HttpResponseBase Response)
        {
            _response = Response;
        }


        public override void Export()
        {
            var intervalData = base.GetHourlyIntervalData();
            Exporter.ExportData(_response, intervalData);
        }
    }

    //Specific implementation for Excel Export
    public class ExcelExporter : IExporter
    {
        public void ExportData(HttpResponseBase Response, List<IntervalDataVM> intervalData)
        {
            ExcelPackage excel = new ExcelPackage();
            string decimalCellFormat = "0.00";

            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

            using (ExcelRange Rng = workSheet.Cells["E2:E" + intervalData.Count.ToString()])    //Formatting the range for decimal values in excel
            {
                Rng.Style.Numberformat.Format = decimalCellFormat;
            }
            workSheet.Cells[1, 1].LoadFromCollection(intervalData, true);

            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=IntervalData.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
    }
}