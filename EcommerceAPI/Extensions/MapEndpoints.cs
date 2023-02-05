using Core.Hubs;

namespace EcommerceAPI.Extensions
{
    public static class MapEndpoints
    {
        public static WebApplication MapUserEndpoints(this WebApplication app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.Redirect("/swagger", permanent: false);
                    await Task.CompletedTask;
                });
                endpoints.MapHub<InventoryHub>("/hubs/stock");
                endpoints.MapHub<ChatHub>($"/{nameof(ChatHub)}");
                endpoints.MapHub<NotificationHub>($"/{nameof(NotificationHub)}");
                endpoints.MapControllers();
            });

            return app;
        }
    }
}
