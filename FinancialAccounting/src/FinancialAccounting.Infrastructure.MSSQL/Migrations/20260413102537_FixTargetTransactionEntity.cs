using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialAccounting.Infrastructure.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class FixTargetTransactionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TargetTransactions_TargetTransactions_TargetId",
                table: "TargetTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_TargetTransactions_Targets_TargetId",
                table: "TargetTransactions",
                column: "TargetId",
                principalTable: "Targets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TargetTransactions_Targets_TargetId",
                table: "TargetTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_TargetTransactions_TargetTransactions_TargetId",
                table: "TargetTransactions",
                column: "TargetId",
                principalTable: "TargetTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
