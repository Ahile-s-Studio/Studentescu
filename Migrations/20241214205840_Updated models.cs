using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studentescu.Migrations
{
    /// <inheritdoc />
    public partial class Updatedmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberInGroup_AspNetUsers_UserId",
                table: "MemberInGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberInGroup_UserGroups_UserGroupId",
                table: "MemberInGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_UserGroups_UserGroupId",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberInGroup",
                table: "MemberInGroup");

            migrationBuilder.RenameTable(
                name: "MemberInGroup",
                newName: "MemberInGroups");

            migrationBuilder.RenameColumn(
                name: "UserGroupId",
                table: "Posts",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserGroupId",
                table: "Posts",
                newName: "IX_Posts_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberInGroup_UserId",
                table: "MemberInGroups",
                newName: "IX_MemberInGroups_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberInGroup_UserGroupId",
                table: "MemberInGroups",
                newName: "IX_MemberInGroups_UserGroupId");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "UserGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberInGroups",
                table: "MemberInGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInGroups_AspNetUsers_UserId",
                table: "MemberInGroups",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInGroups_UserGroups_UserGroupId",
                table: "MemberInGroups",
                column: "UserGroupId",
                principalTable: "UserGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_UserGroups_GroupId",
                table: "Posts",
                column: "GroupId",
                principalTable: "UserGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberInGroups_AspNetUsers_UserId",
                table: "MemberInGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberInGroups_UserGroups_UserGroupId",
                table: "MemberInGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_UserGroups_GroupId",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberInGroups",
                table: "MemberInGroups");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "UserGroups");

            migrationBuilder.RenameTable(
                name: "MemberInGroups",
                newName: "MemberInGroup");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Posts",
                newName: "UserGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_GroupId",
                table: "Posts",
                newName: "IX_Posts_UserGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberInGroups_UserId",
                table: "MemberInGroup",
                newName: "IX_MemberInGroup_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberInGroups_UserGroupId",
                table: "MemberInGroup",
                newName: "IX_MemberInGroup_UserGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberInGroup",
                table: "MemberInGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInGroup_AspNetUsers_UserId",
                table: "MemberInGroup",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInGroup_UserGroups_UserGroupId",
                table: "MemberInGroup",
                column: "UserGroupId",
                principalTable: "UserGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_UserGroups_UserGroupId",
                table: "Posts",
                column: "UserGroupId",
                principalTable: "UserGroups",
                principalColumn: "Id");
        }
    }
}
