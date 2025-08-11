using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneRandevu.Migrations
{
    /// <inheritdoc />
    public partial class DoktorRandevuSaatKontrolu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Randevular_DoktorId",
                table: "Randevular");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_DoktorId_RandevuSaati",
                table: "Randevular",
                columns: new[] { "DoktorId", "RandevuSaati" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Randevular_DoktorId_RandevuSaati",
                table: "Randevular");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_DoktorId",
                table: "Randevular",
                column: "DoktorId");
        }
    }
}
