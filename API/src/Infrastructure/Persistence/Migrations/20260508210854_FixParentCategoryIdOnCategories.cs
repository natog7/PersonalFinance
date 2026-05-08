using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceAPI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixParentCategoryIdOnCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId1",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentCategoryId1",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId1",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentCategoryId1",
                table: "Categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId1",
                table: "Categories",
                column: "ParentCategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId1",
                table: "Categories",
                column: "ParentCategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
