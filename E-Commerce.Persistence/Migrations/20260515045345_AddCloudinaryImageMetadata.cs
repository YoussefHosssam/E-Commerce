using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCloudinaryImageMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "VariantImages",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "legacy");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "VariantImages",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingStatus",
                table: "VariantImages",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Ready");

            migrationBuilder.AddColumn<long>(
                name: "SizeInBytes",
                table: "VariantImages",
                type: "bigint",
                nullable: false,
                defaultValue: 1L);

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "VariantImages",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "VariantImages",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "ProductImages",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "legacy");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "ProductImages",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingStatus",
                table: "ProductImages",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Ready");

            migrationBuilder.AddColumn<long>(
                name: "SizeInBytes",
                table: "ProductImages",
                type: "bigint",
                nullable: false,
                defaultValue: 1L);

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "ProductImages",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "ProductImages",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("""
                UPDATE ProductImages
                SET StorageKey = CONCAT('legacy/product-images/', CONVERT(nvarchar(36), Id))
                WHERE StorageKey = ''
                """);

            migrationBuilder.Sql("""
                UPDATE VariantImages
                SET StorageKey = CONCAT('legacy/variant-images/', CONVERT(nvarchar(36), Id))
                WHERE StorageKey = ''
                """);

            migrationBuilder.CreateIndex(
                name: "IX_VariantImages_StorageKey",
                table: "VariantImages",
                column: "StorageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VariantImages_VariantId",
                table: "VariantImages",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_StorageKey",
                table: "ProductImages",
                column: "StorageKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VariantImages_StorageKey",
                table: "VariantImages");

            migrationBuilder.DropIndex(
                name: "IX_VariantImages_VariantId",
                table: "VariantImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_StorageKey",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "VariantImages");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "VariantImages");

            migrationBuilder.DropColumn(
                name: "ProcessingStatus",
                table: "VariantImages");

            migrationBuilder.DropColumn(
                name: "SizeInBytes",
                table: "VariantImages");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "VariantImages");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "VariantImages");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ProcessingStatus",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "SizeInBytes",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "ProductImages");
        }
    }
}
