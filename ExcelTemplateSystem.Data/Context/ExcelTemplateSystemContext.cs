using ExcelTemplateSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExcelTemplateSystem.Data.Context
{
    public class ExcelTemplateSystemContext : DbContext
    {
        public ExcelTemplateSystemContext(DbContextOptions<ExcelTemplateSystemContext> options) 
            : base(options)
        {
        }

        public DbSet<DocumentTemplate> DocumentTemplates { get; set; }
        public DbSet<TemplateColumn> TemplateColumns { get; set; }
        public DbSet<SubmittedDocument> SubmittedDocuments { get; set; }
        public DbSet<DocumentData> DocumentData { get; set; }
        public DbSet<DocumentHistory> DocumentHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentTemplate>()
                .HasMany(dt => dt.Columns)
                .WithOne(tc => tc.DocumentTemplate)
                .HasForeignKey(tc => tc.DocumentTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DocumentTemplate>()
                .HasMany(dt => dt.SubmittedDocuments)
                .WithOne(sd => sd.DocumentTemplate)
                .HasForeignKey(sd => sd.DocumentTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubmittedDocument>()
                .HasMany(sd => sd.Data)
                .WithOne(dd => dd.SubmittedDocument)
                .HasForeignKey(dd => dd.SubmittedDocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubmittedDocument>()
                .HasMany(sd => sd.History)
                .WithOne(dh => dh.SubmittedDocument)
                .HasForeignKey(dh => dh.SubmittedDocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TemplateColumn>()
                .HasMany(tc => tc.DocumentData)
                .WithOne(dd => dd.TemplateColumn)
                .HasForeignKey(dd => dd.TemplateColumnId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
