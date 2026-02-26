using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothingStore.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceBrandWebsiteAndProductSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Size",
                table: "Products",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "WebsiteUrl",
                table: "Brands",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Products",
                newName: "Size");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Brands",
                newName: "WebsiteUrl");
        }
    }
}
