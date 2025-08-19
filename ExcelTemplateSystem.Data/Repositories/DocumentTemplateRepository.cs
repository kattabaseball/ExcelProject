using ExcelTemplateSystem.Data.Context;
using ExcelTemplateSystem.Data.Entities;
using ExcelTemplateSystem.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelTemplateSystem.Data.Repositories
{
    public class DocumentTemplateRepository : IDocumentTemplateRepository
    {
        private readonly ExcelTemplateSystemContext _context;

        public DocumentTemplateRepository(ExcelTemplateSystemContext context)
        {
            _context = context;
        }

        public async Task<DocumentTemplate> GetTemplateById(int id)
        {
            return await _context.DocumentTemplates
                .Include(dt => dt.Columns)
                .FirstOrDefaultAsync(dt => dt.Id == id);
        }

        public async Task<List<DocumentTemplate>> GetAllTemplates()
        {
            return await _context.DocumentTemplates
                .Include(dt => dt.Columns)
                .ToListAsync();
        }

        public async Task<DocumentTemplate> CreateTemplate(DocumentTemplate template)
        {
            _context.DocumentTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task<DocumentTemplate> UpdateTemplate(DocumentTemplate template)
        {
            _context.DocumentTemplates.Update(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task<bool> DeleteTemplate(int id)
        {
            var template = await _context.DocumentTemplates.FindAsync(id);
            if (template == null) return false;

            _context.DocumentTemplates.Remove(template);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TemplateColumn> AddColumnToTemplate(TemplateColumn column)
        {
            _context.TemplateColumns.Add(column);
            await _context.SaveChangesAsync();
            return column;
        }

        public async Task<bool> RemoveColumnFromTemplate(int columnId)
        {
            var column = await _context.TemplateColumns.FindAsync(columnId);
            if (column == null) return false;

            _context.TemplateColumns.Remove(column);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SubmittedDocument> SubmitDocument(SubmittedDocument document)
        {
            _context.SubmittedDocuments.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<SubmittedDocument> GetSubmittedDocument(string uniqueIdentifier)
        {
            return await _context.SubmittedDocuments
                .Include(sd => sd.DocumentTemplate)
                .ThenInclude(dt => dt.Columns)
                .Include(sd => sd.Data)
                .ThenInclude(dd => dd.TemplateColumn)
                .Include(sd => sd.History)
                .FirstOrDefaultAsync(sd => sd.UniqueIdentifier == uniqueIdentifier);
        }

        public async Task<List<SubmittedDocument>> GetSubmittedDocumentsByTemplate(int templateId)
        {
            return await _context.SubmittedDocuments
                .Where(sd => sd.DocumentTemplateId == templateId)
                .Include(sd => sd.DocumentTemplate)
                .Include(sd => sd.History)
                .ToListAsync();
        }
    }
}
