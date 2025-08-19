using System;

namespace ExcelTemplateSystem.Data.Entities
{
    public class DocumentHistory
    {
        public int Id { get; set; }
        public int SubmittedDocumentId { get; set; }
        public string Action { get; set; } // "Created", "Updated", "Validated", "Rejected"
        public string ActionBy { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; }
        
        public SubmittedDocument SubmittedDocument { get; set; }
    }
}
