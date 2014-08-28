namespace MemberShipMVC.Repositores
{
   using System;
   using System.Web.Mvc;
   using System.Collections.Generic;
   using MemberShipMVC.Models;

   public interface IAdminDayliRepository
   {
       List<Task> GetAll(string UserName);

       void Add(Task Item);

       void Remove(Guid Id);

       void Update(Task Item);

       bool AddProjectsForUser(string projects, Guid user);

       void RemoveProject(Guid Id);

       MultiSelectList GetAllPrjectForUser(Guid User, string[] SelectdValues);

       List<ProjectUser> GetProjectWithUser(Guid User);

       List<ProjectUser> GetProjectsForUserCreate(string UserName);

       bool IsValidDeleteProjerForUser(Guid projectId);

       bool IsValidHours(DateTime Date, Guid IdProject, decimal Hours, string UserName);
       
       bool IsValidDate(DateTime Date);

       bool IsValidDateOfSend(DateTime Date, string UserName);

    }
}
