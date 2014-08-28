using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemberShipMVC.Models.ViewsModels
{
    public class DailyReportExcel
    {
        public DateTime Date { get; set; }

        public string Details { get; set; }

        public decimal Hours { get; set; }   
    }
}