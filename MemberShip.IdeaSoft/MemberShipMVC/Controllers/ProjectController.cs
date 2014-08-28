using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemberShipMVC.Models;
using MemberShipMVC.Repositores;
using MemberShipMVC.Repositores.Interfaces;

namespace MemberShipMVC.Controllers
{
    public class ProjectController : Controller
    {
        private IProjectRepository _iProjectRepository = null;
        
        public ProjectController()
        {
            this._iProjectRepository = new ProjectRepository();  
        }

        public ProjectController(IProjectRepository iProjectRepository)
        {
            this._iProjectRepository = iProjectRepository;        
        }
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetProjects(int page, int rows, string sidx, string sord)
        {
            List<Project> Projects = new List<Project>();
            Projects = _iProjectRepository.GetAll();
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
                               id = projects.IdProject,
                               cell = new string[] {
                                   projects.IdProject.ToString(), projects.Name.ToString(),  projects.Description.ToString(), projects.Removed.ToString()
                                }
                           }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public void EditProject(Project project)
        {
            if (project.IdProject != Guid.Empty)
            {
                _iProjectRepository.Update(project);
            }
            else {
                _iProjectRepository.Add(project);
            }
        }

        public void DeleteProject(string id)
        {
            Guid Id = Guid.Parse(id);
            _iProjectRepository.Delete(Id);
        }

        [HttpGet]
        public ActionResult ValidationName(string Name)
        {
            string strIsValid = "";
            
            if (!_iProjectRepository.ValidationName(Name))
            {
                strIsValid = "NotValid";
            }

            return Json(strIsValid, JsonRequestBehavior.AllowGet);
        }
    }
}
