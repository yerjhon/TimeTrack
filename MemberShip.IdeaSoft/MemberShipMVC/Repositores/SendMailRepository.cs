namespace MemberShipMVC.Repositores
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Data.Entity;
    using MemberShipMVC.Models;
    using MemberShipMVC.Models.ViewsModels;
    using MemberShipMVC.Repositores.Interfaces;
    using SecurityGuard.Interfaces;
    using SecurityGuard.Services;

    public class SendMailRepository : ISendMailRepository
    {
        private IMembershipService _membershipService;
        private IAdminDayliRepository _iAdminDayliRepository;
        private MemberMVCEntities db = null;

        public SendMailRepository()
        {
            this._membershipService = new MembershipService(System.Web.Security.Membership.Provider);
            this._iAdminDayliRepository = new AdminDayliRepository();
            this.db = new MemberMVCEntities();
        }

        #region SendMail

        public Mail GetMail(DateTime Date, string UserName)
        {
            string strMailUser = db.Memberships.FirstOrDefault(m => m.User.UserName == UserName).Email; 
            int intDay = ((int)Date.DayOfWeek - 1) * (-1);
            DateTime dtmFirstDay = Date.AddDays(intDay);
            Mail objMail = new Mail();
            DateTime dtmDateInitialDaily;
            string strProjectForDaily;

            string strTo = "wilfredo.vargas@ideasoftinc.com";
            string strFrom = "reports.ideasoft@gmail.com";
            string strPassword = "Ideasoft2014";


            //string strTo = "reports.ideasoft@gmail.com";
            //string strFrom = "reports.ideasoft@gmail.com";
            //string strPassword = "Ideasoft2014";


            string strSubject = dtmFirstDay.ToString("d") + " - " + Date.ToString("d") + " - " + strMailUser;
            string strBodyProjects = "<b> Project  : </b>";
            string strBody = "<br /><table style ='border:black 1px solid;'><tr><th style='width:400px; border:black 1px solid; text-align:center;'>Task</th> <th style='width:400px; border:black 1px solid; text-align:center;' >Monday</th> <th style='width:400px; border:black 1px solid; text-align:center;'>Tuesday</th> <th style='width:400px; border:black 1px solid; text-align:center;'>Wednesday</th> <th style='width:400px; border:black 1px solid; text-align:center;'>Thursday</th> <th style='width:400px; border:black 1px solid; text-align:center;'>Friday</th> </tr>";
            string strBodyDetails = "<table>";

            //var query = (from t in _iAdminDayliRepository.GetAll(UserName)
            //             where t.Date >= dtmFirstDay && t.Date <= Date
            //             select new
            //             {
            //                 IdTask = t.IdTask,
            //                 Date = t.Date,
            //                 Ticket = t.IdTicket,
            //                 Monday = (int)t.Date.DayOfWeek == 1 ? t.Hours.ToString() : "",
            //                 Tuesday = (int)t.Date.DayOfWeek == 2 ? t.Hours.ToString() : "",
            //                 Wednesday = (int)t.Date.DayOfWeek == 3 ? t.Hours.ToString() : "",
            //                 Thursday = (int)t.Date.DayOfWeek == 4 ? t.Hours.ToString() : "",
            //                 Friday = (int)t.Date.DayOfWeek == 5 ? t.Hours.ToString() : "",
            //                 Detail = t.Detail.ToString()

            //             }).ToList();

            var queryDayliDetail = (from t in _iAdminDayliRepository.GetAll(UserName)
                                    where t.Date >= dtmFirstDay && t.Date <= Date
                                    group t by new { t.Date } into g
                                    select new
                                    {
                                        Date = g.Key.Date,
                                        IdTicket = string.Join(",",g.Select(id=>id.IdTicket)),
                                        IdTask = string.Join(",", g.Select(id => id.IdTask)),
                                        Detail = string.Join(",", g.Select(de => de.Detail)),
                                        Project = g.FirstOrDefault().ProjectUser.Project.Name,
                                        Hours = g.Sum(t => t.Hours)
                                    
                                    }).ToList();

            var queryDayliHours = (from t in _iAdminDayliRepository.GetAll(UserName)
                          where t.Date >= dtmFirstDay && t.Date <= Date
                          group t by new { t.IdTicket } into g 
                          select new
                          {
                              Ticket = g.Key.IdTicket,
                              Monday = g.FirstOrDefault(t => t.Date.DayOfWeek.ToString() == "Monday") != null ? g.FirstOrDefault(t => t.Date.DayOfWeek.ToString() == "Monday").Hours.ToString() : "" ,
                              Tuesday =  g.FirstOrDefault(t => t.Date.DayOfWeek.ToString() == "Tuesday") != null ? g.FirstOrDefault(t => t.Date.DayOfWeek.ToString() == "Tuesday").Hours.ToString() : "",
                              Wednesday =  g.FirstOrDefault(t => t.Date.DayOfWeek.ToString() == "Wednesday")!= null ? g.FirstOrDefault(t => t.Date.DayOfWeek.ToString() == "Wednesday").Hours.ToString() : "",
                              Thursday = g.FirstOrDefault(t => t.Date.DayOfWeek.ToString() == "Thursday") != null ? g.FirstOrDefault(t => t.Date.DayOfWeek.ToString() == "Thursday").Hours.ToString() : "",
                              Friday = g.FirstOrDefault(t => t.Date.DayOfWeek.ToString() == "Friday") != null ? g.FirstOrDefault(t => t.Date.DayOfWeek.ToString() == "Friday").Hours.ToString() : "",
                          }).ToList();

            //dtmDateInitialDayli = query.FirstOrDefault().Date;
            //strBodyDetails += "<span>" + dtmDateInitialDayli.ToString("dddd") + "</span> <br /> <br />";
            
            dtmDateInitialDaily = queryDayliDetail.FirstOrDefault().Date;
            strBodyDetails += "<tr><td colspan='2'><span><b>" + dtmDateInitialDaily.ToString("dddd") + "</b></span></td></tr><br/>";
            strProjectForDaily = queryDayliDetail.FirstOrDefault().Project;
            strBodyProjects += strProjectForDaily.ToString() + "<br />";
            Dictionary<string,decimal> TotalHours = new Dictionary<string,decimal>();

            foreach (var item in queryDayliHours)
            {
                strBody += "<tr><td style='width:400px; border:black 1px solid; text-align:center;'>" + item.Ticket + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + item.Monday + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + item.Tuesday + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + item.Wednesday + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + item.Thursday + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + item.Friday + "</td></tr>";
            }

            foreach (var item in queryDayliDetail)
            {
                
                if (item.Date != dtmDateInitialDaily)
                {
                    strBodyDetails += "<br /><br/> <tr><td colspan='2'><span><b>" + item.Date.ToString("dddd") + "</b></span> </td></tr>" + "<br/>";
                }
                if (item.Project != strProjectForDaily)
                {
                    strProjectForDaily += item.Project + "<br />";
                }
                dtmDateInitialDaily = item.Date;
                strProjectForDaily = item.Project.ToString();
                string[] splitIdTask = item.IdTask.Split(new Char[] { ',' });
                string[] splitDetail = item.Detail.Split(new Char[] { ',' });
                string[] splitIdTicket = item.IdTicket.Split(new Char[] { ',' });

                for (int i = 0; i < splitIdTask.Count(); i++)
                {
                    UpdateTaskFormail(Guid.Parse(splitIdTask[i]));
                    strBodyDetails += "<tr><td><span><b>" + splitIdTicket[i].ToUpper() + ": " + "</b></span></td> <td>" + splitDetail[i] + "</td></tr>";
                }

                TotalHours.Add(item.Date.DayOfWeek.ToString(),item.Hours);
            }

            strBody += "<tr><td style='width:400px; border:black 1px solid; text-align:center;'>" + "<b> TOTAL </b>" + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + TotalHours.FirstOrDefault(t=>t.Key=="Monday").Value.ToString() + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + TotalHours.FirstOrDefault(t=>t.Key=="Tuesday").Value.ToString() + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + TotalHours.FirstOrDefault(t=>t.Key == "Wednesday").Value.ToString() + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + TotalHours.FirstOrDefault(t=>t.Key=="Thursday").Value.ToString() + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + TotalHours.FirstOrDefault(t=>t.Key=="Friday").Value.ToString() + "</td></tr>";

            //foreach (var item in query)
            //{
            //    strBody += "<tr><td style='width:400px; border:black 1px solid; text-align:center;'>" + item.Ticket + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + item.Monday + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + item.Tuesday + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + item.Wednesday + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + item.Thursday + "</td><td style='width:400px; border:black 1px solid; text-align:center;'>" + item.Friday + "</td></tr>";

            //    if (item.Date != dtmDateInitialDayli)
            //    {
            //        strBodyDetails += "<br /><br/> <span>" + item.Date.ToString("dddd") + "</span>" + "<br /> <br />";
            //    }

            //    strBodyDetails += "<span>" + item.Ticket + ":  " + "</span>" + item.Detail + "<br/>";
            //    dtmDateInitialDayli = item.Date;
            //    UpdateTaskFormail(item.IdTask);
            //}

            strBody += "</table> <br/> <br/>";
            strBody += strBodyDetails;
            objMail.ToBack = strMailUser;
            objMail.To = strTo;
            objMail.From = strFrom;
            objMail.Password = strPassword;
            objMail.Subject = strSubject;
            objMail.Body = strBodyProjects + strBody;

            return objMail;
        }

        public void UpdateTaskFormail(Guid IdTask)
        {
            Task objTask = db.Tasks.FirstOrDefault(t => t.IdTask == IdTask);

            if (objTask.Send == false)
            {
                objTask.Send = true;
                db.Entry(objTask).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        #endregion
    }
}