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
    
    public partial class ProjectUser
    {
        public ProjectUser()
        {
            this.Tasks = new HashSet<Task>();
        }
    
        public System.Guid IdUser { get; set; }
        public System.Guid IdProject { get; set; }
        public System.Guid IdProjectUser { get; set; }
        public bool Removed { get; set; }
    
        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}