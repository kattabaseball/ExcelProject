using System;
using System.Collections.Generic;

namespace ExcelTemplateSystem.Data.Entities
{
    public class SubmittedDocument
    {
        public int Id { get; set; }
        public int DocumentTemplateId { get; set; }
        public string UniqueIdentifier { get; set; } = Guid.NewGuid().ToString();
        public string SubmittedBy { get; set; }
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Submitted"; // "Submitted", "Validated", "Rejected"
        
        public DocumentTemplate DocumentTemplate { get; set; }
        public ICollection<DocumentData> Data { get; set; }
        public ICollection<DocumentHistory> History { get; set; }
    }
}
