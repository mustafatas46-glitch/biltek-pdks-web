using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiltekPdks.Infrastructure.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class SicilKartiAlanlari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "personel",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AileSiraNo",
                table: "personel",
                type: "TEXT",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AyakkabiNo",
                table: "personel",
                type: "TEXT",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Boy",
                table: "personel",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CihazProtez",
                table: "personel",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CiltNo",
                table: "personel",
                type: "TEXT",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CuzdanKayitNo",
                table: "personel",
                type: "TEXT",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CuzdanVerildigiYer",
                table: "personel",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CuzdanVerilisNedeni",
                table: "personel",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CuzdanVerilisTarihi",
                table: "personel",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EhliyetVerildigiYer",
                table: "personel",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EhliyetVerilisTarihi",
                table: "personel",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ElbiseBeden",
                table: "personel",
                type: "TEXT",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gozluk",
                table: "personel",
                type: "TEXT",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsTelefonu",
                table: "personel",
                type: "TEXT",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Kilo",
                table: "personel",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MezunBrans",
                table: "personel",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MezunOkul",
                table: "personel",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "MezuniyetTarihi",
                table: "personel",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NufusMahalleKoy",
                table: "personel",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OkulNo",
                table: "personel",
                type: "TEXT",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OzelGunluk",
                table: "personel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OzelUcret",
                table: "personel",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrimGunluk",
                table: "personel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PrimUcreti",
                table: "personel",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sabika",
                table: "personel",
                type: "TEXT",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiraNo",
                table: "personel",
                type: "TEXT",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WcKesintisiUygula",
                table: "personel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WebAdresi",
                table: "personel",
                type: "TEXT",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YabanciDil",
                table: "personel",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "YemekGunluk",
                table: "personel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "YemekUcreti",
                table: "personel",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "YolGunluk",
                table: "personel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "YolUcreti",
                table: "personel",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "AileSiraNo",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "AyakkabiNo",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "Boy",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "CihazProtez",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "CiltNo",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "CuzdanKayitNo",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "CuzdanVerildigiYer",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "CuzdanVerilisNedeni",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "CuzdanVerilisTarihi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "EhliyetVerildigiYer",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "EhliyetVerilisTarihi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "ElbiseBeden",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "Gozluk",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "IsTelefonu",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "Kilo",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "MezunBrans",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "MezunOkul",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "MezuniyetTarihi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "NufusMahalleKoy",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "OkulNo",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "OzelGunluk",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "OzelUcret",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "PrimGunluk",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "PrimUcreti",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "Sabika",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "SiraNo",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "WcKesintisiUygula",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "WebAdresi",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "YabanciDil",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "YemekGunluk",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "YemekUcreti",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "YolGunluk",
                table: "personel");

            migrationBuilder.DropColumn(
                name: "YolUcreti",
                table: "personel");
        }
    }
}
