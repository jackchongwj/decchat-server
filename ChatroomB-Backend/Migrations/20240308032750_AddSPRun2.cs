using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatroomB_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSPRun2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ExecuteSqlFiles(migrationBuilder);
        }

        private void ExecuteSqlFiles(MigrationBuilder migrationBuilder)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "StoredProcedure");

            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"SQL scripts folder not found: {folderPath}");
            }

            string[] filePaths = Directory.GetFiles(folderPath, "*.sql");
            foreach (string filePath in filePaths)
            {
                // Read the content of the SQL file
                string sql = File.ReadAllText(filePath);

                // Execute the SQL script
                migrationBuilder.Sql(sql);
            }

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
