using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Last_Name",
                table: "Teachers",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Hire_Date",
                table: "Teachers",
                newName: "HireDate");

            migrationBuilder.RenameColumn(
                name: "First_Name",
                table: "Teachers",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "Last_Name",
                table: "Students",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "First_Name",
                table: "Students",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "Final_Grade",
                table: "Enrollments",
                newName: "FinalGrade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Teachers",
                newName: "Last_Name");

            migrationBuilder.RenameColumn(
                name: "HireDate",
                table: "Teachers",
                newName: "Hire_Date");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Teachers",
                newName: "First_Name");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Students",
                newName: "Last_Name");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Students",
                newName: "First_Name");

            migrationBuilder.RenameColumn(
                name: "FinalGrade",
                table: "Enrollments",
                newName: "Final_Grade");
        }
    }
}
