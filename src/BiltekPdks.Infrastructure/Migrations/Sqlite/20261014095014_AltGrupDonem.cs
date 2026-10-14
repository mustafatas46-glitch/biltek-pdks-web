using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiltekPdks.Infrastructure.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AltGrupDonem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AltGrupId",
                table: "personel",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "alt_grup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CalismaGrubuId = table.Column<int>(type: "INTEGER", nullable: false),
                    GunlukCalismaDakika = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FirmaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Kod = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false),
                    EskiId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alt_grup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alt_grup_calisma_grubu_CalismaGrubuId",
                        column: x => x.CalismaGrubuId,
                        principalTable: "calisma_grubu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_alt_grup_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "alt_grup_gunu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AltGrupId = table.Column<int>(type: "INTEGER", nullable: false),
                    Gun = table.Column<int>(type: "INTEGER", nullable: false),
                    VardiyaId = table.Column<int>(type: "INTEGER", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FirmaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alt_grup_gunu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alt_grup_gunu_alt_grup_AltGrupId",
                        column: x => x.AltGrupId,
                        principalTable: "alt_grup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_alt_grup_gunu_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_alt_grup_gunu_vardiya_VardiyaId",
                        column: x => x.VardiyaId,
                        principalTable: "vardiya",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "donem_tanimi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CalismaGrubuId = table.Column<int>(type: "INTEGER", nullable: false),
                    AltGrupId = table.Column<int>(type: "INTEGER", nullable: true),
                    Baslangic = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Bitis = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    GirisSaati = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    CikisSaati = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    GunFarki = table.Column<int>(type: "INTEGER", nullable: false),
                    EksikGun = table.Column<int>(type: "INTEGER", nullable: false),
                    EksikDakika = table.Column<int>(type: "INTEGER", nullable: false),
                    FazlaGun = table.Column<int>(type: "INTEGER", nullable: false),
                    FazlaDakika = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FirmaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donem_tanimi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_donem_tanimi_alt_grup_AltGrupId",
                        column: x => x.AltGrupId,
                        principalTable: "alt_grup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_donem_tanimi_calisma_grubu_CalismaGrubuId",
                        column: x => x.CalismaGrubuId,
                        principalTable: "calisma_grubu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_donem_tanimi_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_personel_AltGrupId",
                table: "personel",
                column: "AltGrupId");

            migrationBuilder.CreateIndex(
                name: "IX_alt_grup_CalismaGrubuId",
                table: "alt_grup",
                column: "CalismaGrubuId");

            migrationBuilder.CreateIndex(
                name: "IX_alt_grup_FirmaId",
                table: "alt_grup",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_alt_grup_FirmaId_Ad",
                table: "alt_grup",
                columns: new[] { "FirmaId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_alt_grup_gunu_AltGrupId_Gun",
                table: "alt_grup_gunu",
                columns: new[] { "AltGrupId", "Gun" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alt_grup_gunu_FirmaId",
                table: "alt_grup_gunu",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_alt_grup_gunu_VardiyaId",
                table: "alt_grup_gunu",
                column: "VardiyaId");

            migrationBuilder.CreateIndex(
                name: "IX_donem_tanimi_AltGrupId",
                table: "donem_tanimi",
                column: "AltGrupId");

            migrationBuilder.CreateIndex(
                name: "IX_donem_tanimi_CalismaGrubuId_Baslangic",
                table: "donem_tanimi",
                columns: new[] { "CalismaGrubuId", "Baslangic" });

            migrationBuilder.CreateIndex(
                name: "IX_donem_tanimi_FirmaId",
                table: "donem_tanimi",
                column: "FirmaId");

            migrationBuilder.AddForeignKey(
                name: "FK_personel_alt_grup_AltGrupId",
                table: "personel",
                column: "AltGrupId",
                principalTable: "alt_grup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_personel_alt_grup_AltGrupId",
                table: "personel");

            migrationBuilder.DropTable(
                name: "alt_grup_gunu");

            migrationBuilder.DropTable(
                name: "donem_tanimi");

            migrationBuilder.DropTable(
                name: "alt_grup");

            migrationBuilder.DropIndex(
                name: "IX_personel_AltGrupId",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "AltGrupId",
                table: "personel");
        }
    }
}
