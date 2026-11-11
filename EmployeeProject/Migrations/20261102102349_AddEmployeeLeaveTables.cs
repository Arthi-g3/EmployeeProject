using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeLeaveTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
               // name: "EmployeeLeaveDetails",
            //    columns: table => new
            //    {
            //        LeaveId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        EmployeeId = table.Column<int>(type: "int", nullable: false),
            //        LeaveType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        TotalDays = table.Column<int>(type: "int", nullable: false),
            //        Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        DocumentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ReportingPersonId = table.Column<int>(type: "int", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ApproverRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EmployeeLeaveDetails", x => x.LeaveId);
            //        table.ForeignKey(
            //            name: "FK_EmployeeLeaveDetails_Employees_EmployeeId",
            //            column: x => x.EmployeeId,
            //            principalTable: "Employees",
            //            principalColumn: "EmpId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_EmployeeLeaveDetails_Employees_ReportingPersonId",
            //            column: x => x.ReportingPersonId,
            //            principalTable: "Employees",
            //            principalColumn: "EmpId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PendingLeaves",
            //    columns: table => new
            //    {
            //        PendingLeaveId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        LeaveId = table.Column<int>(type: "int", nullable: false),
            //        ReportingPersonId = table.Column<int>(type: "int", nullable: false),
            //        PendingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PendingLeaves", x => x.PendingLeaveId);
            //        table.ForeignKey(
            //            name: "FK_PendingLeaves_EmployeeLeaveDetails_LeaveId",
            //            column: x => x.LeaveId,
            //            principalTable: "EmployeeLeaveDetails",
            //            principalColumn: "LeaveId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_PendingLeaves_Employees_ReportingPersonId",
            //            column: x => x.ReportingPersonId,
            //            principalTable: "Employees",
            //            principalColumn: "EmpId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_EmployeeLeaveDetails_EmployeeId",
            //    table: "EmployeeLeaveDetails",
            //    column: "EmployeeId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_EmployeeLeaveDetails_ReportingPersonId",
            //    table: "EmployeeLeaveDetails",
            //    column: "ReportingPersonId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PendingLeaves_LeaveId",
            //    table: "PendingLeaves",
            //    column: "LeaveId",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_PendingLeaves_ReportingPersonId",
            //    table: "PendingLeaves",
            //    column: "ReportingPersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "PendingLeaves");

            //migrationBuilder.DropTable(
            //    name: "EmployeeLeaveDetails");
        }
    }
}
