using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiltekPdks.Infrastructure.Migrations.MySql
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    BolumId = table.Column<int>(type: "int", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
