//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hearts4Kids.Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class QA
    {
        public int Id { get; set; }
        public string AskerName { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public Nullable<int> AnswerUserId { get; set; }
        public bool Approved { get; set; }
    
        public virtual UserBio UserBio { get; set; }
    }
}
