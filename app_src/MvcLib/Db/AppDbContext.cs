using Microsoft.EntityFrameworkCore;
using MvcLib.DbEntity;
using MvcLib.DbEntity.MainContent;
using MvcLib.MainContent;
using MvcLib.Sidebar;
using System.IO;

namespace MvcLib.Db
{
    public class AppDbContext : DbContext
    {
        public static string BaseDir = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        #region app of Pdf Uploads.
        public DbSet<MenuModule> menu_modules { get; set; }
        public DbSet<AppSidebar> sidebars { get; set; }
        public DbSet<ItemCategory> categories { get; set; }
        public DbSet<Binding> bindings { get; set; }
        public DbSet<PdfUploadLog> pdf_upload_logs { get; set; }
        public DbSet<PdfFile> pdf_files { get; set; }
        public DbSet<Pdf_Url> pdf_urls { get; set; }

        #endregion

        #region app of Testpaper Uploads

        public DbSet<ColumnData> column_datas { get; set; }
        public DbSet<TestpaperProps> testpaper_props { get; set; }
        public DbSet<TestpaperUpload> testpaper_upload_logs { get; set; }
        public DbSet<FileEntity> app_files { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}