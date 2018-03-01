//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Detectives2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Nullable<int> Year { get; set; }
        public string Plot { get; set; }
        public string Spoilers { get; set; }
        public string Image { get; set; }
        public Nullable<int> AuthorId { get; set; }
        public Nullable<int> DetectiveId { get; set; }
        public Nullable<int> SortOrder { get; set; }
    
        public virtual Author Author { get; set; }
        public virtual Detective Detective { get; set; }
    }
}
