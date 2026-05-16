using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekAlemi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToEmailLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "EmailLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EmailLogs");
        }
    }
}
