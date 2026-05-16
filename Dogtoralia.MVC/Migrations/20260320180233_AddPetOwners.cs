using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dogtoralia.MVC.Migrations
{
    /// <inheritdoc />
    public partial class AddPetOwners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PetOwnerId",
                table: "Pets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PetOwners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetOwners", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PetOwners",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "jorge.sanchez@gmail.com", "Jorge Sánchez", "+52-55-1234-5601" },
                    { 2, new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "maria.fernandez@outlook.com", "María Fernández", "+52-55-1234-5602" },
                    { 3, new DateTime(2024, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "andres.gomez@yahoo.com", "Andrés Gómez", "+52-33-9876-5603" },
                    { 4, new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "patricia.lopez@gmail.com", "Patricia López", "+52-81-5555-5604" },
                    { 5, new DateTime(2024, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "roberto.diaz@hotmail.com", "Roberto Díaz", "+52-55-1234-5605" },
                    { 6, new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "lucia.herrera@gmail.com", "Lucía Herrera", "+52-22-7777-5606" },
                    { 7, new DateTime(2024, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "eduardo.torres@gmail.com", "Eduardo Torres", "+52-55-1234-5607" },
                    { 8, new DateTime(2024, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "gabriela.ruiz@outlook.com", "Gabriela Ruiz", "+52-33-3333-5608" },
                    { 9, new DateTime(2024, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "felipe.morales@yahoo.com", "Felipe Morales", "+52-81-8888-5609" },
                    { 10, new DateTime(2024, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "isabella.castro@gmail.com", "Isabella Castro", "+52-55-1234-5610" }
                });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "PetOwnerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 2,
                column: "PetOwnerId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 3,
                column: "PetOwnerId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 4,
                column: "PetOwnerId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 5,
                column: "PetOwnerId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 6,
                column: "PetOwnerId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 7,
                column: "PetOwnerId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 8,
                column: "PetOwnerId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 9,
                column: "PetOwnerId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 10,
                column: "PetOwnerId",
                value: 10);

            migrationBuilder.CreateIndex(
                name: "IX_Pets_PetOwnerId",
                table: "Pets",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PetOwners_Email",
                table: "PetOwners",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_PetOwners_PetOwnerId",
                table: "Pets",
                column: "PetOwnerId",
                principalTable: "PetOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_PetOwners_PetOwnerId",
                table: "Pets");

            migrationBuilder.DropTable(
                name: "PetOwners");

            migrationBuilder.DropIndex(
                name: "IX_Pets_PetOwnerId",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "PetOwnerId",
                table: "Pets");
        }
    }
}
