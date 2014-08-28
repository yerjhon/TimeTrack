namespace MemberShipMVC.Repositores
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using MemberShipMVC.Repositores.Interfaces;
    using MemberShipMVC.Models;
    using MemberShipMVC.Models.ViewsModels;

    public class ReportRepository : IReportRepository
    {
        MemberMVCEntities db = null;

        public ReportRepository()
        {
            this.db = new MemberMVCEntities();
        }

        #region Report

        public void Dailty()
        {
            throw new NotImplementedException();
        }

        public List<DailyReport> Daily(DateTime InitiaDate, DateTime EndDate, Guid idUser)
        {
            List<Task> listTasks = new List<Task>();
            List<DailyReport> listDaylis = new List<DailyReport>();

            //listTask =  (from prus in db.ProjectUsers
            //            join pr in db.Projects on prus.IdProject equals pr.IdProject
            //            join ta in db.Tasks on pr.IdProject equals ta.IdProject
            //            where ta.Date >= InitiaDate && ta.Date <= EndDate && prus.IdUser == idUser
            //            select ta).ToList();

            listTasks = (from prus in db.ProjectUsers
                        join pr in db.Projects on prus.IdProject equals pr.IdProject
                        join ta in db.Tasks on prus.IdProjectUser equals ta.IdProjectUser
                        where ta.Date >= InitiaDate && ta.Date <= EndDate && prus.IdUser == idUser
                        select ta).ToList();

            var query = (from q in listTasks
                         group q by new { q.Date } into g
                         select new
                         {
                             Date = g.Key.Date,
                             Ticket = string.Join(",", g.Select(ti => ti.IdTicket)),
                             Detail = string.Join(",", g.Select(de => de.Detail)),
                             Hours = g.Sum(t => t.Hours)
                         }).ToList();

            foreach (var item in query)
            {
                DailyReport objDaily = new DailyReport();

                string[] strTicket = item.Ticket.Split(new Char[] { ',' });
                string[] strDetail = item.Detail.Split(new Char[] { ',' });

                objDaily.Date = item.Date;
                objDaily.Details = new List<string>();

                for (int i = 0; i < strTicket.Count(); i++)
                {
                    objDaily.Details.Add(strTicket[i] + " - " + strDetail[i]);
                }

                objDaily.Hours = item.Hours;
                listDaylis.Add(objDaily);
            }

            return listDaylis.OrderBy(t => t.Date).ToList();
        }

        public List<DailyReportExcel> DailyExcel(DateTime InitiaDate, DateTime EndDate, Guid idUser)
        {
            List<Task> listTasks = new List<Task>();
            List<DailyReportExcel> listDaylis = new List<DailyReportExcel>();

            listTasks = (from prus in db.ProjectUsers
                        join pr in db.Projects on prus.IdProject equals pr.IdProject
                        join ta in db.Tasks on prus.IdProjectUser equals ta.IdProjectUser
                        where ta.Date >= InitiaDate && ta.Date <= EndDate && prus.IdUser == idUser
                        select ta).ToList();

            var query = (from q in listTasks
                         group q by new { q.Date } into g
                         select new
                         {
                             Date = g.Key.Date,
                             Ticket = string.Join(",", g.Select(ti => ti.IdTicket)),
                             Detail = string.Join(",", g.Select(de => de.Detail)),
                             Hours = g.Sum(t => t.Hours)
                         }).ToList();

            foreach (var item in query)
            {
                DailyReportExcel objDaily = new DailyReportExcel();

                string[] strTicket = item.Ticket.Split(new Char[] { ',' });
                string[] strDetail = item.Detail.Split(new Char[] { ',' });
                string detail = "";

                objDaily.Date = item.Date;

                for (int i = 0; i < strTicket.Count(); i++)
                {
                    detail += (strTicket[i] + " - " + strDetail[i]) + "<br />";
                }

                objDaily.Details = detail;
                objDaily.Hours = item.Hours;
                listDaylis.Add(objDaily);
            }

            return listDaylis.OrderBy(t => t.Date).ToList();
        }

        public List<User> GetAllsUser()
        {
            List<User> listUsers = new List<User>();
            listUsers = db.Users.ToList();

            return listUsers;
        }

        public bool IsAdministrator(string UserName)
        {
            bool blnIsAdministrator = false;
            
            User objUser = db.Users.FirstOrDefault(u => u.UserName== UserName);

            if(objUser.Roles.FirstOrDefault(r=>r.RoleName == "Administrator") != null)
            {
                blnIsAdministrator = true;
            }

            return blnIsAdministrator;
        }

        public User User(string UserName)
        {
            User ObjUser = db.Users.FirstOrDefault(us => us.UserName == UserName);
            return ObjUser;
        }
        
        #endregion

    }
}