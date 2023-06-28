using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using SimpleERP.GoogleDriveIntegration.Services.Drive;
using System.Diagnostics;

namespace SimpleERP.GoogleDriveIntegration.Jobs
{
    public static class Jobs
    {
        public static void StartJobs()
        {
            // Jobs fire-and-forget
            // Jobs executadas uma única vez e depois são esquecidas. 
            // Elas são acessíveis no dashboard
            var jobId = BackgroundJob.Enqueue(() => JobEsquecida());

            // Jobs delayed
            // Jobs executadas dentro de um tempo especifico
            BackgroundJob.Schedule(() => JobDelayed(), TimeSpan.FromMinutes(2));

            // Recurring jobs
            // Tarefa que será executada muitas vezes por meio de um agendamento CRON definido
            RecurringJob.AddOrUpdate("Meu job recorrente", () => JobRecorrente(), Cron.MinuteInterval(1));

            RecurringJob.AddOrUpdate("Meu job recorrente", () => JobRecorrente(), Cron.MinuteInterval(1));

            RecurringJob.AddOrUpdate("Meu job recorrente 5 minutos", () => JobRecorrente(), Cron.MinuteInterval(5));

            RecurringJob.AddOrUpdate("Meu job recorrente bloqueado", () => JobRecorrenteBloqueado(), Cron.MinuteInterval(1));

            RecurringJob.AddOrUpdate("Meu job recorrente a cada hora", () => JobRecorrente(), Cron.Hourly());

            RecurringJob.AddOrUpdate("Realizar backup do banco de dados", () => JobBackupBancoDados(), Cron.Weekly(DayOfWeek.Saturday, 1, 0));


            // Jobs com continuations
            // Permite que um job seja executada depois da finalização de uma job pai
            jobId = BackgroundJob.Enqueue(() => JobPai());
            BackgroundJob.ContinueJobWith(jobId, () => JobFilho(jobId));
        }

        public static void JobBackupBancoDados()
        {
            string backupFilePath = $"F:\\Projects\\SimpleERP\\google-drive-simple-erp\\Backup\\backup_{DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_")}.backup";

            string password = "123456";

            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "E:\\PostGree\\bin\\pg_dump",
                    Arguments = $"-U postgres -h localhost -p 5432 -F c -f {backupFilePath} --no-password ",
                    UseShellExecute = false
                };
                processStartInfo.EnvironmentVariables["PGPASSWORD"] = password;

                var process = Process.Start(processStartInfo);
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine("Backup realizado com sucesso.");
                    // Configurar o container de DI
                    var services = new ServiceCollection();
                    services.AddScoped<IDriveUploaderService, DriveUploaderService>();

                    // Criar o provedor de serviços
                    var serviceProvider = services.BuildServiceProvider();

                    // Criar o escopo de serviço
                    using (var scope = serviceProvider.CreateScope())
                    {
                        // Resolver o serviço desejado dentro do escopo
                        var scopedService = scope.ServiceProvider.GetRequiredService<IDriveUploaderService>();

                        // Usar o serviço dentro do escopo
                        scopedService.UploadFile(backupFilePath, "F:\\Projects\\SimpleERP\\google-drive-simple-erp\\simpleerp-390702-ec66d24e8cfb.json", "1JWV3R3xNp4migCZV00GBPO4rb9c3t0v_");
                    }

                }
                else
                {
                    Console.WriteLine("Falha ao realizar o backup.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
        }

        public static void JobRecorrenteBloqueado()
        {
            Console.WriteLine($"Job executando {DateTime.Now}  | {DateTime.UtcNow} >> Job recorrente bloqueado iniciando ...");
            while (true)
            {
                int i = 0;
            }
            Console.WriteLine($"Job executando {DateTime.Now}  | {DateTime.UtcNow} >> Job recorrente bloqueado finalizado");
        }

        public static void JobFilho(string jobId)
        {
            Console.WriteLine($"Job executando {DateTime.Now}  | {DateTime.UtcNow} >> Job continuation! (Job pai: {jobId})");
        }

        public static void JobPai()
        {
            Console.WriteLine($"Job executando {DateTime.Now}  | {DateTime.UtcNow} >> Job fire-and-forget pai!");
        }

        public static void JobRecorrente()
        {
            Console.WriteLine($"Job executando {DateTime.Now}  | {DateTime.UtcNow} >> Job recorrente a cada hora gerou um número: {new Random(2012).Next(1, 200)} ");
        }

        public static void JobEsquecida()
        {
            Console.WriteLine($"Job executando {DateTime.Now}  | {DateTime.UtcNow} >> Job Fire-and-forget! (Executada uma vez e depois esquecidas)");
        }

        public static void JobDelayed()
        {
            Console.WriteLine($"Job executando {DateTime.Now}  | {DateTime.UtcNow} >> Job Delayed: 2 minutos após o início da aplicação");
        }
    }
}
