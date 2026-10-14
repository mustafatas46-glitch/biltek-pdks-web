using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiltekPdks.Infrastructure.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class KullaniciBolumu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "kullanici_bolumu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: false),
                    BolumId = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FirmaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kullanici_bolumu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kullanici_bolumu_bolum_BolumId",
                        column: x => x.BolumId,
                        principalTable: "bolum",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_kullanici_bolumu_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_kullanici_bolumu_kullanici_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "kullanici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kullanici_bolumu_BolumId",
                table: "kullanici_bolumu",
                column: "BolumId");

            migrationBuilder.CreateIndex(
                name: "IX_kullanici_bolumu_FirmaId",
                table: "kullanici_bolumu",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_kullanici_bolumu_KullaniciId_BolumId",
                table: "kullanici_bolumu",
                columns: new[] { "KullaniciId", "BolumId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kullanici_bolumu");
        }
    }
}
