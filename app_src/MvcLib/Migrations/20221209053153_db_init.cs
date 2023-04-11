using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MvcLib.Migrations
{
    public partial class db_init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bindings",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    leixing_id = table.Column<int>(nullable: false),
                    mokuai_id = table.Column<int>(nullable: false),
                    xueke_id = table.Column<int>(nullable: false),
                    banben_id = table.Column<int>(nullable: false),
                    maker = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bindings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(nullable: false),
                    orderNo = table.Column<int>(nullable: false),
                    isShow = table.Column<bool>(nullable: false),
                    category = table.Column<int>(nullable: false),
                    make_day = table.Column<DateTime>(nullable: false),
                    update_day = table.Column<DateTime>(nullable: true),
                    memo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "column_datas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Pid = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    Maker = table.Column<string>(nullable: true),
                    MakeDay = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_column_datas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "menu_modules",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(nullable: false),
                    orderNo = table.Column<int>(nullable: false),
                    isShow = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_modules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "testpaper_props",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nianfen_or_shengfen = table.Column<int>(nullable: false),
                    value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testpaper_props", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pdf_upload_logs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(nullable: false),
                    orderNo = table.Column<int>(nullable: false),
                    isShow = table.Column<bool>(nullable: false),
                    bindingId = table.Column<int>(nullable: false),
                    make_day = table.Column<DateTime>(nullable: false),
                    maker = table.Column<string>(nullable: true),
                    memo = table.Column<string>(nullable: true),
                    image_path = table.Column<string>(nullable: true),
                    update_day = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdf_upload_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_pdf_upload_logs_bindings_bindingId",
                        column: x => x.bindingId,
                        principalTable: "bindings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sidebars",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(nullable: false),
                    orderNo = table.Column<int>(nullable: false),
                    isShow = table.Column<bool>(nullable: false),
                    Route = table.Column<string>(nullable: true),
                    ModuleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sidebars", x => x.id);
                    table.ForeignKey(
                        name: "FK_sidebars_menu_modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "menu_modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "testpaper_upload_logs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    make_day = table.Column<DateTime>(nullable: false),
                    columnId = table.Column<int>(nullable: false),
                    nfId = table.Column<int>(nullable: false),
                    sfId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testpaper_upload_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_testpaper_upload_logs_column_datas_columnId",
                        column: x => x.columnId,
                        principalTable: "column_datas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_testpaper_upload_logs_testpaper_props_nfId",
                        column: x => x.nfId,
                        principalTable: "testpaper_props",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_testpaper_upload_logs_testpaper_props_sfId",
                        column: x => x.sfId,
                        principalTable: "testpaper_props",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pdf_files",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    pdfId = table.Column<int>(nullable: false),
                    title = table.Column<string>(nullable: true),
                    make_day = table.Column<DateTime>(nullable: false),
                    saving_path = table.Column<string>(nullable: true),
                    file_type = table.Column<string>(nullable: true),
                    file_size = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdf_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_pdf_files_pdf_upload_logs_pdfId",
                        column: x => x.pdfId,
                        principalTable: "pdf_upload_logs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pdf_urls",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    pdfId = table.Column<int>(nullable: false),
                    title = table.Column<string>(nullable: true),
                    make_day = table.Column<DateTime>(nullable: false),
                    image_path = table.Column<string>(nullable: true),
                    pdf_url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdf_urls", x => x.id);
                    table.ForeignKey(
                        name: "FK_pdf_urls_pdf_upload_logs_pdfId",
                        column: x => x.pdfId,
                        principalTable: "pdf_upload_logs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "app_files",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(nullable: false),
                    orderNo = table.Column<int>(nullable: false),
                    isShow = table.Column<bool>(nullable: false),
                    path = table.Column<string>(nullable: true),
                    length = table.Column<long>(nullable: false),
                    fk_id = table.Column<string>(nullable: true),
                    make_day = table.Column<DateTime>(nullable: false),
                    maker = table.Column<string>(nullable: true),
                    TestpaperUploadid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_app_files_testpaper_upload_logs_TestpaperUploadid",
                        column: x => x.TestpaperUploadid,
                        principalTable: "testpaper_upload_logs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_app_files_TestpaperUploadid",
                table: "app_files",
                column: "TestpaperUploadid");

            migrationBuilder.CreateIndex(
                name: "IX_pdf_files_pdfId",
                table: "pdf_files",
                column: "pdfId");

            migrationBuilder.CreateIndex(
                name: "IX_pdf_upload_logs_bindingId",
                table: "pdf_upload_logs",
                column: "bindingId");

            migrationBuilder.CreateIndex(
                name: "IX_pdf_urls_pdfId",
                table: "pdf_urls",
                column: "pdfId");

            migrationBuilder.CreateIndex(
                name: "IX_sidebars_ModuleId",
                table: "sidebars",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_testpaper_upload_logs_columnId",
                table: "testpaper_upload_logs",
                column: "columnId");

            migrationBuilder.CreateIndex(
                name: "IX_testpaper_upload_logs_nfId",
                table: "testpaper_upload_logs",
                column: "nfId");

            migrationBuilder.CreateIndex(
                name: "IX_testpaper_upload_logs_sfId",
                table: "testpaper_upload_logs",
                column: "sfId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_files");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "pdf_files");

            migrationBuilder.DropTable(
                name: "pdf_urls");

            migrationBuilder.DropTable(
                name: "sidebars");

            migrationBuilder.DropTable(
                name: "testpaper_upload_logs");

            migrationBuilder.DropTable(
                name: "pdf_upload_logs");

            migrationBuilder.DropTable(
                name: "menu_modules");

            migrationBuilder.DropTable(
                name: "column_datas");

            migrationBuilder.DropTable(
                name: "testpaper_props");

            migrationBuilder.DropTable(
                name: "bindings");
        }
    }
}
