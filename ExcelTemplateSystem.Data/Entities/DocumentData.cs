namespace ExcelTemplateSystem.Data.Entities
{
    public class DocumentData
    {
        public int Id { get; set; }
        public int SubmittedDocumentId { get; set; }
        public int TemplateColumnId { get; set; }
        public string Value { get; set; }
        
        public SubmittedDocument SubmittedDocument { get; set; }
        public TemplateColumn TemplateColumn { get; set; }
    }
}
