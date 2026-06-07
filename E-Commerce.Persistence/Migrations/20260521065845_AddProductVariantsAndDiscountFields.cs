using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVariantsAndDiscountFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Variants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAmount",
                table: "Variants",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PriceCurrency",
                table: "Variants",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EGP");

            migrationBuilder.AddColumn<decimal>(
                name: "CompareAtPriceAmount",
                table: "Products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompareAtPriceCurrency",
                table: "Products",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasDiscount",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasVariants",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("""
                UPDATE v
                SET
                    v.PriceAmount = COALESCE(v.PriceOverrideAmount, p.BasePriceAmount),
                    v.PriceCurrency = COALESCE(v.PriceOverrideCurrency, p.BasePriceCurrency)
                FROM [Variants] v
                INNER JOIN [Products] p ON p.Id = v.ProductId
                """);

            migrationBuilder.Sql("""
                UPDATE p
                SET p.HasVariants = CASE
                    WHEN variant_counts.ActiveVariantCount > 1 THEN CAST(1 AS bit)
                    ELSE CAST(0 AS bit)
                END
                FROM [Products] p
                INNER JOIN (
                    SELECT ProductId, COUNT(*) AS ActiveVariantCount
                    FROM [Variants]
                    WHERE IsActive = 1
                    GROUP BY ProductId
                ) variant_counts ON variant_counts.ProductId = p.Id
                """);

            migrationBuilder.Sql("""
                WITH RankedVariants AS (
                    SELECT
                        Id,
                        ROW_NUMBER() OVER (PARTITION BY ProductId ORDER BY Sku, Id) AS RowNumber
                    FROM [Variants]
                    WHERE IsActive = 1
                )
                UPDATE v
                SET IsDefault = CASE WHEN rv.RowNumber = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END
                FROM [Variants] v
                INNER JOIN RankedVariants rv ON rv.Id = v.Id
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Variants_ProductId_IsDefault",
                table: "Variants",
                columns: new[] { "ProductId", "IsDefault" },
                unique: true,
                filter: "[IsDefault] = 1 AND [IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Variants_ProductId_IsDefault",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "PriceAmount",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "PriceCurrency",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "CompareAtPriceAmount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CompareAtPriceCurrency",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasDiscount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasVariants",
                table: "Products");
        }
    }
}
