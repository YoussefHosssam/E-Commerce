using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantColorValueObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Color",
                table: "Variants",
                newName: "ColorName");

            migrationBuilder.Sql("""
                UPDATE [Variants]
                SET [ColorName] = 'Unknown'
                WHERE [ColorName] IS NULL OR LTRIM(RTRIM([ColorName])) = ''
                """);

            migrationBuilder.AlterColumn<string>(
                name: "ColorName",
                table: "Variants",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Unknown",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorHexCode",
                table: "Variants",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "#000000");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorHexCode",
                table: "Variants");

            migrationBuilder.AlterColumn<string>(
                name: "ColorName",
                table: "Variants",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.RenameColumn(
                name: "ColorName",
                table: "Variants",
                newName: "Color");
        }
    }
}
