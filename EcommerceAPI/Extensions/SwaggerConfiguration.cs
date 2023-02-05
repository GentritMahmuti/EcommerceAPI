namespace EcommerceAPI.Extensions
{
    public static class SwaggerConfiguration
    {
        public static WebApplication ConfigureSwagger(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.DisplayRequestDuration();
                    c.DefaultModelExpandDepth(0);
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerceProject");
                    c.OAuthClientId("fb1b97e4-778a-431d-abb1-78bbdca9253b");
                    c.OAuthClientSecret("21551351-fc9a-4d8e-8619-8c7e5acb6d47");
                    c.OAuthAppName("ECommerceProject");
                    c.OAuthUsePkce();
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                });
            }
            return app;
        }
    }
}
