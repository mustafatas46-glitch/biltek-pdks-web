using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiltekPdks.Infrastructure.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class ExcelAktarim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AktarimId",
                table: "personel",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AktarimId",
                table: "kazanc_kesinti",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AktarimId",
                table: "izin",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AktarimId",
                table: "hareket",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "aktarim_kaydi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tur = table.Column<int>(type: "INTEGER", nullable: false),
                    Zaman = table.Column<DateTime>(type: "TEXT", nullable: false),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: true),
                    DosyaAdi = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    SatirSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    Eklenen = table.Column<int>(type: "INTEGER", nullable: false),
                    Guncellenen = table.Column<int>(type: "INTEGER", nullable: false),
                    Atlanan = table.Column<int>(type: "INTEGER", nullable: false),
                    Hatali = table.Column<int>(type: "INTEGER", nullable: false),
                    Eslesme = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    GeriAlindi = table.Column<bool>(type: "INTEGER", nullable: false),
                    GeriAlmaZamani = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FirmaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aktarim_kaydi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_aktarim_kaydi_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "aktarim_sablonu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tur = table.Column<int>(type: "INTEGER", nullable: false),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Eslesme = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    BaslikSatiri = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FirmaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aktarim_sablonu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_aktarim_sablonu_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bordro_satiri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PersonelId = table.Column<int>(type: "INTEGER", nullable: false),
                    Yil = table.Column<int>(type: "INTEGER", nullable: false),
                    Ay = table.Column<int>(type: "INTEGER", nullable: false),
                    CalisilanGun = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: true),
                    IzinGun = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: true),
                    DevamsizGun = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: true),
                    FazlaMesaiSaat = table.Column<decimal>(type: "TEXT", precision: 8, scale: 2, nullable: true),
                    BrutUcret = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    NetUcret = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    AktarimId = table.Column<int>(type: "INTEGER", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FirmaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bordro_satiri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bordro_satiri_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bordro_satiri_personel_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "personel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kazanc_kesinti_AktarimId",
                table: "kazanc_kesinti",
                column: "AktarimId");

            migrationBuilder.CreateIndex(
                name: "IX_izin_AktarimId",
                table: "izin",
                column: "AktarimId");

            migrationBuilder.CreateIndex(
                name: "IX_hareket_AktarimId",
                table: "hareket",
                column: "AktarimId");

            migrationBuilder.CreateIndex(
                name: "IX_aktarim_kaydi_FirmaId",
                table: "aktarim_kaydi",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_aktarim_kaydi_FirmaId_Zaman",
                table: "aktarim_kaydi",
                columns: new[] { "FirmaId", "Zaman" });

            migrationBuilder.CreateIndex(
                name: "IX_aktarim_sablonu_FirmaId",
                table: "aktarim_sablonu",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_aktarim_sablonu_FirmaId_Tur_Ad",
                table: "aktarim_sablonu",
                columns: new[] { "FirmaId", "Tur", "Ad" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bordro_satiri_AktarimId",
                table: "bordro_satiri",
                column: "AktarimId");

            migrationBuilder.CreateIndex(
                name: "IX_bordro_satiri_FirmaId",
                table: "bordro_satiri",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_bordro_satiri_PersonelId_Yil_Ay",
                table: "bordro_satiri",
                columns: new[] { "PersonelId", "Yil", "Ay" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aktarim_kaydi");

            migrationBuilder.DropTable(
                name: "aktarim_sablonu");

            migrationBuilder.DropTable(
                name: "bordro_satiri");

            migrationBuilder.DropIndex(
                name: "IX_kazanc_kesinti_AktarimId",
                table: "kazanc_kesinti");

            migrationBuilder.DropIndex(
                name: "IX_izin_AktarimId",
                table: "izin");

            migrationBuilder.DropIndex(
                name: "IX_hareket_AktarimId",
                table: "hareket");

            migrationBuilder.DropColumn(
                name: "AktarimId",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "AktarimId",
                table: "kazanc_kesinti");

            migrationBuilder.DropColumn(
                name: "AktarimId",
                table: "izin");

            migrationBuilder.DropColumn(
                name: "AktarimId",
                table: "hareket");
        }
    }
}
