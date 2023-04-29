using FurquimSite.DBContext;
using Microsoft.EntityFrameworkCore;

namespace AskerSite_
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) 
        {
            services.AddControllersWithViews();
            services.AddDbContext<EstDbContext>(
                options => options.UseSqlServer("Data Source=ASKERBOOK\\MSSQLSERVER01;Initial Catalog=estbase;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"));
            
        }

        public void Configure(WebApplication app, IWebHostEnvironment enviroment)
        {
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

        }
    }
}
