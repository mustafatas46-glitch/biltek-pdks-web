using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiltekPdks.Infrastructure.Migrations.MySql
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
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AktarimId",
                table: "kazanc_kesinti",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AktarimId",
                table: "izin",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AktarimId",
                table: "hareket",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "aktarim_kaydi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    Zaman = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: true),
                    DosyaAdi = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SatirSayisi = table.Column<int>(type: "int", nullable: false),
                    Eklenen = table.Column<int>(type: "int", nullable: false),
                    Guncellenen = table.Column<int>(type: "int", nullable: false),
                    Atlanan = table.Column<int>(type: "int", nullable: false),
                    Hatali = table.Column<int>(type: "int", nullable: false),
                    Eslesme = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GeriAlindi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    GeriAlmaZamani = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aktarim_sablonu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    Ad = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Eslesme = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BaslikSatiri = table.Column<int>(type: "int", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "bordro_satiri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    Yil = table.Column<int>(type: "int", nullable: false),
                    Ay = table.Column<int>(type: "int", nullable: false),
                    CalisilanGun = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    IzinGun = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    DevamsizGun = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    FazlaMesaiSaat = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: true),
                    BrutUcret = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    NetUcret = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Aciklama = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AktarimId = table.Column<int>(type: "int", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
