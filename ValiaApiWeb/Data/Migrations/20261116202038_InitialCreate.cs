using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValiaApiWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    USER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FULL_NAME = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.USER_ID);
                });

            migrationBuilder.CreateTable(
                name: "WORK_CONDITION_QUESTIONS",
                columns: table => new
                {
                    WORK_CONDITION_QUESTION_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    QUESTION_TEXT = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    CATEGORY = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DISPLAY_ORDER = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    QUESTIONNAIRE_VERSION = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WORK_CONDITION_QUESTIONS", x => x.WORK_CONDITION_QUESTION_ID);
                });

            migrationBuilder.CreateTable(
                name: "COMMUNITY_POSTS",
                columns: table => new
                {
                    COMMUNITY_POST_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    USER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CATEGORY = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    TITLE = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    CONTENT = table.Column<string>(type: "NCLOB", maxLength: 5000, nullable: false),
                    IS_ANONYMOUS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    VERSION = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMMUNITY_POSTS", x => x.COMMUNITY_POST_ID);
                    table.ForeignKey(
                        name: "FK_COMMUNITY_POSTS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYMENTS",
                columns: table => new
                {
                    EMPLOYMENT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    USER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    JOB_TITLE = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    EMPLOYMENT_TYPE = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    ADMISSION_DATE = table.Column<DateTime>(type: "DATE", nullable: false),
                    TERMINATION_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    BASE_SALARY = table.Column<decimal>(type: "NUMBER(12,2)", nullable: true),
                    WEEKLY_WORKLOAD = table.Column<decimal>(type: "NUMBER(5,2)", nullable: true),
                    PROFESSIONAL_CATEGORY = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: true),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    VERSION = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYMENTS", x => x.EMPLOYMENT_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYMENTS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "COMMUNITY_COMMENTS",
                columns: table => new
                {
                    COMMUNITY_COMMENT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    COMMUNITY_POST_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    USER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CONTENT = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    IS_ANONYMOUS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    VERSION = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMMUNITY_COMMENTS", x => x.COMMUNITY_COMMENT_ID);
                    table.ForeignKey(
                        name: "FK_COMMUNITY_COMMENTS_COMMUNITY_POSTS_COMMUNITY_POST_ID",
                        column: x => x.COMMUNITY_POST_ID,
                        principalTable: "COMMUNITY_POSTS",
                        principalColumn: "COMMUNITY_POST_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_COMMUNITY_COMMENTS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EVIDENCES",
                columns: table => new
                {
                    EVIDENCE_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    USER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EMPLOYMENT_ID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    TITLE = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    CATEGORY = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    FILE_NAME = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    STORAGE_KEY = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CONTENT_TYPE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    FILE_SIZE = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVIDENCES", x => x.EVIDENCE_ID);
                    table.ForeignKey(
                        name: "FK_EVIDENCES_EMPLOYMENTS_EMPLOYMENT_ID",
                        column: x => x.EMPLOYMENT_ID,
                        principalTable: "EMPLOYMENTS",
                        principalColumn: "EMPLOYMENT_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EVIDENCES_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WORK_CONDITION_ASSESSMENTS",
                columns: table => new
                {
                    WORK_CONDITION_ASSESSMENT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EMPLOYMENT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    QUESTIONNAIRE_VERSION = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ASSESSMENT_DATE = table.Column<DateTime>(type: "DATE", nullable: false),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WORK_CONDITION_ASSESSMENTS", x => x.WORK_CONDITION_ASSESSMENT_ID);
                    table.ForeignKey(
                        name: "FK_WORK_CONDITION_ASSESSMENTS_EMPLOYMENTS_EMPLOYMENT_ID",
                        column: x => x.EMPLOYMENT_ID,
                        principalTable: "EMPLOYMENTS",
                        principalColumn: "EMPLOYMENT_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WORK_OCCURRENCES",
                columns: table => new
                {
                    WORK_OCCURRENCE_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EMPLOYMENT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CATEGORY = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NCLOB", maxLength: 3000, nullable: false),
                    OCCURRED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    VERSION = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WORK_OCCURRENCES", x => x.WORK_OCCURRENCE_ID);
                    table.ForeignKey(
                        name: "FK_WORK_OCCURRENCES_EMPLOYMENTS_EMPLOYMENT_ID",
                        column: x => x.EMPLOYMENT_ID,
                        principalTable: "EMPLOYMENTS",
                        principalColumn: "EMPLOYMENT_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WORK_SHIFTS",
                columns: table => new
                {
                    WORK_SHIFT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EMPLOYMENT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    START_TIME = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    END_TIME = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    BREAK_START = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    BREAK_END = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    NOTES = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    UPDATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    VERSION = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WORK_SHIFTS", x => x.WORK_SHIFT_ID);
                    table.ForeignKey(
                        name: "FK_WORK_SHIFTS_EMPLOYMENTS_EMPLOYMENT_ID",
                        column: x => x.EMPLOYMENT_ID,
                        principalTable: "EMPLOYMENTS",
                        principalColumn: "EMPLOYMENT_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WORK_CONDITION_ANSWERS",
                columns: table => new
                {
                    WORK_CONDITION_ANSWER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    WORK_CONDITION_ASSESSMENT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    WORK_CONDITION_QUESTION_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    SCORE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    OBSERVATION = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WORK_CONDITION_ANSWERS", x => x.WORK_CONDITION_ANSWER_ID);
                    table.ForeignKey(
                        name: "FK_WORK_CONDITION_ANSWERS_WORK_CONDITION_ASSESSMENTS_WORK_CONDITION_ASSESSMENT_ID",
                        column: x => x.WORK_CONDITION_ASSESSMENT_ID,
                        principalTable: "WORK_CONDITION_ASSESSMENTS",
                        principalColumn: "WORK_CONDITION_ASSESSMENT_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WORK_CONDITION_ANSWERS_WORK_CONDITION_QUESTIONS_WORK_CONDITION_QUESTION_ID",
                        column: x => x.WORK_CONDITION_QUESTION_ID,
                        principalTable: "WORK_CONDITION_QUESTIONS",
                        principalColumn: "WORK_CONDITION_QUESTION_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_COMMUNITY_COMMENTS_COMMUNITY_POST_ID_IS_ACTIVE_CREATED_AT",
                table: "COMMUNITY_COMMENTS",
                columns: new[] { "COMMUNITY_POST_ID", "IS_ACTIVE", "CREATED_AT" });

            migrationBuilder.CreateIndex(
                name: "IX_COMMUNITY_COMMENTS_USER_ID",
                table: "COMMUNITY_COMMENTS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_COMMUNITY_POSTS_IS_ACTIVE_CREATED_AT",
                table: "COMMUNITY_POSTS",
                columns: new[] { "IS_ACTIVE", "CREATED_AT" });

            migrationBuilder.CreateIndex(
                name: "IX_COMMUNITY_POSTS_USER_ID",
                table: "COMMUNITY_POSTS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYMENTS_USER_ID",
                table: "EMPLOYMENTS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EVIDENCES_EMPLOYMENT_ID",
                table: "EVIDENCES",
                column: "EMPLOYMENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EVIDENCES_STORAGE_KEY",
                table: "EVIDENCES",
                column: "STORAGE_KEY",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EVIDENCES_USER_ID",
                table: "EVIDENCES",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_EMAIL",
                table: "USERS",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WORK_CONDITION_ANSWERS_WORK_CONDITION_ASSESSMENT_ID_WORK_CONDITION_QUESTION_ID",
                table: "WORK_CONDITION_ANSWERS",
                columns: new[] { "WORK_CONDITION_ASSESSMENT_ID", "WORK_CONDITION_QUESTION_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WORK_CONDITION_ANSWERS_WORK_CONDITION_QUESTION_ID",
                table: "WORK_CONDITION_ANSWERS",
                column: "WORK_CONDITION_QUESTION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WORK_CONDITION_ASSESSMENTS_EMPLOYMENT_ID_ASSESSMENT_DATE",
                table: "WORK_CONDITION_ASSESSMENTS",
                columns: new[] { "EMPLOYMENT_ID", "ASSESSMENT_DATE" });

            migrationBuilder.CreateIndex(
                name: "IX_WORK_CONDITION_QUESTIONS_QUESTIONNAIRE_VERSION_DISPLAY_ORDER",
                table: "WORK_CONDITION_QUESTIONS",
                columns: new[] { "QUESTIONNAIRE_VERSION", "DISPLAY_ORDER" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WORK_OCCURRENCES_EMPLOYMENT_ID_OCCURRED_AT",
                table: "WORK_OCCURRENCES",
                columns: new[] { "EMPLOYMENT_ID", "OCCURRED_AT" });

            migrationBuilder.CreateIndex(
                name: "IX_WORK_SHIFTS_EMPLOYMENT_ID_START_TIME",
                table: "WORK_SHIFTS",
                columns: new[] { "EMPLOYMENT_ID", "START_TIME" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "COMMUNITY_COMMENTS");

            migrationBuilder.DropTable(
                name: "EVIDENCES");

            migrationBuilder.DropTable(
                name: "WORK_CONDITION_ANSWERS");

            migrationBuilder.DropTable(
                name: "WORK_OCCURRENCES");

            migrationBuilder.DropTable(
                name: "WORK_SHIFTS");

            migrationBuilder.DropTable(
                name: "COMMUNITY_POSTS");

            migrationBuilder.DropTable(
                name: "WORK_CONDITION_ASSESSMENTS");

            migrationBuilder.DropTable(
                name: "WORK_CONDITION_QUESTIONS");

            migrationBuilder.DropTable(
                name: "EMPLOYMENTS");

            migrationBuilder.DropTable(
                name: "USERS");
        }
    }
}
