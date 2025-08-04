using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneRandevu.Migrations
{
    /// <inheritdoc />
    public partial class yeni : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KullaniciAdi",
                table: "Hastalar");

            migrationBuilder.RenameColumn(
                name: "Sifre",
                table: "Hastalar",
                newName: "Parola");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Parola",
                table: "Hastalar",
                newName: "Sifre");

            migrationBuilder.AddColumn<string>(
                name: "KullaniciAdi",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
