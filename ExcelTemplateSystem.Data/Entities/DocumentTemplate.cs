using System;
using System.Collections.Generic;

namespace ExcelTemplateSystem.Data.Entities
{
    public class DocumentTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        
        public ICollection<TemplateColumn> Columns { get; set; }
        public ICollection<SubmittedDocument> SubmittedDocuments { get; set; }
    }
}
