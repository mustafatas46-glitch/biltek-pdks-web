using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiltekPdks.Infrastructure.Migrations.MySql
{
    /// <inheritdoc />
    public partial class IlkSema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "firma",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Kod = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ad = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VergiDairesi = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VergiNo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SgkSicilNo = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Adres = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Il = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ilce = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefon = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Yetkili = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Aktif = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EskiId = table.Column<int>(type: "int", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_firma", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "bolum",
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
                    table.PrimaryKey("PK_bolum", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bolum_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "calisma_grubu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GunlukCalismaDakika = table.Column<int>(type: "int", nullable: false),
                    DonemlikCalismaDakika = table.Column<int>(type: "int", nullable: false),
                    CumartesiCalisilir = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PazarCalisilir = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                    table.PrimaryKey("PK_calisma_grubu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_calisma_grubu_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "gorev",
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
                    table.PrimaryKey("PK_gorev", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gorev_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "izin_turu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Ucretli = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    YillikIzindenDuser = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                    table.PrimaryKey("PK_izin_turu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_izin_turu_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "resmi_tatil",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tarih = table.Column<DateOnly>(type: "date", nullable: false),
                    Ad = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resmi_tatil", x => x.Id);
                    table.ForeignKey(
                        name: "FK_resmi_tatil_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sube",
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
                    table.PrimaryKey("PK_sube", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sube_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vardiya",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Baslangic = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    Bitis = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    MolaDakika = table.Column<int>(type: "int", nullable: false),
                    GecKalmaToleransDakika = table.Column<int>(type: "int", nullable: false),
                    ErkenCikmaToleransDakika = table.Column<int>(type: "int", nullable: false),
                    FazlaMesaiEsikDakika = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_vardiya", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vardiya_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "personel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KartNo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SicilNo = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ad = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Soyad = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TcKimlikNo = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cinsiyet = table.Column<int>(type: "int", nullable: false),
                    DogumTarihi = table.Column<DateOnly>(type: "date", nullable: true),
                    IseGirisTarihi = table.Column<DateOnly>(type: "date", nullable: false),
                    IstenCikisTarihi = table.Column<DateOnly>(type: "date", nullable: true),
                    IstenAyrilmaNedeni = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CepTelefonu = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Eposta = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Adres = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubeId = table.Column<int>(type: "int", nullable: true),
                    BolumId = table.Column<int>(type: "int", nullable: true),
                    GorevId = table.Column<int>(type: "int", nullable: true),
                    CalismaGrubuId = table.Column<int>(type: "int", nullable: true),
                    EskiId = table.Column<int>(type: "int", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_personel_bolum_BolumId",
                        column: x => x.BolumId,
                        principalTable: "bolum",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_personel_calisma_grubu_CalismaGrubuId",
                        column: x => x.CalismaGrubuId,
                        principalTable: "calisma_grubu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_personel_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_personel_gorev_GorevId",
                        column: x => x.GorevId,
                        principalTable: "gorev",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_personel_sube_SubeId",
                        column: x => x.SubeId,
                        principalTable: "sube",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "calisma_grubu_gunu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CalismaGrubuId = table.Column<int>(type: "int", nullable: false),
                    Gun = table.Column<int>(type: "int", nullable: false),
                    VardiyaId = table.Column<int>(type: "int", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calisma_grubu_gunu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_calisma_grubu_gunu_calisma_grubu_CalismaGrubuId",
                        column: x => x.CalismaGrubuId,
                        principalTable: "calisma_grubu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_calisma_grubu_gunu_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_calisma_grubu_gunu_vardiya_VardiyaId",
                        column: x => x.VardiyaId,
                        principalTable: "vardiya",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "hareket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    Zaman = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Kaynak = table.Column<int>(type: "int", nullable: false),
                    TerminalKodu = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Aciklama = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Iptal = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    KaydedenKullaniciId = table.Column<int>(type: "int", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hareket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_hareket_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_hareket_personel_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "personel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "izin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    IzinTuruId = table.Column<int>(type: "int", nullable: false),
                    Baslangic = table.Column<DateOnly>(type: "date", nullable: false),
                    Bitis = table.Column<DateOnly>(type: "date", nullable: false),
                    Aciklama = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_izin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_izin_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_izin_izin_turu_IzinTuruId",
                        column: x => x.IzinTuruId,
                        principalTable: "izin_turu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_izin_personel_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "personel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "kullanici",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KullaniciAdi = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SifreHash = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdSoyad = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    Aktif = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SonGirisTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    BasarisizGirisSayisi = table.Column<int>(type: "int", nullable: false),
                    KilitBitis = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PersonelId = table.Column<int>(type: "int", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kullanici", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kullanici_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_kullanici_personel_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "personel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "personel_vardiya",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateOnly>(type: "date", nullable: false),
                    VardiyaId = table.Column<int>(type: "int", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirmaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personel_vardiya", x => x.Id);
                    table.ForeignKey(
                        name: "FK_personel_vardiya_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_personel_vardiya_personel_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "personel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_personel_vardiya_vardiya_VardiyaId",
                        column: x => x.VardiyaId,
                        principalTable: "vardiya",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_bolum_FirmaId",
                table: "bolum",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_bolum_FirmaId_Ad",
                table: "bolum",
                columns: new[] { "FirmaId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_calisma_grubu_FirmaId",
                table: "calisma_grubu",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_calisma_grubu_FirmaId_Ad",
                table: "calisma_grubu",
                columns: new[] { "FirmaId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_calisma_grubu_gunu_CalismaGrubuId_Gun",
                table: "calisma_grubu_gunu",
                columns: new[] { "CalismaGrubuId", "Gun" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_calisma_grubu_gunu_FirmaId",
                table: "calisma_grubu_gunu",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_calisma_grubu_gunu_VardiyaId",
                table: "calisma_grubu_gunu",
                column: "VardiyaId");

            migrationBuilder.CreateIndex(
                name: "IX_firma_Kod",
                table: "firma",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gorev_FirmaId",
                table: "gorev",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_gorev_FirmaId_Ad",
                table: "gorev",
                columns: new[] { "FirmaId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_hareket_FirmaId",
                table: "hareket",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_hareket_PersonelId_Zaman",
                table: "hareket",
                columns: new[] { "PersonelId", "Zaman" });

            migrationBuilder.CreateIndex(
                name: "IX_izin_FirmaId",
                table: "izin",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_izin_IzinTuruId",
                table: "izin",
                column: "IzinTuruId");

            migrationBuilder.CreateIndex(
                name: "IX_izin_PersonelId_Baslangic",
                table: "izin",
                columns: new[] { "PersonelId", "Baslangic" });

            migrationBuilder.CreateIndex(
                name: "IX_izin_turu_FirmaId",
                table: "izin_turu",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_izin_turu_FirmaId_Ad",
                table: "izin_turu",
                columns: new[] { "FirmaId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_kullanici_FirmaId",
                table: "kullanici",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_kullanici_FirmaId_KullaniciAdi",
                table: "kullanici",
                columns: new[] { "FirmaId", "KullaniciAdi" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_kullanici_PersonelId",
                table: "kullanici",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_personel_BolumId",
                table: "personel",
                column: "BolumId");

            migrationBuilder.CreateIndex(
                name: "IX_personel_CalismaGrubuId",
                table: "personel",
                column: "CalismaGrubuId");

            migrationBuilder.CreateIndex(
                name: "IX_personel_FirmaId",
                table: "personel",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_personel_FirmaId_KartNo",
                table: "personel",
                columns: new[] { "FirmaId", "KartNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_personel_FirmaId_Soyad_Ad",
                table: "personel",
                columns: new[] { "FirmaId", "Soyad", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_personel_GorevId",
                table: "personel",
                column: "GorevId");

            migrationBuilder.CreateIndex(
                name: "IX_personel_SubeId",
                table: "personel",
                column: "SubeId");

            migrationBuilder.CreateIndex(
                name: "IX_personel_vardiya_FirmaId",
                table: "personel_vardiya",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_personel_vardiya_PersonelId_Tarih",
                table: "personel_vardiya",
                columns: new[] { "PersonelId", "Tarih" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_personel_vardiya_VardiyaId",
                table: "personel_vardiya",
                column: "VardiyaId");

            migrationBuilder.CreateIndex(
                name: "IX_resmi_tatil_FirmaId",
                table: "resmi_tatil",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_resmi_tatil_FirmaId_Tarih",
                table: "resmi_tatil",
                columns: new[] { "FirmaId", "Tarih" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sube_FirmaId",
                table: "sube",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_sube_FirmaId_Ad",
                table: "sube",
                columns: new[] { "FirmaId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_vardiya_FirmaId",
                table: "vardiya",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_vardiya_FirmaId_Ad",
                table: "vardiya",
                columns: new[] { "FirmaId", "Ad" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "calisma_grubu_gunu");

            migrationBuilder.DropTable(
                name: "hareket");

            migrationBuilder.DropTable(
                name: "izin");

            migrationBuilder.DropTable(
                name: "kullanici");

            migrationBuilder.DropTable(
                name: "personel_vardiya");

            migrationBuilder.DropTable(
                name: "resmi_tatil");

            migrationBuilder.DropTable(
                name: "izin_turu");

            migrationBuilder.DropTable(
                name: "personel");

            migrationBuilder.DropTable(
                name: "vardiya");

            migrationBuilder.DropTable(
                name: "bolum");

            migrationBuilder.DropTable(
                name: "calisma_grubu");

            migrationBuilder.DropTable(
                name: "gorev");

            migrationBuilder.DropTable(
                name: "sube");

            migrationBuilder.DropTable(
                name: "firma");
        }
    }
}
