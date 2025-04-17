using KarinaLawBot.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using KarinaLawBot.Models;

public class TextMessageController
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly IStorage _memoryStorage;

    public TextMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
    {
        _telegramClient = telegramBotClient;
        _memoryStorage = memoryStorage;
    }

    public async Task ShowMainMenu(Message message, CancellationToken ct)
    {
        var session = _memoryStorage.GetSession(message.Chat.Id);
        session.CurrentMenu = "main";

        var buttons = MenuHelper.GetMainMenuButtons();

        await _telegramClient.SendTextMessageAsync(
            message.Chat.Id,
            "<b>Главное меню</b>\n\nВыберите раздел:",
            parseMode: ParseMode.Html,
            replyMarkup: new InlineKeyboardMarkup(buttons),
            cancellationToken: ct);
    }
}