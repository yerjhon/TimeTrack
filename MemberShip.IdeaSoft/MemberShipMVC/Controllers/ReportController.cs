using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MemberShipMVC.Models;
using MemberShipMVC.Models.ViewsModels;
using MemberShipMVC.Repositores;
using MemberShipMVC.Repositores.Interfaces;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace MemberShipMVC.Controllers
{
    [Authorize(Roles = "Administrator, Developer, Quality Engineer")]
    public class ReportController : Controller
    {
        
        IReportRepository iReportRepository = null;



        public ReportController()
        {
            this.iReportRepository = new ReportRepository();
        }

        public ReportController(IReportRepository iReportRepository)
        {
            this.iReportRepository = iReportRepository;
        }

        public ActionResult Index()
        {
            if (iReportRepository.IsAdministrator(User.Identity.Name))
            {
                var SelectUser = new SelectList(iReportRepository.GetAllsUser(), "UserId", "UserName");
                ViewData["listUsers"] = SelectUser;            
            }
            return View();
        }

        [HttpGet]
        public ActionResult GetReport(string InitiaDate, string EndDate, string IdUser)
        {
            DateTime dtmInitDate = DateTime.Parse(InitiaDate);
            DateTime dtmEndDate = DateTime.Parse(EndDate);
            Guid IdUsers;
            if (!string.IsNullOrEmpty(IdUser))
            {
                IdUsers = Guid.Parse(IdUser);
            }
            else
            {
                IdUsers = iReportRepository.User(User.Identity.Name).UserId;
            }
            List<DailyReport> listTasks = iReportRepository.Daily(dtmInitDate, dtmEndDate, IdUsers);
            
            return PartialView(listTasks);
        }

        [HttpPost]
        public ActionResult ExportData(string InitiaDateExport, string EndDateExport, string IdUserExport)
        {
            GridView gv = new GridView();
            DateTime dtmInitDate = DateTime.Parse(InitiaDateExport);
            DateTime dtmEndDate = DateTime.Parse(EndDateExport);
            Guid IdUsers;

            if (!string.IsNullOrEmpty(IdUserExport))
            {
                IdUsers = Guid.Parse(IdUserExport);
            }
            else
            {
                IdUsers = iReportRepository.User(User.Identity.Name).UserId;
            }
            
            gv.DataSource = iReportRepository.DailyExcel(dtmInitDate, dtmEndDate, IdUsers);
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Marklist.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            
            gv.RenderControl(htw);
            string ss = sw.ToString().Replace("&lt;br /&gt;","<br/>");
            Response.Output.Write(ss);
            Response.Flush();
            Response.End();

            return RedirectToAction("Index");
        }
    }
}
