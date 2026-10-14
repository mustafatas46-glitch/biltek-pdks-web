using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiltekPdks.Infrastructure.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class GunlukParametreler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CokluGiriseEksikYazma",
                table: "vardiya",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DevamsizlikCezaDakika",
                table: "vardiya",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EksikCezaKatsayisi",
                table: "vardiya",
                type: "TEXT",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ErkenCezaKatsayisi",
                table: "vardiya",
                type: "TEXT",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GecCezaKatsayisi",
                table: "vardiya",
                type: "TEXT",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MaxToleransDakika",
                table: "vardiya",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "NormalTamamlaSonraFazlaMesai",
                table: "vardiya",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "vardiya_mola",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VardiyaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Baslangic = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    Bitis = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    Ucretli = table.Column<bool>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FirmaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vardiya_mola", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vardiya_mola_firma_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "firma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vardiya_mola_vardiya_VardiyaId",
                        column: x => x.VardiyaId,
                        principalTable: "vardiya",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vardiya_mola_FirmaId",
                table: "vardiya_mola",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_vardiya_mola_VardiyaId",
                table: "vardiya_mola",
                column: "VardiyaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vardiya_mola");

            migrationBuilder.DropColumn(
                name: "CokluGiriseEksikYazma",
                table: "vardiya");

            migrationBuilder.DropColumn(
                name: "DevamsizlikCezaDakika",
                table: "vardiya");

            migrationBuilder.DropColumn(
                name: "EksikCezaKatsayisi",
                table: "vardiya");

            migrationBuilder.DropColumn(
                name: "ErkenCezaKatsayisi",
                table: "vardiya");

            migrationBuilder.DropColumn(
                name: "GecCezaKatsayisi",
                table: "vardiya");

            migrationBuilder.DropColumn(
                name: "MaxToleransDakika",
                table: "vardiya");

            migrationBuilder.DropColumn(
                name: "NormalTamamlaSonraFazlaMesai",
                table: "vardiya");
        }
    }
}
