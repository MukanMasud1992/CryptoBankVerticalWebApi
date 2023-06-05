using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoBankVerticalWebApi.Migrations
{
    /// <inheritdoc />
    public partial class EditFieldInRefreshTokenSecond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NextRefreshToken",
                table: "RefreshTokens",
                newName: "ReplacedByNextToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReplacedByNextToken",
                table: "RefreshTokens",
                newName: "NextRefreshToken");
        }
    }
}
