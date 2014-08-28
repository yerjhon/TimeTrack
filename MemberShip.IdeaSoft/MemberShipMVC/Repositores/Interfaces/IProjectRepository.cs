namespace MemberShipMVC.Repositores.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MemberShipMVC.Models;

    public interface IProjectRepository
    {
        List<Project> GetAll();

        void Add(Project Item);

        void Update(Project Item);

        void Delete(Guid Id);

        bool ValidationName(string Name);
    }
}
