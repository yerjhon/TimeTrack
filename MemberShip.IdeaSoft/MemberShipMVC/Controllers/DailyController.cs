using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using MemberShipMVC.Repositores;
using MemberShipMVC.Models;

namespace MemberShipMVC.Controllers
{
    public class DailyController : ApiController
    {
        static readonly IAdminDayliRepository iAdminDayliRepository = new IAdminDayliRepository();

        
        
        
        public dynamic GetTasks(string sidx, string sord, int page, int rows)
        {
            var tasks = iAdminDayliRepository.GetAll() as IEnumerable<Task>;
            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            var totalRecords = tasks.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            tasks = tasks.Skip(pageIndex * pageSize).Take(pageSize);
            return new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from task in tasks
                    select new 
                    {
                        i = task.IdTask.ToString();
                        cell = new string [] {
                           task.IdTask.ToString(),
                           task.Date.ToString(),
                           task.IdProject.ToString(),
                           //task.Project.Name,
                           task.IdTicket,
                           task.Detail,
                        }
                    }).ToArray();
                )
            };
        
        }
        
        public HttpResponseMessage PostTask(Task item)
        {
            item = iAdminDayliRepository.Add(item);
            var response = Request.CreateResponse<Task>(HttpStatusCode.Created, item);
            string uri = Url.Link("DefaultApi", new { id = item.IdTask });
            response.Headers.Location = new Uri(uri);
            return response;
        }

    }
}
