using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Service.Controllers
{
    //[Authorize]   Commented since Authentication and Authorization are not implemented.
    [RoutePrefix("api/Values")]
    public class ValuesController : ApiController
    {
        private IntervalDataRepositoryAbstract objDal = null;
        public ValuesController()
        {
            objDal = new IntervalRepository();
        }

        [HttpGet]   // GET api/values
        [AllowAnonymous]    //Using anonymous data access here as Authentication and Authorization are not implemented.
        [Route("GetHourlyIntervalData")]
        public List<IntervalDataVM> GetHourlyIntervalData()
        {
            try
            {
                return objDal.GetHourlyIntervalData();
            }
            catch (Exception ex)
            {
                //Log error and return empty list object - Log not implemented here.
                string error = ex.Message;
                return new List<IntervalDataVM>();
            }
        }
    }
}
