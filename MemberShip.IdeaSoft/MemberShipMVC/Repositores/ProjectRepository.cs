using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemberShipMVC.Models;
using MemberShipMVC.Repositores.Interfaces;
using System.Data.Entity;

namespace MemberShipMVC.Repositores
{
    public class ProjectRepository : IProjectRepository
    {
        private MemberMVCEntities db = null;
        
        public ProjectRepository()
        {
            db = new MemberMVCEntities();
        }

        #region CrudProject
        
        public List<Project> GetAll()
        {
            List<Project> listProjects = db.Projects.Where(p => p.Removed == false).ToList();
            return listProjects;
        }

        public void Add(Project Item)
        {
            Item.IdProject = Guid.NewGuid();
            db.Projects.Add(Item);
            db.SaveChanges();
        }

        public void Update(Project Item)
        {
            db.Entry(Item).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Delete(Guid Id)
        {
            Project objProject = db.Projects.FirstOrDefault(pr => pr.IdProject == Id);
            if(objProject != null)
            {
                objProject.Removed = true;
                db.Entry(objProject).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
     
        #endregion

        #region ValidationProject

        public bool ValidationName(string Name)
        {
            bool blnIsValid = true;
            Project objProject = db.Projects.FirstOrDefault(pr => pr.Name.ToUpper() == Name.ToUpper());
            
            if (objProject != null)
            {
                blnIsValid = false;
            }

            return blnIsValid;
        }

        #endregion

    }
}