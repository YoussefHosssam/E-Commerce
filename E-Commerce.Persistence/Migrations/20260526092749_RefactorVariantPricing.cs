using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorVariantPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PriceCurrency",
                table: "Variants",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceAmount",
                table: "Variants",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.Sql("""
                UPDATE [Variants]
                SET
                    [PriceAmount] = NULL,
                    [PriceCurrency] = NULL
                WHERE [PriceOverrideAmount] IS NULL
                  AND [PriceOverrideCurrency] IS NULL
                """);

            migrationBuilder.DropColumn(
                name: "PriceOverrideAmount",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "PriceOverrideCurrency",
                table: "Variants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceOverrideAmount",
                table: "Variants",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceOverrideCurrency",
                table: "Variants",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE v
                SET
                    v.PriceOverrideAmount = v.PriceAmount,
                    v.PriceOverrideCurrency = v.PriceCurrency
                FROM [Variants] v
                WHERE v.PriceAmount IS NOT NULL
                  AND v.PriceCurrency IS NOT NULL
                """);

            migrationBuilder.Sql("""
                UPDATE v
                SET
                    v.PriceAmount = p.BasePriceAmount,
                    v.PriceCurrency = p.BasePriceCurrency
                FROM [Variants] v
                INNER JOIN [Products] p ON p.Id = v.ProductId
                WHERE v.PriceAmount IS NULL
                   OR v.PriceCurrency IS NULL
                """);

            migrationBuilder.AlterColumn<string>(
                name: "PriceCurrency",
                table: "Variants",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceAmount",
                table: "Variants",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
