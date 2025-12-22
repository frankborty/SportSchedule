namespace SportSchedule.Utils
{
    public static class CorsConfigurator
    {
        internal static void ConfigureCors(IConfiguration configuration, CorsOptions options)
        {
            // Leggi origin dal file di configurazione
            var allowedOriginsString = configuration["Cors:AllowedOrigins"];
            var allowedOrigins = Array.Empty<string>();

            if (!string.IsNullOrEmpty(allowedOriginsString))
            {
                allowedOrigins = allowedOriginsString
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }

            options.AddPolicy("FrontendOnly", policy =>
            {
                if (allowedOrigins != null && allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
                else
                {
                    // Nessuna origine configurata: fallback sicuro (blocca tutto)
                    policy.DisallowCredentials(); // o lasciamo policy vuota                                  
                }
            });
        }
    }
}
