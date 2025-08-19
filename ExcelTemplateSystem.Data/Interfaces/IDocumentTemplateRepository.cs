using ExcelTemplateSystem.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExcelTemplateSystem.Data.Interfaces
{
    public interface IDocumentTemplateRepository
    {
        Task<DocumentTemplate> GetTemplateById(int id);
        Task<List<DocumentTemplate>> GetAllTemplates();
        Task<DocumentTemplate> CreateTemplate(DocumentTemplate template);
        Task<DocumentTemplate> UpdateTemplate(DocumentTemplate template);
        Task<bool> DeleteTemplate(int id);
        
        Task<TemplateColumn> AddColumnToTemplate(TemplateColumn column);
        Task<bool> RemoveColumnFromTemplate(int columnId);
        
        Task<SubmittedDocument> SubmitDocument(SubmittedDocument document);
        Task<SubmittedDocument> GetSubmittedDocument(string uniqueIdentifier);
        Task<List<SubmittedDocument>> GetSubmittedDocumentsByTemplate(int templateId);
    }
}
