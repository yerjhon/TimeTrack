namespace MemberShipMVC.Repositores
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Data.Entity;
    using System.Collections.Generic;
    using MemberShipMVC.Models;

    public class AdminDayliRepository : IAdminDayliRepository
    {
        private MemberMVCEntities db = null;

        public AdminDayliRepository()
        {
            this.db = new MemberMVCEntities();
        }

        #region CrudTask
        
        public List<Task> GetAll(string UserName)
        {
            List<Task> listTaks = new List<Task>();

            listTaks = (from prus in db.ProjectUsers
                        join ta in db.Tasks on prus.IdProjectUser equals ta.IdProjectUser
                        where prus.User.UserName == UserName
                        select ta).ToList();

            listTaks = listTaks.OrderBy(t => t.Date).ToList();

            return listTaks;
        }

        public void Add(Task Item)
        {
            MemberMVCEntities db1 = new MemberMVCEntities();
            Item.IdTask = Guid.NewGuid();
            if (Item.IdTicket == null)
            {
                Item.IdTicket = "No ticket";
            }

            db1.Tasks.Add(Item);
            db1.SaveChanges();
        }

        public void Remove(Guid Id)
        {
            Task objTask = db.Tasks.Find(Id);
            db.Tasks.Remove(objTask);
            db.SaveChanges();
        }

        public void Update(Task Item)
        {
            db.Entry(Item).State = EntityState.Modified;
            db.SaveChanges();
        }

        #endregion

        #region AssignProjectsToUsers

        public bool AddProjectsForUser(string projects, Guid user)
        {
            bool blnSave = true;
            string[] splitProject = projects.Split(new Char[] { ',' });

            for (int i = 0; i < splitProject.Count(); i++)
            {
                Guid IdProject = Guid.Parse(splitProject[i]);
                ProjectUser objProjectUser = db.ProjectUsers.FirstOrDefault(p => p.IdUser == user && p.IdProject == IdProject);
                if (objProjectUser == null)
                {
                    ProjectUser objProjecUser = new ProjectUser();
                    objProjecUser.IdProjectUser = Guid.NewGuid();
                    objProjecUser.IdProject = IdProject;
                    objProjecUser.IdUser = user;
                    db.ProjectUsers.Add(objProjecUser);
                }
                else
                {
                    objProjectUser.Removed  = false;
                    db.Entry(objProjectUser).State = EntityState.Modified;
                }
            }

            db.SaveChanges();
            return blnSave;
        }

        public void RemoveProject(Guid Id)
        {
            ProjectUser objProject = db.ProjectUsers.Find(Id);
            objProject.Removed = true;
            db.Entry(objProject).State = EntityState.Modified;
            db.SaveChanges();
        }

        public MultiSelectList GetAllPrjectForUser(Guid User, string[] SelectdValues)
        {
            List<Project> listProjects = new List<Project>();
            listProjects = db.Projects.Where(pr => pr.ProjectUsers.FirstOrDefault(p => p.IdUser == User && p.Removed != true).IdUser != User && pr.Removed == false).ToList();
            return new MultiSelectList(listProjects, "IdProject", "Name", SelectdValues);
        }

        public List<ProjectUser> GetProjectWithUser(Guid User)
        {
            List<ProjectUser> listProjects = new List<ProjectUser>();
            listProjects = (from prus in db.ProjectUsers
                            join us in db.Users on prus.IdUser equals us.UserId
                            where us.UserId == User && prus.Removed == false
                            select prus).ToList();

            return listProjects;
        }

        #endregion

        #region Validations
        
        public bool IsValidDeleteProjerForUser(Guid projectId)
        {
            bool blnIsValid = true;
            
            if (db.Tasks.FirstOrDefault(t => t.IdProjectUser == projectId) != null)
            {
                blnIsValid = false;
            }
        
            return blnIsValid;
        }

        public bool IsValidHours(DateTime Date, Guid IdProject, decimal Hours, string UserName)
        {
            List<Task> listTasks = new List<Task>();
            decimal decCantHours;
            bool blnIsValid = true;

            //listTask =  (from prus in db.ProjectUsers
            //             join pr in db.Projects on prus.IdProject equals pr.IdProject
            //             join ta in db.Tasks on pr.IdProject equals ta.IdProject
            //             where ta.Date == Date && pr.IdProject == IdProject && prus.User.UserName == UserName
            //             select ta).ToList();

            listTasks = (from prus in db.ProjectUsers
                         join pr in db.Projects on prus.IdProject equals pr.IdProject
                         join ta in db.Tasks on prus.IdProjectUser equals ta.IdProjectUser
                         where ta.Date == Date && prus.User.UserName == UserName
                         select ta).ToList();

            decCantHours = listTasks.Sum(t => t.Hours) + Hours;

            if (decCantHours > 24)
            {
                blnIsValid = false;
            }

            return blnIsValid;
        }

        public bool IsValidDate(DateTime Date)
        {
            bool blnIsValid = false;

            int intDayIntial = (int)DateTime.Now.DayOfWeek;
            int intDayEnd = (((int)DateTime.Now.DayOfWeek) - 1) * (-1) - 7;
            int intDayAfterEnd = (((int)DateTime.Now.DayOfWeek) - 5) * (-1);

            DateTime dtmBeforeDayWeek = intDayIntial <= 5 && intDayIntial != 0 ? DateTime.Today.AddDays(intDayEnd) : intDayIntial == 6 ? DateTime.Today.AddDays(-12) : DateTime.Today.AddDays(-13);
            DateTime dtmAfterDayWeek = intDayIntial <= 5 && intDayIntial != 0 ? DateTime.Today.AddDays(intDayAfterEnd) : DateTime.Today;

            if (Date <= dtmAfterDayWeek && Date >= dtmBeforeDayWeek)
            {
                blnIsValid = true;
            }

            return blnIsValid;
        }

        public bool IsValidDateOfSend(DateTime Date, string UserName)
        {
            bool blnIsValid = true;
            Task objTask = db.Tasks.FirstOrDefault(ta => ta.Date == Date && ta.Send == true && ta.ProjectUser.User.UserName == UserName);
            
            if (objTask != null) {
                blnIsValid = false;
            }

            return blnIsValid;
        }

        #endregion

        public List<ProjectUser> GetProjectsForUserCreate(string UserName)
        {
            List<ProjectUser> listProject = (from pr in db.Projects
                                         join prus in db.ProjectUsers on pr.IdProject equals prus.IdProject
                                         where prus.User.UserName == UserName && prus.Removed == false
                                         select prus).ToList();

            return listProject;
        }

    }
}