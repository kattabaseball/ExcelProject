using ClosedXML.Excel;
using ExcelTemplateSystem.Business.Interfaces;
using ExcelTemplateSystem.Business.Models;
using ExcelTemplateSystem.Data.Entities;
using ExcelTemplateSystem.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExcelTemplateSystem.Business.Services
{
    public class DocumentTemplateService : IDocumentTemplateService
    {
        private readonly IDocumentTemplateRepository _repository;

        public DocumentTemplateService(IDocumentTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<DocumentTemplateDto> GetTemplateById(int id)
        {
            var template = await _repository.GetTemplateById(id);
            return MapToDto(template);
        }

        public async Task<List<DocumentTemplateDto>> GetAllTemplates()
        {
            var templates = await _repository.GetAllTemplates();
            return templates.Select(MapToDto).ToList();
        }

        public async Task<DocumentTemplateDto> CreateTemplate(DocumentTemplateDto templateDto)
        {
            var template = new DocumentTemplate
            {
                Name = templateDto.Name,
                Description = templateDto.Description,
                Version = templateDto.Version,
                IsActive = templateDto.IsActive,
                Columns = templateDto.Columns?.Select(c => new TemplateColumn
                {
                    ColumnName = c.ColumnName,
                    DataType = c.DataType,
                    IsRequired = c.IsRequired,
                    MinValue = c.MinValue,
                    MaxValue = c.MaxValue,
                    MaxLength = c.MaxLength,
                    DefaultValue = c.DefaultValue,
                    ValidationRegex = c.ValidationRegex
                }).ToList()
            };

            var createdTemplate = await _repository.CreateTemplate(template);
            return MapToDto(createdTemplate);
        }

        public async Task<DocumentTemplateDto> UpdateTemplate(DocumentTemplateDto templateDto)
        {
            var template = await _repository.GetTemplateById(templateDto.Id);
            if (template == null) return null;

            template.Name = templateDto.Name;
            template.Description = templateDto.Description;
            template.Version = templateDto.Version;
            template.IsActive = templateDto.IsActive;

            // Update columns if provided
            if (templateDto.Columns != null)
            {
                // Remove existing columns
                foreach (var existingColumn in template.Columns.ToList())
                {
                    await _repository.RemoveColumnFromTemplate(existingColumn.Id);
                }

                // Add updated columns
                foreach (var columnDto in templateDto.Columns)
                {
                    var column = new TemplateColumn
                    {
                        DocumentTemplateId = template.Id,
                        ColumnName = columnDto.ColumnName,
                        DataType = columnDto.DataType,
                        IsRequired = columnDto.IsRequired,
                        MinValue = columnDto.MinValue,
                        MaxValue = columnDto.MaxValue,
                        MaxLength = columnDto.MaxLength,
                        DefaultValue = columnDto.DefaultValue,
                        ValidationRegex = columnDto.ValidationRegex
                    };
                    await _repository.AddColumnToTemplate(column);
                }
            }

            var updatedTemplate = await _repository.UpdateTemplate(template);
            return MapToDto(updatedTemplate);
        }

        public async Task<bool> DeleteTemplate(int id)
        {
            return await _repository.DeleteTemplate(id);
        }

        public async Task<TemplateColumnDto> AddColumnToTemplate(TemplateColumnDto columnDto)
        {
            var column = new TemplateColumn
            {
                DocumentTemplateId = columnDto.DocumentTemplateId,
                ColumnName = columnDto.ColumnName,
                DataType = columnDto.DataType,
                IsRequired = columnDto.IsRequired,
                MinValue = columnDto.MinValue,
                MaxValue = columnDto.MaxValue,
                MaxLength = columnDto.MaxLength,
                DefaultValue = columnDto.DefaultValue,
                ValidationRegex = columnDto.ValidationRegex
            };

            var addedColumn = await _repository.AddColumnToTemplate(column);
            return MapToDto(addedColumn);
        }

        public async Task<bool> RemoveColumnFromTemplate(int columnId)
        {
            return await _repository.RemoveColumnFromTemplate(columnId);
        }

        public async Task<Stream> GenerateExcelTemplate(int templateId)
        {
            var template = await _repository.GetTemplateById(templateId);
            if (template == null) return null;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Template");

            // Add headers with formatting
            int row = 1;
            int col = 1;
            
            // Header styling will be applied directly to cells

            foreach (var column in template.Columns.OrderBy(c => c.Id))
            {
                var cell = worksheet.Cell(row, col);
                cell.Value = column.ColumnName;
                // Apply style properties directly to the cell
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                
                // Set column width based on data type
                if (column.DataType == "Number" || column.DataType == "Currency")
                    worksheet.Column(col).Width = 15;
                else if (column.DataType == "Date")
                    worksheet.Column(col).Width = 15;
                else if (column.DataType == "Boolean")
                    worksheet.Column(col).Width = 10;
                else
                    worksheet.Column(col).Width = 20;
                
                // Apply data validation to the column (starting from row 2)
                var dataRange = worksheet.Range(2, col, 1000, col);
                
                // Apply data type specific validation
                if (column.DataType == "Number" || column.DataType == "Currency")
                {
                    // Set number format
                    var numberFormat = column.DataType == "Currency" ? "$#,##0.00" : "0.00";
                    dataRange.Style.NumberFormat.Format = numberFormat;
                    
                    // Apply number validation
                    var validation = dataRange.CreateDataValidation();
                    
                    if (column.MinValue.HasValue || column.MaxValue.HasValue)
                    {
                        // Convert decimal values to int for validation
                        int minValue = column.MinValue.HasValue ? (int)Math.Floor(column.MinValue.Value) : int.MinValue;
                        int maxValue = column.MaxValue.HasValue ? (int)Math.Ceiling(column.MaxValue.Value) : int.MaxValue;
                        
                        validation.WholeNumber.Between(minValue, maxValue);
                        validation.ErrorTitle = "Invalid Number";
                        validation.ErrorMessage = $"Please enter a valid number between {minValue} and {maxValue}";
                    }
                    else
                    {
                        // If no range specified, just validate that it's a number
                        validation.WholeNumber.Between(int.MinValue, int.MaxValue);
                        validation.ErrorTitle = "Invalid Number";
                        validation.ErrorMessage = "Please enter a valid number";
                    }
                }
                else if (column.DataType == "Date")
                {
                    // Set date format
                    dataRange.Style.DateFormat.Format = "yyyy-mm-dd";
                    
                    // Apply date validation
                    var validation = dataRange.CreateDataValidation();
                    validation.Date.Between("1900-01-01", "2100-12-31");
                    validation.ErrorTitle = "Invalid Date";
                    validation.ErrorMessage = "Please enter a valid date (YYYY-MM-DD)";
                }
                else if (column.DataType == "Boolean")
                {
                    // Apply boolean validation (Yes/No dropdown)
                    var validation = dataRange.CreateDataValidation();
                    validation.List("Yes,No");
                    validation.ErrorTitle = "Invalid Selection";
                    validation.ErrorMessage = "Please select 'Yes' or 'No' from the dropdown";
                    
                    // Set default value if specified
                    if (!string.IsNullOrEmpty(column.DefaultValue))
                        worksheet.Cell(2, col).Value = column.DefaultValue;
                }
                else // Text type
                {
                    // Apply text length validation if specified
                    if (column.MaxLength.HasValue)
                    {
                        var validation = dataRange.CreateDataValidation();
                        // Using Between with 0 and MaxLength to ensure text length is within limits
                        validation.TextLength.Between(0, column.MaxLength.Value);
                        validation.ErrorTitle = "Text Too Long";
                        validation.ErrorMessage = $"Text cannot exceed {column.MaxLength.Value} characters";
                    }
                }
                
                // Apply required field validation
                if (column.IsRequired)
                {
                    var validation = dataRange.CreateDataValidation();
                    validation.Custom("LEN(TRIM(A1))>0");
                    validation.ErrorTitle = "Required Field";
                    validation.ErrorMessage = $"'{column.ColumnName}' is a required field";
                }
                
                // Set default value if specified and not already set
                if (!string.IsNullOrEmpty(column.DefaultValue) && column.DataType != "Boolean")
                {
                    worksheet.Cell(2, col).Value = column.DefaultValue;
                }
                
                col++;
            }

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task<DocumentSubmissionResult> SubmitDocumentData(int templateId, Stream excelFile, string submittedBy)
        {
            var result = new DocumentSubmissionResult();
            var template = await _repository.GetTemplateById(templateId);
            if (template == null)
            {
                result.Errors.Add(new ValidationError { ErrorMessage = "Template not found" });
                return result;
            }

            try
            {
                using var workbook = new XLWorkbook(excelFile);
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1); // Skip header row

                var submittedDocument = new SubmittedDocument
                {
                    DocumentTemplateId = templateId,
                    SubmittedBy = submittedBy,
                    Status = "Submitted",
                    Data = new List<DocumentData>(),
                    History = new List<DocumentHistory>
                    {
                        new DocumentHistory
                        {
                            Action = "Created",
                            ActionBy = submittedBy,
                            Notes = "Document submitted"
                        }
                    }
                };

                foreach (var row in rows)
                {
                    foreach (var column in template.Columns)
                    {
                        var cellValue = row.Cell(column.Id).Value.ToString();
                        var validationResult = ValidateCellValue(cellValue, column);
                        
                        if (!validationResult.IsValid)
                        {
                            result.Errors.Add(new ValidationError
                            {
                                ColumnName = column.ColumnName,
                                ErrorMessage = validationResult.ErrorMessage
                            });
                        }

                        submittedDocument.Data.Add(new DocumentData
                        {
                            TemplateColumnId = column.Id,
                            Value = cellValue
                        });
                    }
                }

                if (!result.Errors.Any())
                {
                    submittedDocument.Status = "Validated";
                    submittedDocument.History.Add(new DocumentHistory
                    {
                        Action = "Validated",
                        ActionBy = "System",
                        Notes = "Document validated successfully"
                    });
                }

                var savedDocument = await _repository.SubmitDocument(submittedDocument);
                result.UniqueIdentifier = savedDocument.UniqueIdentifier;
                result.IsValid = !result.Errors.Any();
                
                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationError { ErrorMessage = $"Error processing document: {ex.Message}" });
                return result;
            }
        }

        public async Task<SubmittedDocumentDto> GetSubmittedDocument(string uniqueIdentifier)
        {
            var document = await _repository.GetSubmittedDocument(uniqueIdentifier);
            return MapToDto(document);
        }

        public async Task<List<SubmittedDocumentDto>> GetSubmittedDocumentsByTemplate(int templateId)
        {
            var documents = await _repository.GetSubmittedDocumentsByTemplate(templateId);
            return documents.Select(MapToDto).ToList();
        }

        private (bool IsValid, string ErrorMessage) ValidateCellValue(string value, TemplateColumn column)
        {
            if (column.IsRequired && string.IsNullOrWhiteSpace(value))
            {
                return (false, $"{column.ColumnName} is required");
            }

            if (string.IsNullOrWhiteSpace(value)) return (true, null);

            switch (column.DataType)
            {
                case "Number":
                case "Currency":
                    if (!decimal.TryParse(value, out var number))
                        return (false, $"{column.ColumnName} must be a number");
                    
                    if (column.MinValue.HasValue && number < column.MinValue.Value)
                        return (false, $"{column.ColumnName} must be at least {column.MinValue}");
                    
                    if (column.MaxValue.HasValue && number > column.MaxValue.Value)
                        return (false, $"{column.ColumnName} must be at most {column.MaxValue}");
                    break;

                case "Date":
                    if (!DateTime.TryParse(value, out _))
                        return (false, $"{column.ColumnName} must be a valid date");
                    break;

                case "Boolean":
                    if (!bool.TryParse(value, out _))
                        return (false, $"{column.ColumnName} must be true or false");
                    break;
            }

            if (column.MaxLength.HasValue && value.Length > column.MaxLength.Value)
                return (false, $"{column.ColumnName} must be at most {column.MaxLength} characters");

            if (!string.IsNullOrEmpty(column.ValidationRegex) && 
                !Regex.IsMatch(value, column.ValidationRegex))
                return (false, $"{column.ColumnName} format is invalid");

            return (true, null);
        }

        private DocumentTemplateDto MapToDto(DocumentTemplate template)
        {
            if (template == null) return null;

            return new DocumentTemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                Version = template.Version,
                CreatedDate = template.CreatedDate,
                IsActive = template.IsActive,
                Columns = template.Columns?.Select(MapToDto).ToList()
            };
        }

        private TemplateColumnDto MapToDto(TemplateColumn column)
        {
            if (column == null) return null;

            return new TemplateColumnDto
            {
                Id = column.Id,
                DocumentTemplateId = column.DocumentTemplateId,
                ColumnName = column.ColumnName,
                DataType = column.DataType,
                IsRequired = column.IsRequired,
                MinValue = column.MinValue,
                MaxValue = column.MaxValue,
                MaxLength = column.MaxLength,
                DefaultValue = column.DefaultValue,
                ValidationRegex = column.ValidationRegex
            };
        }

        private SubmittedDocumentDto MapToDto(SubmittedDocument document)
        {
            if (document == null) return null;

            return new SubmittedDocumentDto
            {
                Id = document.Id,
                DocumentTemplateId = document.DocumentTemplateId,
                UniqueIdentifier = document.UniqueIdentifier,
                SubmittedBy = document.SubmittedBy,
                SubmittedDate = document.SubmittedDate,
                Status = document.Status,
                Data = document.Data?.Select(d => new DocumentDataDto
                {
                    TemplateColumnId = d.TemplateColumnId,
                    Value = d.Value
                }).ToList(),
                History = document.History?.Select(h => new DocumentHistoryDto
                {
                    Action = h.Action,
                    ActionBy = h.ActionBy,
                    ActionDate = h.ActionDate,
                    Notes = h.Notes
                }).ToList()
            };
        }
    }
}
