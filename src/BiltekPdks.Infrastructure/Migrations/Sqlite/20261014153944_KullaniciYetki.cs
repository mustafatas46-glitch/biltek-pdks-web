using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiltekPdks.Infrastructure.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class KullaniciYetki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OzelYetki",
                table: "kullanici",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "kullanici_yetkisi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    Yetki = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FirmaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kullanici_yetkisi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kullanici_yetkisi_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_kullanici_yetkisi_kullanici_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "kullanici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kullanici_yetkisi_FirmaId",
                table: "kullanici_yetkisi",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_kullanici_yetkisi_KullaniciId_Yetki",
                table: "kullanici_yetkisi",
                columns: new[] { "KullaniciId", "Yetki" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kullanici_yetkisi");

            migrationBuilder.DropColumn(
                name: "OzelYetki",
                table: "kullanici");
        }
    }
}
