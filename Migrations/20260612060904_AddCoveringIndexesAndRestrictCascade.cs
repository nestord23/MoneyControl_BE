using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyControl.Migrations
{
    /// <inheritdoc />
    public partial class AddCoveringIndexesAndRestrictCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Categories_CategoryId",
                table: "Expenses");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Categories_CategoryId",
                table: "Expenses",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS IX_Expenses_Date_Amount
                ON "Expenses" ("Date") INCLUDE ("Amount")
            """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS IX_Expenses_Type_Amount
                ON "Expenses" ("Type") INCLUDE ("Amount")
            """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS IX_Incomes_Date_Amount
                ON "Incomes" ("Date") INCLUDE ("Amount")
            """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS IX_Loans_Status_Amount
                ON "Loans" ("Status") INCLUDE ("Amount")
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Expenses_Date_Amount");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Expenses_Type_Amount");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Incomes_Date_Amount");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Loans_Status_Amount");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Categories_CategoryId",
                table: "Expenses");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Categories_CategoryId",
                table: "Expenses",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
