using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemberShipMVC.Models.ViewsModels
{
    public class DailyReport
    {
        public DateTime Date { get; set; }

        public List<string> Details { get; set; }

        public decimal Hours { get; set; }
    }
}