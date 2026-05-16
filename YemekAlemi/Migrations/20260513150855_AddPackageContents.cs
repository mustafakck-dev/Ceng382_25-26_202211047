using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekAlemi.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageContents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PackageContents",
                table: "Foods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackageContents",
                table: "Foods");
        }
    }
}
