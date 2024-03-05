using SS.Backend.DataAccess;

namespace HTMLTesting
{
    public class Startup
    {
        // Constructor
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Method used to configure services.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to container
            services.AddControllersWithViews();
            services.AddSingleton<ConfigService>(new ConfigService(Path.Combine(AppContext.BaseDirectory, "config.local.txt")));
            services.AddScoped<SqlDAO>();
        }


        // Method used to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Products}/{action=Index}/{id?}");
            });
        }
    }
}
