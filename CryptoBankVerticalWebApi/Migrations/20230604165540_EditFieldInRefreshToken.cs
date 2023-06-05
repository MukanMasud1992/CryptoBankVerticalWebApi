using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoBankVerticalWebApi.Migrations
{
    /// <inheritdoc />
    public partial class EditFieldInRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ForeRunnerToken",
                table: "RefreshTokens",
                newName: "NextRefreshToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NextRefreshToken",
                table: "RefreshTokens",
                newName: "ForeRunnerToken");
        }
    }
}
