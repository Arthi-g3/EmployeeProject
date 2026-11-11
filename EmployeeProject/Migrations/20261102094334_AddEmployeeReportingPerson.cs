using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeReportingPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReportingPersonId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ReportingPersonId",
                table: "Employees",
                column: "ReportingPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Employees_ReportingPersonId",
                table: "Employees",
                column: "ReportingPersonId",
                principalTable: "Employees",
                principalColumn: "EmpId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Employees_ReportingPersonId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ReportingPersonId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ReportingPersonId",
                table: "Employees");
        }
    }
}
