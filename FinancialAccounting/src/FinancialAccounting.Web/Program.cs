using FinancialAccounting.Application;
using FinancialAccounting.Infrastructure.MSSQL;
using FinancialAccounting.Infrastructure.MSSQL.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInsfrasturcture();

var connectionString = builder.Configuration.GetConnectionString("MsSqlServerConnectionString");
builder.Services.AddDbContext<FinancialAccountingDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseAuthorization();

app.MapControllers();

app.Run();