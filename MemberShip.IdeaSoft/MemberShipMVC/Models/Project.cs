//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MemberShipMVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Project
    {
        public Project()
        {
            this.ProjectUsers = new HashSet<ProjectUser>();
        }
    
        public System.Guid IdProject { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Removed { get; set; }
    
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
    }
}
