using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FONEXPO2024.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Korekcije : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prijave_PromoKodovi_PromoKodID",
                table: "Prijave");

            migrationBuilder.AlterColumn<string>(
                name: "StatusPrijave",
                table: "Prijave",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Aktivna",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatumPrijave",
                table: "Prijave",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "DodatneInfo",
                table: "Manifestacije",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Profesija",
                table: "Korisnici",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "EmailPotvrdjen",
                table: "Korisnici",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Adresa2",
                table: "Korisnici",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Prijave_PromoKodovi_PromoKodID",
                table: "Prijave",
                column: "PromoKodID",
                principalTable: "PromoKodovi",
                principalColumn: "PromoKodID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prijave_PromoKodovi_PromoKodID",
                table: "Prijave");

            migrationBuilder.AlterColumn<string>(
                name: "StatusPrijave",
                table: "Prijave",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Aktivna");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatumPrijave",
                table: "Prijave",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "DodatneInfo",
                table: "Manifestacije",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Profesija",
                table: "Korisnici",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EmailPotvrdjen",
                table: "Korisnici",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Adresa2",
                table: "Korisnici",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Prijave_PromoKodovi_PromoKodID",
                table: "Prijave",
                column: "PromoKodID",
                principalTable: "PromoKodovi",
                principalColumn: "PromoKodID");
        }
    }
}
