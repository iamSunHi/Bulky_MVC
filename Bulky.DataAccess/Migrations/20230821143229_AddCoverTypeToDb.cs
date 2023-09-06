using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverTypeToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoverTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoverTypes", x => x.Id);
                });

			migrationBuilder.InsertData(
				table: "CoverTypes",
				columns: new[] { "Id", "Name" },
				values: new object[,]
				{
					{ 1, "Hardcover" },
                    { 2, "Paperback" },
                    { 3, "Leatherbound" },
                    { 4, "Spiral-bound" },
                    { 5, "Dust Jacket" },
                    { 6, "Flexibound" },
                    { 7, "Casebound" },
                    { 8, "Board Book" },
                    { 9, "Cloth-bound" },
                    { 10, "Matte Finish" },
                    { 11, "Glossy Finish" }
				});
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoverTypes");
        }
    }
}
