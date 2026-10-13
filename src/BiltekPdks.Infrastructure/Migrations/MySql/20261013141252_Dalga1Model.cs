using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiltekPdks.Infrastructure.Migrations.MySql
{
    /// <inheritdoc />
    public partial class Dalga1Model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_personel_FirmaId_KartNo",
                table: "personel");

            migrationBuilder.AddColumn<string>(
                name: "AnneAdi",
                table: "personel",
                type: "varchar(60)",
                maxLength: 60,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AskerlikDurumu",
                table: "personel",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BabaAdi",
                table: "personel",
                type: "varchar(60)",
                maxLength: 60,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BankaAdi",
                table: "personel",
                type: "varchar(60)",
                maxLength: 60,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "BirimId",
                table: "personel",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BolgeId",
                table: "personel",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CocukSayisi",
                table: "personel",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DevamsizlikCezaKatsayisi",
                table: "personel",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DogumYeri",
                table: "personel",
                type: "varchar(60)",
                maxLength: 60,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EgitimDurumu",
                table: "personel",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EhliyetNo",
                table: "personel",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EhliyetSinifi",
                table: "personel",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "EskiUcret",
                table: "personel",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvTelefonu",
                table: "personel",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "FazlaMesaiAlir",
                table: "personel",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Iban",
                table: "personel",
                type: "varchar(34)",
                maxLength: 34,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Il",
                table: "personel",
                type: "varchar(60)",
                maxLength: 60,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Ilce",
                table: "personel",
                type: "varchar(60)",
                maxLength: 60,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateOnly>(
                name: "IlkIseBaslamaTarihi",
                table: "personel",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IlkSoyad",
                table: "personel",
                type: "varchar(60)",
                maxLength: 60,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "KanGrubu",
                table: "personel",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "KartId",
                table: "personel",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MedeniHal",
                table: "personel",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "MesaiSaatUcreti",
                table: "personel",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MeslekId",
                table: "personel",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NufusIl",
                table: "personel",
                type: "varchar(60)",
                maxLength: 60,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NufusIlce",
                table: "personel",
                type: "varchar(60)",
                maxLength: 60,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ServisId",
                table: "personel",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "SgkGirisTarihi",
                table: "personel",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SgkSicilNo",
                table: "personel",
                type: "varchar(40)",
                maxLength: 40,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "TakimId",
                table: "personel",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Ucret",
                table: "personel",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UcretTipi",
                table: "personel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Uyruk",
                table: "personel",
                type: "varchar(60)",
                maxLength: 60,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "VergiNo",
                table: "personel",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "FazlaMesaiCarpani",
                table: "calisma_grubu",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MaasBoleniSaat",
                table: "calisma_grubu",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TatilMesaisiCarpani",
                table: "calisma_grubu",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "birim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false),
                    Kod = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ad = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Aktif = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EskiId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_birim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_birim_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "bolge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false),
                    Kod = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ad = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Aktif = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EskiId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bolge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bolge_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "islem_kaydi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Zaman = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: true),
                    Tablo = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    KayitId = table.Column<int>(type: "int", nullable: false),
                    Islem = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Degisiklikler = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_islem_kaydi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_islem_kaydi_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "kazanc_kesinti_turu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false),
                    Kod = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ad = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Aktif = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EskiId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kazanc_kesinti_turu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kazanc_kesinti_turu_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "meslek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false),
                    Kod = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ad = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Aktif = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EskiId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meslek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_meslek_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "servis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false),
                    Kod = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ad = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Aktif = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EskiId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_servis_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "takim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false),
                    Kod = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ad = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Aktif = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EskiId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_takim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_takim_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "kazanc_kesinti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    KazancKesintiTuruId = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateOnly>(type: "date", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    KaydedenKullaniciId = table.Column<int>(type: "int", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kazanc_kesinti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kazanc_kesinti_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_kazanc_kesinti_kazanc_kesinti_turu_KazancKesintiTuruId",
                        column: x => x.KazancKesintiTuruId,
                        principalTable: "kazanc_kesinti_turu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_kazanc_kesinti_personel_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "personel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_personel_BirimId",
                table: "personel",
                column: "BirimId");

            migrationBuilder.CreateIndex(
                name: "IX_personel_BolgeId",
                table: "personel",
                column: "BolgeId");

            migrationBuilder.CreateIndex(
                name: "IX_personel_FirmaId_KartNo",
                table: "personel",
                columns: new[] { "FirmaId", "KartNo" });

            migrationBuilder.CreateIndex(
                name: "IX_personel_MeslekId",
                table: "personel",
                column: "MeslekId");

            migrationBuilder.CreateIndex(
                name: "IX_personel_ServisId",
                table: "personel",
                column: "ServisId");

            migrationBuilder.CreateIndex(
                name: "IX_personel_TakimId",
                table: "personel",
                column: "TakimId");

            migrationBuilder.CreateIndex(
                name: "IX_birim_FirmaId",
                table: "birim",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_birim_FirmaId_Ad",
                table: "birim",
                columns: new[] { "FirmaId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_bolge_FirmaId",
                table: "bolge",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_bolge_FirmaId_Ad",
                table: "bolge",
                columns: new[] { "FirmaId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_islem_kaydi_FirmaId",
                table: "islem_kaydi",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_islem_kaydi_FirmaId_Zaman",
                table: "islem_kaydi",
                columns: new[] { "FirmaId", "Zaman" });

            migrationBuilder.CreateIndex(
                name: "IX_islem_kaydi_Tablo_KayitId",
                table: "islem_kaydi",
                columns: new[] { "Tablo", "KayitId" });

            migrationBuilder.CreateIndex(
                name: "IX_kazanc_kesinti_FirmaId",
                table: "kazanc_kesinti",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_kazanc_kesinti_KazancKesintiTuruId",
                table: "kazanc_kesinti",
                column: "KazancKesintiTuruId");

            migrationBuilder.CreateIndex(
                name: "IX_kazanc_kesinti_PersonelId_Tarih",
                table: "kazanc_kesinti",
                columns: new[] { "PersonelId", "Tarih" });

            migrationBuilder.CreateIndex(
                name: "IX_kazanc_kesinti_turu_FirmaId",
                table: "kazanc_kesinti_turu",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_kazanc_kesinti_turu_FirmaId_Ad",
                table: "kazanc_kesinti_turu",
                columns: new[] { "FirmaId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_meslek_FirmaId",
                table: "meslek",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_meslek_FirmaId_Ad",
                table: "meslek",
                columns: new[] { "FirmaId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_servis_FirmaId",
                table: "servis",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_servis_FirmaId_Ad",
                table: "servis",
                columns: new[] { "FirmaId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_takim_FirmaId",
                table: "takim",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_takim_FirmaId_Ad",
                table: "takim",
                columns: new[] { "FirmaId", "Ad" });

            migrationBuilder.AddForeignKey(
                name: "FK_personel_birim_BirimId",
                table: "personel",
                column: "BirimId",
                principalTable: "birim",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_personel_bolge_BolgeId",
                table: "personel",
                column: "BolgeId",
                principalTable: "bolge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_personel_meslek_MeslekId",
                table: "personel",
                column: "MeslekId",
                principalTable: "meslek",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_personel_servis_ServisId",
                table: "personel",
                column: "ServisId",
                principalTable: "servis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_personel_takim_TakimId",
                table: "personel",
                column: "TakimId",
                principalTable: "takim",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_personel_birim_BirimId",
                table: "personel");

            migrationBuilder.DropForeignKey(
                name: "FK_personel_bolge_BolgeId",
                table: "personel");

            migrationBuilder.DropForeignKey(
                name: "FK_personel_meslek_MeslekId",
                table: "personel");

            migrationBuilder.DropForeignKey(
                name: "FK_personel_servis_ServisId",
                table: "personel");

            migrationBuilder.DropForeignKey(
                name: "FK_personel_takim_TakimId",
                table: "personel");

            migrationBuilder.DropTable(
                name: "birim");

            migrationBuilder.DropTable(
                name: "bolge");

            migrationBuilder.DropTable(
                name: "islem_kaydi");

            migrationBuilder.DropTable(
                name: "kazanc_kesinti");

            migrationBuilder.DropTable(
                name: "meslek");

            migrationBuilder.DropTable(
                name: "servis");

            migrationBuilder.DropTable(
                name: "takim");

            migrationBuilder.DropTable(
                name: "kazanc_kesinti_turu");

            migrationBuilder.DropIndex(
                name: "IX_personel_BirimId",
                table: "personel");

            migrationBuilder.DropIndex(
                name: "IX_personel_BolgeId",
                table: "personel");

            migrationBuilder.DropIndex(
                name: "IX_personel_FirmaId_KartNo",
                table: "personel");

            migrationBuilder.DropIndex(
                name: "IX_personel_MeslekId",
                table: "personel");

            migrationBuilder.DropIndex(
                name: "IX_personel_ServisId",
                table: "personel");

            migrationBuilder.DropIndex(
                name: "IX_personel_TakimId",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "AnneAdi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "AskerlikDurumu",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "BabaAdi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "BankaAdi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "BirimId",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "BolgeId",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "CocukSayisi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "DevamsizlikCezaKatsayisi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "DogumYeri",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "EgitimDurumu",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "EhliyetNo",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "EhliyetSinifi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "EskiUcret",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "EvTelefonu",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "FazlaMesaiAlir",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "Iban",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "Il",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "Ilce",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "IlkIseBaslamaTarihi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "IlkSoyad",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "KanGrubu",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "KartId",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "MedeniHal",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "MesaiSaatUcreti",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "MeslekId",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "NufusIl",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "NufusIlce",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "ServisId",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "SgkGirisTarihi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "SgkSicilNo",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "TakimId",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "Ucret",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "UcretTipi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "Uyruk",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "VergiNo",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "FazlaMesaiCarpani",
                table: "calisma_grubu");

            migrationBuilder.DropColumn(
                name: "MaasBoleniSaat",
                table: "calisma_grubu");

            migrationBuilder.DropColumn(
                name: "TatilMesaisiCarpani",
                table: "calisma_grubu");

            migrationBuilder.CreateIndex(
                name: "IX_personel_FirmaId_KartNo",
                table: "personel",
                columns: new[] { "FirmaId", "KartNo" },
                unique: true);
        }
    }
}
