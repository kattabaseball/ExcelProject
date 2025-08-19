using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelTemplateSystem.Business.Models
{
    public class DocumentTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        
        public List<TemplateColumnDto> Columns { get; set; }
    }

    public class TemplateColumnDto
    {
        public int Id { get; set; }
        public int DocumentTemplateId { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool IsRequired { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public int? MaxLength { get; set; }
        public string DefaultValue { get; set; }
        public string ValidationRegex { get; set; }
    }

    public class SubmittedDocumentDto
    {
        public int Id { get; set; }
        public int DocumentTemplateId { get; set; }
        public string UniqueIdentifier { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string Status { get; set; }
        
        public List<DocumentDataDto> Data { get; set; }
        public List<DocumentHistoryDto> History { get; set; }
    }

    public class DocumentDataDto
    {
        public int TemplateColumnId { get; set; }
        public string Value { get; set; }
    }

    public class DocumentHistoryDto
    {
        public string Action { get; set; }
        public string ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public string Notes { get; set; }
    }

    public class ValidationError
    {
        public string ColumnName { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class DocumentSubmissionResult
    {
        public bool IsValid { get; set; }
        public string UniqueIdentifier { get; set; }
        public List<ValidationError> Errors { get; set; } = new List<ValidationError>();
    }
}
