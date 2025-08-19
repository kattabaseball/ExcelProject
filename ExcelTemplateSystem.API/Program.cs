using Microsoft.EntityFrameworkCore;
using ExcelTemplateSystem.Data.Context;
using ExcelTemplateSystem.Data.Interfaces;
using ExcelTemplateSystem.Data.Repositories;
using ExcelTemplateSystem.Business.Interfaces;
using ExcelTemplateSystem.Business.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext
builder.Services.AddDbContext<ExcelTemplateSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IDocumentTemplateRepository, DocumentTemplateRepository>();

// Register services
builder.Services.AddScoped<IDocumentTemplateService, DocumentTemplateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ExcelTemplateSystemContext>();
    context.Database.Migrate();
}

app.Run();
