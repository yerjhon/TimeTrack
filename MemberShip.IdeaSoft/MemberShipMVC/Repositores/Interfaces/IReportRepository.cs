namespace MemberShipMVC.Repositores.Interfaces
{
    using System;
    using MemberShipMVC.Models;
    using System.Collections.Generic;
    using MemberShipMVC.Models.ViewsModels;

    public interface IReportRepository
    {
        void Dailty();

        List<DailyReport> Daily(DateTime InitialDate, DateTime EndDate, Guid idUser);

        List<User> GetAllsUser();

        List<DailyReportExcel> DailyExcel(DateTime InitiaDate, DateTime EndDate, Guid idUser);

        bool IsAdministrator(string UserName);

        User User(string UserName);
    }
}
