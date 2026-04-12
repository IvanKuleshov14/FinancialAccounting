using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialAccounting.Infrastructure.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountTargetEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                table: "AccountTargets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "AccountTargets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
