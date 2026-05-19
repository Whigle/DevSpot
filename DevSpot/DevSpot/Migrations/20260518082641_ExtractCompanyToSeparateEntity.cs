using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevSpot.Migrations
{
    /// <inheritdoc />
    public partial class ExtractCompanyToSeparateEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Companies table first
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            // Insert unique company names from JobPostings into Companies table
            migrationBuilder.Sql(@"
                INSERT INTO Companies (Name, Description, WebsiteUrl)
                SELECT DISTINCT Company, NULL, NULL
                FROM JobPostings
                WHERE Company IS NOT NULL AND Company <> ''
            ");

            // Add CompanyId column to JobPostings (nullable temporarily)
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "JobPostings",
                type: "int",
                nullable: true);

            // Update JobPostings to link to Companies based on company name
            migrationBuilder.Sql(@"
                UPDATE jp
                SET CompanyId = c.Id
                FROM JobPostings jp
                INNER JOIN Companies c ON jp.Company = c.Name
            ");

            // Make CompanyId NOT NULL after data migration
            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "JobPostings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Create index and foreign key
            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_CompanyId",
                table: "JobPostings",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_Companies_CompanyId",
                table: "JobPostings",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Drop the old Company column
            migrationBuilder.DropColumn(
                name: "Company",
                table: "JobPostings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_Companies_CompanyId",
                table: "JobPostings");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_CompanyId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "JobPostings");

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "JobPostings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
