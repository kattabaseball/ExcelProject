using ExcelTemplateSystem.Business.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ExcelTemplateSystem.Business.Interfaces
{
    public interface IDocumentTemplateService
    {
        Task<DocumentTemplateDto> GetTemplateById(int id);
        Task<List<DocumentTemplateDto>> GetAllTemplates();
        Task<DocumentTemplateDto> CreateTemplate(DocumentTemplateDto templateDto);
        Task<DocumentTemplateDto> UpdateTemplate(DocumentTemplateDto templateDto);
        Task<bool> DeleteTemplate(int id);
        
        Task<TemplateColumnDto> AddColumnToTemplate(TemplateColumnDto columnDto);
        Task<bool> RemoveColumnFromTemplate(int columnId);
        
        Task<Stream> GenerateExcelTemplate(int templateId);
        Task<DocumentSubmissionResult> SubmitDocumentData(int templateId, Stream excelFile, string submittedBy);
        Task<SubmittedDocumentDto> GetSubmittedDocument(string uniqueIdentifier);
        Task<List<SubmittedDocumentDto>> GetSubmittedDocumentsByTemplate(int templateId);
    }
}
