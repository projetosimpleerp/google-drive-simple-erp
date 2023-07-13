using Hangfire;
using Npgsql;
using SimpleERP.GoogleDriveIntegration.Services.Drive;
using System.Diagnostics;

namespace SimpleERP.GoogleDriveIntegration.Jobs
{
    public static class Jobs
    {
        private static IConfiguration _configuration;
        public static void StartJobs(IConfiguration configuration)
        {
            Console.WriteLine("Iniciado a criação do hangfire");
            _configuration = configuration;

            RecurringJob.AddOrUpdate("Realizar backup do banco de dados", () => JobBackupBancoDados(), Cron.Weekly(DayOfWeek.Saturday, 1, 0));

            Console.WriteLine("Concluída a criação do hangfire");
        }

        public static void JobBackupBancoDados()
        {
            Console.WriteLine("Iniciado o processo de backup do banco");
            string backupFilePath = $"{VariaveisDeAmbiente.CaminhoBackupLocal}backup_{DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_")}.backup";

            try
            {
                var conn = new NpgsqlConnectionStringBuilder(_configuration.GetConnectionString("PostGres"));

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = VariaveisDeAmbiente.PGDUMP,
                    //Arguments = $"-U {conn.Username} -h {conn.Host} -p {conn.Port} -F c -f {backupFilePath} --no-password ",
                    Arguments = $"--host {conn.Host} --port {conn.Port} --username {conn.Username} --dbname {conn.Database} --format tar --file {backupFilePath} --no-password ",
                    UseShellExecute = false
                };
                processStartInfo.EnvironmentVariables["PGPASSWORD"] = conn.Password;

                var process = Process.Start(processStartInfo);
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine("Backup local realizado com sucesso.");
                    var services = new ServiceCollection();
                    services.AddScoped<IDriveUploaderService, DriveUploaderService>();

                    var serviceProvider = services.BuildServiceProvider();

                    Console.WriteLine("Iniciando o backup para o Google Drive");
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var scopedService = scope.ServiceProvider.GetRequiredService<IDriveUploaderService>();

                        var pastaUpload = _configuration.GetSection("GoogleDrive")["PastaBackupBanco"];

                        if (scopedService.UploadFile(backupFilePath, VariaveisDeAmbiente.JsonCredenciais, pastaUpload))
                            Console.WriteLine("SUCESSO - Processo de backup realizado com sucesso");
                        else
                            Console.WriteLine("FALHA - Erro ao realizar o backup para o google drive");
                    }

                }
                else
                {
                    Console.WriteLine("FALHA - Erro ao realizar o backup local");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("JOB >> Erro: " + ex.Message);
            }
        }
       
    }
}
