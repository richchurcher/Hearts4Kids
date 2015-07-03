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
    
    public partial class UserBio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserBio()
        {
            this.Quotes = new HashSet<Quote>();
            this.QAs = new HashSet<QA>();
        }
    
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DomainConstants.Teams Team { get; set; }
        public DomainConstants.Professions Profession { get; set; }
        public string BioPicUrl { get; set; }
        public bool MainTeamPage { get; set; }
        public string Bio { get; set; }
        public bool Trustee { get; set; }
        public string CitationDescription { get; set; }
        public bool Approved { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quote> Quotes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QA> QAs { get; set; }
    }
}