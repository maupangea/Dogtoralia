using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DogtoraliaMVC.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateOwnerFieldsFromPets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_PetOwners_PetOwnerId",
                table: "Pets");

            // Remove pets that have no valid PetOwner so the NOT NULL + FK constraint can be applied cleanly
            migrationBuilder.Sql("DELETE FROM [Pets] WHERE [PetOwnerId] IS NULL OR [PetOwnerId] NOT IN (SELECT [Id] FROM [PetOwners])");

            migrationBuilder.DropColumn(
                name: "OwnerEmail",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "OwnerPhone",
                table: "Pets");

            migrationBuilder.AlterColumn<int>(
                name: "PetOwnerId",
                table: "Pets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_PetOwners_PetOwnerId",
                table: "Pets",
                column: "PetOwnerId",
                principalTable: "PetOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_PetOwners_PetOwnerId",
                table: "Pets");

            migrationBuilder.AlterColumn<int>(
                name: "PetOwnerId",
                table: "Pets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "OwnerEmail",
                table: "Pets",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "Pets",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnerPhone",
                table: "Pets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "OwnerEmail", "OwnerName", "OwnerPhone" },
                values: new object[] { "jorge.sanchez@gmail.com", "Jorge Sánchez", "+52-55-1234-5601" });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "OwnerEmail", "OwnerName", "OwnerPhone" },
                values: new object[] { "maria.fernandez@outlook.com", "María Fernández", "+52-55-1234-5602" });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "OwnerEmail", "OwnerName", "OwnerPhone" },
                values: new object[] { "andres.gomez@yahoo.com", "Andrés Gómez", "+52-33-9876-5603" });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "OwnerEmail", "OwnerName", "OwnerPhone" },
                values: new object[] { "patricia.lopez@gmail.com", "Patricia López", "+52-81-5555-5604" });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "OwnerEmail", "OwnerName", "OwnerPhone" },
                values: new object[] { "roberto.diaz@hotmail.com", "Roberto Díaz", "+52-55-1234-5605" });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "OwnerEmail", "OwnerName", "OwnerPhone" },
                values: new object[] { "lucia.herrera@gmail.com", "Lucía Herrera", "+52-22-7777-5606" });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "OwnerEmail", "OwnerName", "OwnerPhone" },
                values: new object[] { "eduardo.torres@gmail.com", "Eduardo Torres", "+52-55-1234-5607" });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "OwnerEmail", "OwnerName", "OwnerPhone" },
                values: new object[] { "gabriela.ruiz@outlook.com", "Gabriela Ruiz", "+52-33-3333-5608" });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "OwnerEmail", "OwnerName", "OwnerPhone" },
                values: new object[] { "felipe.morales@yahoo.com", "Felipe Morales", "+52-81-8888-5609" });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "OwnerEmail", "OwnerName", "OwnerPhone" },
                values: new object[] { "isabella.castro@gmail.com", "Isabella Castro", "+52-55-1234-5610" });

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_PetOwners_PetOwnerId",
                table: "Pets",
                column: "PetOwnerId",
                principalTable: "PetOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
