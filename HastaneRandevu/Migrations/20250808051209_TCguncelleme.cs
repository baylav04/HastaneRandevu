using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneRandevu.Migrations
{
    /// <inheritdoc />
    public partial class TCguncelleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TCKimlikNo",
                table: "Hastalar",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Hastalar_TCKimlikNo",
                table: "Hastalar",
                column: "TCKimlikNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Hastalar_TCKimlikNo",
                table: "Hastalar");

            migrationBuilder.AlterColumn<string>(
                name: "TCKimlikNo",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);
        }
    }
}
