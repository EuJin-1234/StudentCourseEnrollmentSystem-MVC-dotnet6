using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Course_Enrollment_System.Migrations
{
    /// <inheritdoc />
    public partial class AddIsWithdrawnCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWithdrawn",
                table: "Enrollments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWithdrawn",
                table: "Enrollments");
        }
    }
}
