namespace MemberShipMVC.Repositores.Interfaces
{
    using System;
    using MemberShipMVC.Models.ViewsModels;

    public interface ISendMailRepository
    {
        Mail GetMail(DateTime Date, string UserName);

        void UpdateTaskFormail(Guid IdTask);
    }
}

