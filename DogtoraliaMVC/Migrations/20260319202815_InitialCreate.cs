using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DogtoraliaMVC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Species = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Breed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OwnerEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OwnerPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specialities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clinics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SpecialityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clinics_Specialities_SpecialityId",
                        column: x => x.SpecialityId,
                        principalTable: "Specialities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Veterinarians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    ClinicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veterinarians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veterinarians_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Pets",
                columns: new[] { "Id", "Breed", "CreatedAt", "DateOfBirth", "Name", "Notes", "OwnerEmail", "OwnerName", "OwnerPhone", "Species" },
                values: new object[,]
                {
                    { 1, "Labrador Retriever", new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Max", "Alérgico al pollo.", "jorge.sanchez@gmail.com", "Jorge Sánchez", "+52-55-1234-5601", "Dog" },
                    { 2, "Siamese", new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Luna", null, "maria.fernandez@outlook.com", "María Fernández", "+52-55-1234-5602", "Cat" },
                    { 3, "African Grey Parrot", new DateTime(2024, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2018, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Paco", "Habla español.", "andres.gomez@yahoo.com", "Andrés Gómez", "+52-33-9876-5603", "Bird" },
                    { 4, "Golden Retriever", new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2019, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bella", null, "patricia.lopez@gmail.com", "Patricia López", "+52-81-5555-5604", "Dog" },
                    { 5, "Persian", new DateTime(2024, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mimi", "Pelo largo, requiere grooming mensual.", "roberto.diaz@hotmail.com", "Roberto Díaz", "+52-55-1234-5605", "Cat" },
                    { 6, "German Shepherd", new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rocky", null, "lucia.herrera@gmail.com", "Lucía Herrera", "+52-22-7777-5606", "Dog" },
                    { 7, "Cockatiel", new DateTime(2024, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Coco", "Muy sociable.", "eduardo.torres@gmail.com", "Eduardo Torres", "+52-55-1234-5607", "Bird" },
                    { 8, "Maine Coon", new DateTime(2024, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 6, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nala", null, "gabriela.ruiz@outlook.com", "Gabriela Ruiz", "+52-33-3333-5608", "Cat" },
                    { 9, "Holland Lop", new DateTime(2024, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tito", "Dieta especial sin pellets.", "felipe.morales@yahoo.com", "Felipe Morales", "+52-81-8888-5609", "Rabbit" },
                    { 10, "Beagle", new DateTime(2024, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kira", null, "isabella.castro@gmail.com", "Isabella Castro", "+52-55-1234-5610", "Dog" }
                });

            migrationBuilder.InsertData(
                table: "Specialities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "General Practice" },
                    { 2, "Dermatology" },
                    { 3, "Orthopedics" },
                    { 4, "Cardiology" },
                    { 5, "Oncology" }
                });

            migrationBuilder.InsertData(
                table: "Clinics",
                columns: new[] { "Id", "Address", "CreatedAt", "Description", "Email", "Name", "Phone", "SpecialityId", "Website" },
                values: new object[,]
                {
                    { 1, "Av. Insurgentes Sur 1234, San Ángel, CDMX", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Clínica de medicina general para mascotas en San Ángel.", "contacto@sanangelvет.mx", "Clínica Veterinaria San Ángel", "+52-55-5550-1001", 1, "https://sanangelvет.mx" },
                    { 2, "Presidente Masaryk 321, Polanco, CDMX", new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Especialistas en dermatología veterinaria.", "info@dermapetpolanco.mx", "DermaPet Polanco", "+52-55-5550-2002", 2, "https://dermapetpolanco.mx" },
                    { 3, "Av. Vallarta 4560, Guadalajara, Jalisco", new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cirugía y rehabilitación ortopédica para animales.", "ortopaws@gdl.mx", "OrtoPaws Guadalajara", "+52-33-3333-3003", 3, null },
                    { 4, "Av. Garza Sada 2501, Monterrey, NL", new DateTime(2024, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cardiología veterinaria de alta especialidad.", "cardiovet@mty.mx", "CardioVet Monterrey", "+52-81-8181-4004", 4, "https://cardiovetmty.mx" },
                    { 5, "Blvd. Atlixcáyotl 2000, Puebla, PUE", new DateTime(2024, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Centro oncológico veterinario en Puebla.", "oncoanimal@puebla.mx", "OncoAnimal Puebla", "+52-22-2222-5005", 5, null },
                    { 6, "Francisco Sosa 150, Coyoacán, CDMX", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Atención integral y medicina preventiva para mascotas.", "integral@coyoacanvet.mx", "Clínica Integral Coyoacán", "+52-55-5550-6006", 1, "https://coyoacanvet.mx" }
                });

            migrationBuilder.InsertData(
                table: "Veterinarians",
                columns: new[] { "Id", "ClinicId", "Email", "FirstName", "LastName", "LicenseNumber", "Phone", "YearsOfExperience" },
                values: new object[,]
                {
                    { 1, 1, "c.mendoza@sanangelvет.mx", "Carlos", "Mendoza Ruiz", "MV-100001", "+52-55-5550-1101", 10 },
                    { 2, 1, "l.garcia@sanangelvет.mx", "Laura", "García Pérez", "MV-100002", "+52-55-5550-1102", 7 },
                    { 3, 2, "m.torres@dermapetpolanco.mx", "Miguel", "Torres Sánchez", "MV-100003", "+52-55-5550-2101", 12 },
                    { 4, 2, "a.lopez@dermapetpolanco.mx", "Ana", "López Hernández", "MV-100004", "+52-55-5550-2102", 5 },
                    { 5, 3, "r.castillo@gdl.mx", "Roberto", "Castillo Vega", "MV-100005", "+52-33-3333-3101", 15 },
                    { 6, 3, "s.ramirez@gdl.mx", "Sofia", "Ramírez Díaz", "MV-100006", "+52-33-3333-3102", 8 },
                    { 7, 4, "d.morales@mty.mx", "Diego", "Morales Cruz", "MV-100007", "+52-81-8181-4101", 20 },
                    { 8, 4, "v.jimenez@mty.mx", "Valeria", "Jiménez Flores", "MV-100008", "+52-81-8181-4102", 6 },
                    { 9, 5, "h.reyes@puebla.mx", "Héctor", "Reyes Martínez", "MV-100009", "+52-22-2222-5101", 18 },
                    { 10, 6, "c.navarro@coyoacanvet.mx", "Carmen", "Navarro Ortiz", "MV-100010", "+52-55-5550-6101", 9 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_SpecialityId",
                table: "Clinics",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Veterinarians_ClinicId",
                table: "Veterinarians",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Veterinarians_LicenseNumber",
                table: "Veterinarians",
                column: "LicenseNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "Veterinarians");

            migrationBuilder.DropTable(
                name: "Clinics");

            migrationBuilder.DropTable(
                name: "Specialities");
        }
    }
}
