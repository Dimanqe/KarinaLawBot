using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KarinaLawBot.Configuration;
using Telegram.Bot;
using KarinaLawBot.Controllers;
using KarinaLawBot.Services;

namespace KarinaLawBot
{
    
        public class Program
        {
            public static async Task Main()
            {
                Console.OutputEncoding = Encoding.Unicode;

                // Объект, отвечающий за постоянный жизненный цикл приложения
                var host = new HostBuilder()
                    .ConfigureServices((hostContext, services) => ConfigureServices(services)) // Задаем конфигурацию
                    .UseConsoleLifetime() // Позволяет поддерживать приложение активным в консоли
                    .Build(); // Собираем

                Console.WriteLine("Сервис запущен");
                // Запускаем сервис
                await host.RunAsync();
                Console.WriteLine("Сервис остановлен");
            }


            static void ConfigureServices(IServiceCollection services)
            {
            var appSettings = BuildAppSettings();
            services.AddSingleton(appSettings);
            // Подключаем контроллеры сообщений и кнопок
            services.AddTransient<DefaultMessageController>();
               
                services.AddTransient<TextMessageController>();
                services.AddTransient<InlineKeyboardController>();

                services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(appSettings.BotToken));
                services.AddHostedService<Bot>();
                services.AddSingleton<IStorage, MemoryStorage>();

               
            }
            static AppSettings BuildAppSettings()
            {
                return new AppSettings()
                {
                    DownloadsFolder = "C:\\Users\\dsank\\Downloads",
                    BotToken = "7756736091:AAGJCCm-PgeSXAsPjGYO7H9feP6jxjCJLK0",
                    AudioFileName = "audio",
                    InputAudioFormat = "ogg",
                    OutputAudioFormat = "wav",
                    InputAudioBitrate = 48000
                };
            }
        }
    
}
