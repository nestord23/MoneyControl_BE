using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyControl.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Loans_Status",
                table: "Loans",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_Date",
                table: "Incomes",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Date",
                table: "Expenses",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Type",
                table: "Expenses",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CategoryId_Date",
                table: "Expenses",
                columns: new[] { "CategoryId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Loans_Status",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_Date",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_Date",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_Type",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_CategoryId_Date",
                table: "Expenses");
        }
    }
}
