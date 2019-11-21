using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmailDaemon.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobName = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    User = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    DateRetrieved = table.Column<DateTimeOffset>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    jobId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Emails_Jobs_jobId",
                        column: x => x.jobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emails_jobId",
                table: "Emails",
                column: "jobId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
