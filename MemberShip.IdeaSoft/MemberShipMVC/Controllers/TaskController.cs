using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemberShipMVC.Models;
using MemberShipMVC.Models.ViewsModels;
using MemberShipMVC.Repositores;
using System.ComponentModel.DataAnnotations;
using MemberShipMVC.Repositores.Interfaces;
using System.Net.Mail;

namespace MemberShipMVC.Controllers
{
    [Authorize(Roles = "Administrator, Developer, Quality Engineer")]
    public class TaskController : Controller
    {
        private IAdminDayliRepository iAdminDayliRepository = null;
        private IReportRepository iReportRepository = null;
        private ISendMailRepository iSendMailRepository = null;

        public TaskController()
        {
            this.iAdminDayliRepository = new AdminDayliRepository();
            this.iReportRepository = new ReportRepository();
            this.iSendMailRepository = new SendMailRepository();
        }

        public TaskController(IAdminDayliRepository iAdminDayliRepository, IReportRepository iReportRepository, ISendMailRepository iSendMailRepository)
        {
            this.iAdminDayliRepository = iAdminDayliRepository;
            this.iReportRepository = iReportRepository;
            this.iSendMailRepository = iSendMailRepository;
        }
   
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserAndTask()
        {
            var SelectUser = new SelectList(iReportRepository.GetAllsUser(), "UserId", "UserName");
            ViewData["listUsers"] = SelectUser;
            return View();
        }

        public ActionResult GetProjectForUser(string IdUser)
        {
            Guid UserId = Guid.Parse(IdUser);
            ViewBag.ProjectList = iAdminDayliRepository.GetAllPrjectForUser(UserId,null);
            return PartialView();
        }

        [HttpPost]
        public ActionResult SaveProjectForUser(FormCollection form)
        {
            string strSave = "Not";
            string strSelectedProjects = form["Projects"];
            string strUser = form["UserId"];
            Guid UserId = Guid.Parse(strUser);

            if (iAdminDayliRepository.AddProjectsForUser(strSelectedProjects, UserId))
            {
                strSave = "Yes";
            }

            ViewBag.ProjectList = iAdminDayliRepository.GetAllPrjectForUser(UserId, strSelectedProjects.Split(','));
            return Json(strSave, JsonRequestBehavior.AllowGet);
        }

        public void DeleteProject(string id)
        {
            Guid idProject = Guid.Parse(id);
            iAdminDayliRepository.RemoveProject(idProject);
        }

        [HttpGet]
        public ActionResult GetProjets(int page, int rows, string sidx, string sord, string user)
        {
            Guid UserId = Guid.Parse(user);
            List<ProjectUser> Projects = new List<ProjectUser>();
            Projects = iAdminDayliRepository.GetProjectWithUser(UserId);
            int intCount = Projects.Count();
            
            var jsonData = new
            {
                total = 1,
                page = page,
                records = intCount,
                rows = (
                           from projects in Projects
                           select new
                           {
                               id = projects.IdProjectUser,
                               cell = new string[] {
                                   projects.IdProjectUser.ToString(), projects.Project.Name.ToString(), projects.Project.Description.ToString()
                                }
                           }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTasks(int page, int rows, string sidx, string sord)
        {
            string strUserName = User.Identity.Name;
            List<Task> Tasks = new List<Task>();
            Tasks = iAdminDayliRepository.GetAll(strUserName);
            int intCount = Tasks.Count();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = intCount,
                rows = (
                           from tasks in Tasks
                           select new
                           {
                               id = tasks.IdTask,
                               cell = new string[] {
                                   tasks.IdTask.ToString(), tasks.Send.ToString(),  tasks.ProjectUser.Project.Name.ToString(),tasks.Date.ToString("yyyy-MM-dd"),
                                   tasks.IdTicket.ToString(), tasks.Detail.ToString(), tasks.Hours.ToString()
                                }
                               //cell = new string[] {
                               //    tasks.IdTask.ToString(),  tasks.ProjectUser.Project.Name.ToString(),tasks.Date.ToString("yyyy-MM-dd"),
                               //    tasks.IdTicket.ToString(), tasks.Detail.ToString(), tasks.Hours.ToString()
                               // }
                          }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ValidationHoursAndDate(string Date, string IdProject, string Hours)
        { 
            string strIsValid  = "";
            string strUserName = User.Identity.Name;
            DateTime dtmDate = DateTime.Parse(Date);
            Guid guidProject = Guid.Parse(IdProject);
            decimal decHous = decimal.Parse(Hours);

            if (iAdminDayliRepository.IsValidDate(dtmDate))
            {
                strIsValid = "NotValidDateOfSend";

                if (iAdminDayliRepository.IsValidDateOfSend(dtmDate, User.Identity.Name.ToString()))
                {
                    strIsValid = "NoValidHours";
                    
                    if (iAdminDayliRepository.IsValidHours(dtmDate, guidProject, decHous, strUserName))
                    {
                        strIsValid = "IsValid";
                    }
                }
            }

            return Json(strIsValid, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ValidationProjectAndTask(string IdProject)
        {
            string strIsValid = "";
            Guid projectId = Guid.Parse(IdProject);
         
            if(iAdminDayliRepository.IsValidDeleteProjerForUser(projectId))
            {
                strIsValid="IsValid";
            }

            return Json(strIsValid,JsonRequestBehavior.AllowGet);
        }


        public void EditTask (Task task)
        {
            Guid idEmpy = Guid.Empty;

            if (task.IdTask != idEmpy)
            {
                iAdminDayliRepository.Update(task);
            }
            else
            {
                iAdminDayliRepository.Add(task);
            }
        }

        public void Delete(string id)
        { 
            Guid idTask = Guid.Parse(id);
            iAdminDayliRepository.Remove(idTask);
        }

        public string GetProject(string Date)
        {
            string strUserName = User.Identity.Name;
            List<ProjectUser> listProjects = iAdminDayliRepository.GetProjectsForUserCreate(strUserName);
            var data = "<select>";

            foreach (var project in listProjects)
            {
                data += "<option value='" + project.IdProjectUser + "'>" + project.Project.Name + "</option>";
            }

            data += "</select>";

            return data;
        }

        [HttpPost]
        public ActionResult SenMail(string Date)
        {
            string strUserName = User.Identity.Name;
            DateTime dtmDate = DateTime.Parse(Date);
            Mail objMail = iSendMailRepository.GetMail(dtmDate, strUserName);
           
            MailMessage mail = new MailMessage();
            mail.To.Add(objMail.To);
            mail.To.Add(objMail.ToBack);
            mail.From = new MailAddress(objMail.From);
            mail.Subject = objMail.Subject;
            mail.Body = objMail.Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            

            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;


            smtp.UseDefaultCredentials = false;
            
            smtp.Credentials = new System.Net.NetworkCredential(objMail.From, objMail.Password);
            smtp.EnableSsl = true;
            
            smtp.Send(mail);
          
            
            return Json(new { Date = "True" });
        }

    }
}