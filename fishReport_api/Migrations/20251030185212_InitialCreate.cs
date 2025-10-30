using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FishHouseApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FishMarkets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FishMarkets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Habitat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Length = table.Column<int>(type: "int", nullable: false),
                    Population = table.Column<int>(type: "int", nullable: true),
                    Lifespan = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FishMarketInventory",
                columns: table => new
                {
                    FishMarketId = table.Column<int>(type: "int", nullable: false),
                    SpeciesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FishMarketInventory", x => new { x.FishMarketId, x.SpeciesId });
                    table.ForeignKey(
                        name: "FK_FishMarketInventory_FishMarkets_FishMarketId",
                        column: x => x.FishMarketId,
                        principalTable: "FishMarkets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FishMarketInventory_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FishMarkets",
                columns: new[] { "Id", "Location", "MarketName" },
                values: new object[,]
                {
                    { 1, "New York, NY", "NY Fish Co." },
                    { 2, "New Bedford, MA", "Tempest" },
                    { 3, "Cape May, NJ", "Lobster House" }
                });

            migrationBuilder.InsertData(
                table: "Species",
                columns: new[] { "Id", "Habitat", "Length", "Lifespan", "Name", "Population", "Price" },
                values: new object[,]
                {
                    { 1, "Open Ocean", 300, 15, "Bluefin Tuna", 25000, 12.99m },
                    { 2, "Tropical Waters", 180, 5, "Mahi Mahi", 75000, 8.50m },
                    { 3, "Deep Sea", 400, 9, "Swordfish", 30000, 14.25m },
                    { 4, "Rivers and Oceans", 150, 8, "Atlantic Salmon", 40000, 9.75m },
                    { 5, "Tropical Oceans", 250, 7, "Yellowfin Tuna", 60000, 11.20m },
                    { 6, "Coral Reefs", 230, 30, "Humphead Wrasse", 12000, 18.00m },
                    { 7, "Coastal Waters", 15, 4, "Anchovy", 1000000, 2.00m },
                    { 8, "Open Ocean", 420, 12, "Marlin", 20000, 13.80m },
                    { 9, "Northern Pacific", 240, 25, "Pacific Halibut", 35000, 10.50m },
                    { 10, "Coral and Rocky Reefs", 170, 10, "Giant Trevally", 18000, 9.90m },
                    { 11, "Warm Coastal Waters", 180, 7, "King Mackerel", 50000, 7.99m },
                    { 12, "Coastal and Estuarine Waters", 200, 50, "Tarpon", 22000, 6.49m },
                    { 13, "Tropical Oceans", 330, 4, "Sailfish", 27000, 15.25m },
                    { 14, "Reefs", 80, 15, "Snapper", 62000, 6.99m },
                    { 15, "Reefs and Rocky Areas", 250, 20, "Groupers", 40000, 10.25m },
                    { 16, "Warm Oceans", 30, 5, "Flying Fish", 85000, 5.50m },
                    { 17, "Shallow Coastal Waters", 60, 7, "Pompano", 30000, 8.25m },
                    { 18, "Rivers and Coastal Waters", 350, 60, "Sturgeon", 15000, 16.50m }
                });

            migrationBuilder.InsertData(
                table: "FishMarketInventory",
                columns: new[] { "FishMarketId", "SpeciesId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 4 },
                    { 1, 6 },
                    { 1, 7 },
                    { 1, 9 },
                    { 1, 10 },
                    { 2, 3 },
                    { 2, 5 },
                    { 2, 8 },
                    { 2, 11 },
                    { 2, 12 },
                    { 2, 14 },
                    { 2, 16 },
                    { 2, 18 },
                    { 3, 4 },
                    { 3, 7 },
                    { 3, 9 },
                    { 3, 13 },
                    { 3, 15 },
                    { 3, 17 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FishMarketInventory_SpeciesId",
                table: "FishMarketInventory",
                column: "SpeciesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FishMarketInventory");

            migrationBuilder.DropTable(
                name: "FishMarkets");

            migrationBuilder.DropTable(
                name: "Species");
        }
    }
}
