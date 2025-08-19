using ExcelTemplateSystem.Business.Interfaces;
using ExcelTemplateSystem.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ExcelTemplateSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentTemplatesController : ControllerBase
    {
        private readonly IDocumentTemplateService _templateService;

        public DocumentTemplatesController(IDocumentTemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DocumentTemplateDto>>> GetAllTemplates()
        {
            var templates = await _templateService.GetAllTemplates();
            return Ok(templates);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentTemplateDto>> GetTemplateById(int id)
        {
            var template = await _templateService.GetTemplateById(id);
            if (template == null) return NotFound();
            return Ok(template);
        }

        [HttpPost]
        public async Task<ActionResult<DocumentTemplateDto>> CreateTemplate(DocumentTemplateDto templateDto)
        {
            var createdTemplate = await _templateService.CreateTemplate(templateDto);
            return CreatedAtAction(nameof(GetTemplateById), new { id = createdTemplate.Id }, createdTemplate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, DocumentTemplateDto templateDto)
        {
            if (id != templateDto.Id) return BadRequest();
            
            var updatedTemplate = await _templateService.UpdateTemplate(templateDto);
            if (updatedTemplate == null) return NotFound();
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var result = await _templateService.DeleteTemplate(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{templateId}/columns")]
        public async Task<ActionResult<TemplateColumnDto>> AddColumnToTemplate(int templateId, TemplateColumnDto columnDto)
        {
            if (templateId != columnDto.DocumentTemplateId) return BadRequest();
            
            var column = await _templateService.AddColumnToTemplate(columnDto);
            return CreatedAtAction(nameof(GetTemplateById), new { id = templateId }, column);
        }

        [HttpDelete("columns/{columnId}")]
        public async Task<IActionResult> RemoveColumnFromTemplate(int columnId)
        {
            var result = await _templateService.RemoveColumnFromTemplate(columnId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("{templateId}/download")]
        public async Task<IActionResult> DownloadTemplate(int templateId)
        {
            var stream = await _templateService.GenerateExcelTemplate(templateId);
            if (stream == null) return NotFound();
            
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "template.xlsx");
        }

        [HttpPost("{templateId}/submit")]
        public async Task<ActionResult<DocumentSubmissionResult>> SubmitDocument(int templateId, IFormFile file, [FromQuery] string submittedBy)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
                return BadRequest("Only Excel files (.xlsx) are supported");

            await using var stream = file.OpenReadStream();
            var result = await _templateService.SubmitDocumentData(templateId, stream, submittedBy);
            
            if (result.IsValid)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpGet("submissions/{uniqueIdentifier}")]
        public async Task<ActionResult<SubmittedDocumentDto>> GetSubmittedDocument(string uniqueIdentifier)
        {
            var document = await _templateService.GetSubmittedDocument(uniqueIdentifier);
            if (document == null) return NotFound();
            return Ok(document);
        }

        [HttpGet("{templateId}/submissions")]
        public async Task<ActionResult<List<SubmittedDocumentDto>>> GetSubmittedDocumentsByTemplate(int templateId)
        {
            var documents = await _templateService.GetSubmittedDocumentsByTemplate(templateId);
            return Ok(documents);
        }
    }
}
