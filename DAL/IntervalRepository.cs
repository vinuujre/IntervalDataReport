using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public abstract class IntervalDataRepositoryAbstract
    {
        //Have a generic method to return the data in the table as half hourly data.
        public List<IntervalData> GetIntervalData()
        {
            IntervalDatabaseEntities db = new IntervalDatabaseEntities();

            List<IntervalData> data = db.IntervalData.ToList();
            return data;
        }

        public abstract List<IntervalDataVM> GetHourlyIntervalData();
    }

    public class IntervalRepository : IntervalDataRepositoryAbstract
    {
        public override List<IntervalDataVM> GetHourlyIntervalData()
        {
            //Get the half hourly data from the base class
            var rawData = base.GetIntervalData();

            //Convert half hourly data into hourly data grouping by hour
            List<IntervalDataVM> hourlyData = rawData.Select(x =>
            new IntervalDataVM()
            {
                DeliveryPoint = x.DeliveryPoint,
                Date = x.Date.ToString("dd/MM/yyyy"),
                TimeSlot = x.TimeSlot.Hours,  //Get the hour into the timeslot column leaving out the minutes and seconds
                SlotValue = x.SlotVal == null ? 0 : x.SlotVal.Value
            }).GroupBy(grp => new { grp.DeliveryPoint, grp.Date, grp.TimeSlot }).Select(y => //Group by DeliveryPoint, Date and TimeSlot which is only in hour now.
            new IntervalDataVM()
            {
                DeliveryPoint = y.Select(d => d.DeliveryPoint).FirstOrDefault(),
                Date = y.Select(d => d.Date).FirstOrDefault(),
                TimeSlot = y.Select(d => d.TimeSlot).FirstOrDefault(),
                SlotValue = Convert.ToDecimal(y.Sum(d => d.SlotValue).ToString("0.00")) //Sum the slot values for each hour and format the outcome
            }).ToList();

            return hourlyData;
        }
    }
}
