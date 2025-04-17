using KarinaLawBot.Controllers;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using KarinaLawBot.Services;
using Telegram.Bot;

namespace KarinaLawBot
{
    public class Bot : BackgroundService
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly InlineKeyboardController _inlineKeyboardController;
        private readonly TextMessageController _textMessageController;
        private readonly DefaultMessageController _defaultMessageController;
        private readonly IStorage _memoryStorage;  // Add this field

        public Bot(
            ITelegramBotClient telegramClient,
            InlineKeyboardController inlineKeyboardController,
            TextMessageController textMessageController,
            DefaultMessageController defaultMessageController,
            IStorage memoryStorage)  // Add this parameter
        {
            _telegramClient = telegramClient;
            _inlineKeyboardController = inlineKeyboardController;
            _textMessageController = textMessageController;
            _defaultMessageController = defaultMessageController;
            _memoryStorage = memoryStorage;  // Initialize the field
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("Attempting to connect to Telegram...");
                    _telegramClient.StartReceiving(
                        HandleUpdateAsync,
                        HandleErrorAsync,
                        new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
                        cancellationToken: stoppingToken);

                    Console.WriteLine("Connected successfully");
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection error: {ex.Message}");
                    await Task.Delay(10000, stoppingToken); // Wait 10 seconds before retry
                }
            }
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Type == UpdateType.CallbackQuery)
                {
                    await _inlineKeyboardController.Handle(update.CallbackQuery, cancellationToken);
                    return;
                }

                if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Text)
                {
                    if (update.Message.Text == "/start")
                    {
                        var session = _memoryStorage.GetSession(update.Message.Chat.Id);
                        session.MenuHistory.Clear();
                        session.CurrentMenu = null;

                        await _textMessageController.ShowMainMenu(update.Message, cancellationToken);
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling update: {ex.Message}");
            }
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            Console.WriteLine("Waiting 10 seconds before reconnection.");
            return Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        }
    }
}