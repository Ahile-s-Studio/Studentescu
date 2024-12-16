using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studentescu.Migrations
{
    /// <inheritdoc />
    public partial class PublicProfileDefaultFalse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Comment_ParentId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Post_PostId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Follow_AspNetUsers_FolloweeId",
                table: "Follow");

            migrationBuilder.DropForeignKey(
                name: "FK_Follow_AspNetUsers_FollowerId",
                table: "Follow");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowRequest_AspNetUsers_RequesterId",
                table: "FollowRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowRequest_AspNetUsers_TargetId",
                table: "FollowRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Post_PostId",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberInGroup_AspNetUsers_UserId",
                table: "MemberInGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberInGroup_UserGroup_UserGroupId",
                table: "MemberInGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_AspNetUsers_UserId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_UserGroup_UserGroupId",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberInGroup",
                table: "MemberInGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FollowRequest",
                table: "FollowRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Follow",
                table: "Follow");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Post",
                newName: "Posts");

            migrationBuilder.RenameTable(
                name: "MemberInGroup",
                newName: "GroupMemberships");

            migrationBuilder.RenameTable(
                name: "FollowRequest",
                newName: "FollowRequests");

            migrationBuilder.RenameTable(
                name: "Follow",
                newName: "Follows");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Post_UserId",
                table: "Posts",
                newName: "IX_Posts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_UserGroupId",
                table: "Posts",
                newName: "IX_Posts_UserGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberInGroup_UserId",
                table: "GroupMemberships",
                newName: "IX_GroupMemberships_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberInGroup_UserGroupId",
                table: "GroupMemberships",
                newName: "IX_GroupMemberships_UserGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_FollowRequest_TargetId",
                table: "FollowRequests",
                newName: "IX_FollowRequests_TargetId");

            migrationBuilder.RenameIndex(
                name: "IX_FollowRequest_RequesterId",
                table: "FollowRequests",
                newName: "IX_FollowRequests_RequesterId");

            migrationBuilder.RenameIndex(
                name: "IX_Follow_FollowerId",
                table: "Follows",
                newName: "IX_Follows_FollowerId");

            migrationBuilder.RenameIndex(
                name: "IX_Follow_FolloweeId",
                table: "Follows",
                newName: "IX_Follows_FolloweeId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserId",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_PostId",
                table: "Comments",
                newName: "IX_Comments_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_ParentId",
                table: "Comments",
                newName: "IX_Comments_ParentId");

            migrationBuilder.AlterColumn<bool>(
                name: "Public",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMemberships",
                table: "GroupMemberships",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FollowRequests",
                table: "FollowRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follows",
                table: "Follows",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments",
                column: "ParentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRequests_AspNetUsers_RequesterId",
                table: "FollowRequests",
                column: "RequesterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRequests_AspNetUsers_TargetId",
                table: "FollowRequests",
                column: "TargetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_FolloweeId",
                table: "Follows",
                column: "FolloweeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_FollowerId",
                table: "Follows",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMemberships_AspNetUsers_UserId",
                table: "GroupMemberships",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMemberships_UserGroup_UserGroupId",
                table: "GroupMemberships",
                column: "UserGroupId",
                principalTable: "UserGroup",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Posts_PostId",
                table: "Like",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_UserGroup_UserGroupId",
                table: "Posts",
                column: "UserGroupId",
                principalTable: "UserGroup",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowRequests_AspNetUsers_RequesterId",
                table: "FollowRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowRequests_AspNetUsers_TargetId",
                table: "FollowRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_FolloweeId",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_FollowerId",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMemberships_AspNetUsers_UserId",
                table: "GroupMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMemberships_UserGroup_UserGroupId",
                table: "GroupMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Posts_PostId",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_UserGroup_UserGroupId",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMemberships",
                table: "GroupMemberships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Follows",
                table: "Follows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FollowRequests",
                table: "FollowRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Post");

            migrationBuilder.RenameTable(
                name: "GroupMemberships",
                newName: "MemberInGroup");

            migrationBuilder.RenameTable(
                name: "Follows",
                newName: "Follow");

            migrationBuilder.RenameTable(
                name: "FollowRequests",
                newName: "FollowRequest");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserId",
                table: "Post",
                newName: "IX_Post_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserGroupId",
                table: "Post",
                newName: "IX_Post_UserGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMemberships_UserId",
                table: "MemberInGroup",
                newName: "IX_MemberInGroup_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMemberships_UserGroupId",
                table: "MemberInGroup",
                newName: "IX_MemberInGroup_UserGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_FollowerId",
                table: "Follow",
                newName: "IX_Follow_FollowerId");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_FolloweeId",
                table: "Follow",
                newName: "IX_Follow_FolloweeId");

            migrationBuilder.RenameIndex(
                name: "IX_FollowRequests_TargetId",
                table: "FollowRequest",
                newName: "IX_FollowRequest_TargetId");

            migrationBuilder.RenameIndex(
                name: "IX_FollowRequests_RequesterId",
                table: "FollowRequest",
                newName: "IX_FollowRequest_RequesterId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "Comment",
                newName: "IX_Comment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_PostId",
                table: "Comment",
                newName: "IX_Comment_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ParentId",
                table: "Comment",
                newName: "IX_Comment_ParentId");

            migrationBuilder.AlterColumn<bool>(
                name: "Public",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post",
                table: "Post",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberInGroup",
                table: "MemberInGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follow",
                table: "Follow",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FollowRequest",
                table: "FollowRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Comment_ParentId",
                table: "Comment",
                column: "ParentId",
                principalTable: "Comment",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Post_PostId",
                table: "Comment",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Follow_AspNetUsers_FolloweeId",
                table: "Follow",
                column: "FolloweeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Follow_AspNetUsers_FollowerId",
                table: "Follow",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRequest_AspNetUsers_RequesterId",
                table: "FollowRequest",
                column: "RequesterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRequest_AspNetUsers_TargetId",
                table: "FollowRequest",
                column: "TargetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Post_PostId",
                table: "Like",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInGroup_AspNetUsers_UserId",
                table: "MemberInGroup",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInGroup_UserGroup_UserGroupId",
                table: "MemberInGroup",
                column: "UserGroupId",
                principalTable: "UserGroup",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_AspNetUsers_UserId",
                table: "Post",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_UserGroup_UserGroupId",
                table: "Post",
                column: "UserGroupId",
                principalTable: "UserGroup",
                principalColumn: "Id");
        }
    }
}