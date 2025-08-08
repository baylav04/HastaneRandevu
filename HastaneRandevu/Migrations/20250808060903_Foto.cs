using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneRandevu.Migrations
{
    /// <inheritdoc />
    public partial class Foto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Detaylar",
                table: "Doktorlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "Doktorlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unvan",
                table: "Doktorlar",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Detaylar",
                table: "Doktorlar");

            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "Doktorlar");

            migrationBuilder.DropColumn(
                name: "Unvan",
                table: "Doktorlar");
        }
    }
}
