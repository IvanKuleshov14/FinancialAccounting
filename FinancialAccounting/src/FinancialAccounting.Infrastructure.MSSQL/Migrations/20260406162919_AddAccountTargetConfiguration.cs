using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialAccounting.Infrastructure.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountTargetConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AccountTarget_TargetId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_TargetId",
                table: "Accounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountTarget",
                table: "AccountTarget");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "AccountTarget",
                newName: "AccountTargets");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AccountTargets",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "AccountTargets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountTargets",
                table: "AccountTargets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTargets_AccountId",
                table: "AccountTargets",
                column: "AccountId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTargets_Accounts_AccountId",
                table: "AccountTargets",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountTargets_Accounts_AccountId",
                table: "AccountTargets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountTargets",
                table: "AccountTargets");

            migrationBuilder.DropIndex(
                name: "IX_AccountTargets_AccountId",
                table: "AccountTargets");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AccountTargets");

            migrationBuilder.RenameTable(
                name: "AccountTargets",
                newName: "AccountTarget");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AccountTarget",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountTarget",
                table: "AccountTarget",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_TargetId",
                table: "Accounts",
                column: "TargetId",
                unique: true,
                filter: "[TargetId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AccountTarget_TargetId",
                table: "Accounts",
                column: "TargetId",
                principalTable: "AccountTarget",
                principalColumn: "Id");
        }
    }
}
