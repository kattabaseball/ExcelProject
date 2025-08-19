namespace ExcelTemplateSystem.Data.Entities
{
    public class TemplateColumn
    {
        public int Id { get; set; }
        public int DocumentTemplateId { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; } // "Text", "Number", "Currency", "Date", "Boolean"
        public bool IsRequired { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public int? MaxLength { get; set; }
        public string DefaultValue { get; set; }
        public string ValidationRegex { get; set; }
        
        public DocumentTemplate DocumentTemplate { get; set; }
        public ICollection<DocumentData> DocumentData { get; set; }
    }
}
