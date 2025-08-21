using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FONEXPO2024.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    KorisnikID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Profesija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostanskiBroj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mesto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Drzava = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmailPotvrdjen = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.KorisnikID);
                });

            migrationBuilder.CreateTable(
                name: "Manifestacije",
                columns: table => new
                {
                    ManifestacijaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lokacija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumPocetka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumZavrsetka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DodatneInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxPosetilacaPoDanu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manifestacije", x => x.ManifestacijaID);
                });

            migrationBuilder.CreateTable(
                name: "ExpoDani",
                columns: table => new
                {
                    ExpoDanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManifestacijaID = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tema = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpoDani", x => x.ExpoDanID);
                    table.ForeignKey(
                        name: "FK_ExpoDani_Manifestacije_ManifestacijaID",
                        column: x => x.ManifestacijaID,
                        principalTable: "Manifestacije",
                        principalColumn: "ManifestacijaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CeneDana",
                columns: table => new
                {
                    CenaDanaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpoDanID = table.Column<int>(type: "int", nullable: false),
                    Cena = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CeneDana", x => x.CenaDanaID);
                    table.ForeignKey(
                        name: "FK_CeneDana_ExpoDani_ExpoDanID",
                        column: x => x.ExpoDanID,
                        principalTable: "ExpoDani",
                        principalColumn: "ExpoDanID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Izlozbe",
                columns: table => new
                {
                    IzlozbaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpoDanID = table.Column<int>(type: "int", nullable: false),
                    VremeOtvaranja = table.Column<TimeSpan>(type: "time", nullable: false),
                    VremeZatvaranja = table.Column<TimeSpan>(type: "time", nullable: false),
                    Umetnik = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Izlozbe", x => x.IzlozbaID);
                    table.ForeignKey(
                        name: "FK_Izlozbe_ExpoDani_ExpoDanID",
                        column: x => x.ExpoDanID,
                        principalTable: "ExpoDani",
                        principalColumn: "ExpoDanID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prijave",
                columns: table => new
                {
                    PrijavaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StatusPrijave = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumPrijave = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OriginalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsEarlyBird = table.Column<bool>(type: "bit", nullable: false),
                    BrojOsoba = table.Column<int>(type: "int", nullable: false),
                    KorisnikID = table.Column<int>(type: "int", nullable: false),
                    PromoKodID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prijave", x => x.PrijavaID);
                    table.ForeignKey(
                        name: "FK_Prijave_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrijaveDana",
                columns: table => new
                {
                    PrijavaID = table.Column<int>(type: "int", nullable: false),
                    ExpoDanID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrijaveDana", x => new { x.PrijavaID, x.ExpoDanID });
                    table.ForeignKey(
                        name: "FK_PrijaveDana_ExpoDani_ExpoDanID",
                        column: x => x.ExpoDanID,
                        principalTable: "ExpoDani",
                        principalColumn: "ExpoDanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrijaveDana_Prijave_PrijavaID",
                        column: x => x.PrijavaID,
                        principalTable: "Prijave",
                        principalColumn: "PrijavaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromoKodovi",
                columns: table => new
                {
                    PromoKodID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kod = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    GenerisanOdPrijavaID = table.Column<int>(type: "int", nullable: false),
                    IskoriscenOdPrijavaID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoKodovi", x => x.PromoKodID);
                    table.ForeignKey(
                        name: "FK_PromoKodovi_Prijave_GenerisanOdPrijavaID",
                        column: x => x.GenerisanOdPrijavaID,
                        principalTable: "Prijave",
                        principalColumn: "PrijavaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoKodovi_Prijave_IskoriscenOdPrijavaID",
                        column: x => x.IskoriscenOdPrijavaID,
                        principalTable: "Prijave",
                        principalColumn: "PrijavaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CeneDana_ExpoDanID",
                table: "CeneDana",
                column: "ExpoDanID");

            migrationBuilder.CreateIndex(
                name: "IX_ExpoDani_ManifestacijaID",
                table: "ExpoDani",
                column: "ManifestacijaID");

            migrationBuilder.CreateIndex(
                name: "IX_Izlozbe_ExpoDanID",
                table: "Izlozbe",
                column: "ExpoDanID");

            migrationBuilder.CreateIndex(
                name: "IX_Korisnici_Email",
                table: "Korisnici",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prijave_KorisnikID",
                table: "Prijave",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Prijave_PromoKodID",
                table: "Prijave",
                column: "PromoKodID");

            migrationBuilder.CreateIndex(
                name: "IX_Prijave_Token",
                table: "Prijave",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrijaveDana_ExpoDanID",
                table: "PrijaveDana",
                column: "ExpoDanID");

            migrationBuilder.CreateIndex(
                name: "IX_PromoKodovi_GenerisanOdPrijavaID",
                table: "PromoKodovi",
                column: "GenerisanOdPrijavaID");

            migrationBuilder.CreateIndex(
                name: "IX_PromoKodovi_IskoriscenOdPrijavaID",
                table: "PromoKodovi",
                column: "IskoriscenOdPrijavaID");

            migrationBuilder.CreateIndex(
                name: "IX_PromoKodovi_Kod",
                table: "PromoKodovi",
                column: "Kod",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Prijave_PromoKodovi_PromoKodID",
                table: "Prijave",
                column: "PromoKodID",
                principalTable: "PromoKodovi",
                principalColumn: "PromoKodID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prijave_Korisnici_KorisnikID",
                table: "Prijave");

            migrationBuilder.DropForeignKey(
                name: "FK_Prijave_PromoKodovi_PromoKodID",
                table: "Prijave");

            migrationBuilder.DropTable(
                name: "CeneDana");

            migrationBuilder.DropTable(
                name: "Izlozbe");

            migrationBuilder.DropTable(
                name: "PrijaveDana");

            migrationBuilder.DropTable(
                name: "ExpoDani");

            migrationBuilder.DropTable(
                name: "Manifestacije");

            migrationBuilder.DropTable(
                name: "Korisnici");

            migrationBuilder.DropTable(
                name: "PromoKodovi");

            migrationBuilder.DropTable(
                name: "Prijave");
        }
    }
}
