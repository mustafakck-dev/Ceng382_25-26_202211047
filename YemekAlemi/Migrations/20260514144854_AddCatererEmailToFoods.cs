using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekAlemi.Migrations
{
    /// <inheritdoc />
    public partial class AddCatererEmailToFoods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CatererEmail",
                table: "Foods",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatererEmail",
                table: "Foods");
        }
    }
}
