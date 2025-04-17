#region

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KarinaLawBot.Models;
using KarinaLawBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

#endregion

public class InlineKeyboardController
{
    private readonly IStorage _memoryStorage;
    private readonly ITelegramBotClient _telegramClient;

    public InlineKeyboardController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
    {
        _telegramClient = telegramBotClient;
        _memoryStorage = memoryStorage;
    }

    public async Task Handle(CallbackQuery callbackQuery, CancellationToken ct)
    {
        if (callbackQuery?.Data == null || callbackQuery.Message == null)
            return;

        var session = _memoryStorage.GetSession(callbackQuery.From.Id);

        // Handle back navigation
        if (callbackQuery.Data == "← Назад")
        {
            if (session.MenuHistory.Count > 0)
            {
                var previousMenu = session.MenuHistory.Pop();
                await ShowMenu(callbackQuery, previousMenu, ct);
            }
            else
            {
                await ShowMainMenu(callbackQuery, ct);
            }

            return;
        }

        if (callbackQuery.Data == "Главное меню") await ShowMainMenu(callbackQuery, ct);

        // Store current menu in history before navigating
        if (!string.IsNullOrEmpty(session.CurrentMenu)) session.MenuHistory.Push(session.CurrentMenu);

        // Handle menu selection
        switch (callbackQuery.Data)
        {
            case "Брак и супружество":
                session.CurrentMenu = "marriage";
                await ShowMarriageMenu(callbackQuery, ct);
                break;

            case "Вступление в брак":
                session.CurrentMenu = "marriage_entry";
                await ShowSubsection(callbackQuery, "Вступление в брак",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/a2d57f18a83a6bc642bc858b902cd569e3cef0af/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/460a84ab74616e740d2581dd3d2140ee1af4b5d8/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/e816c719b5266ca4f28f9ae0c236ab2e8520f3e2/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/d431b248afe31c49fd597b5beb10122ca74df291/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/eff2fdb151dc56cf74a0a70b3dbef1475c08d5c0/",
                    ct);
                break;

            case "Брачный договор":
                session.CurrentMenu = "marriage_agreement";
                await ShowSubsection(callbackQuery, "Брачный договор",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/a2d57f18a83a6bc642bc858b902cd569e3cef0af/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/460a84ab74616e740d2581dd3d2140ee1af4b5d8/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/e816c719b5266ca4f28f9ae0c236ab2e8520f3e2/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/d431b248afe31c49fd597b5beb10122ca74df291/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/eff2fdb151dc56cf74a0a70b3dbef1475c08d5c0/",
                    ct);
                break;
            case "Права и обязанности супругов":
                session.CurrentMenu = "spouses_rights";
                await ShowSubsection(callbackQuery, "Права и обязанности супругов",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/147636abacca1216aa2d431aa2c6dc3fd7864d56/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/49f88a73de81af97ad014444a159326332edb145/",
                    ct);
                break;
            case "Имущество супругов":
                session.CurrentMenu = "spouses_property";
                await ShowSubsection(callbackQuery, "Имущество супругов",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/57d1c84a547cfe1569a406f58a5b3ef183001ebd/?ysclid=m9k8cx7150361995019",
                    ct);
                break;
            case "Развод":
                session.CurrentMenu = "divorce";
                await ShowDivorceMenu(callbackQuery, ct);
                break;
            case "Процедура развода":
                session.CurrentMenu = "divorce_procedure";
                await ShowSubsection(callbackQuery, "Процедура развода",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/72751553f2dbd0ffeb99df74fd4b0e9a57ac4255/?ysclid=m9k5w3ec8b163179644",
                    ct);
                break;
            case "Раздел имущества (При разводе)":
                session.CurrentMenu = "property_division_when_divorce";
                await ShowSubsection(callbackQuery, "(При разводе)",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/ff4380f81fa499927e7dc1d442880aa81e558b05/?ysclid=m9k60k0lc4849139829",
                    ct);
                break;
            case "Алименты":
                session.CurrentMenu = "alimony_when_divorce";
                await ShowSubsection(callbackQuery, "Алименты",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/c254bd8b53f788a3ea016e5c4527190ed7160c7f/?ysclid=m9k619pl4k576099136",
                    ct);
                break;
            case "Дети (При разводе)":
                session.CurrentMenu = "children_when_divorce";
                await ShowSubsection(callbackQuery, "Дети (При разводе)",
                    "https://www.consultant.ru/law/podborki/razdel_detej_pri_razvode/?ysclid=m9k629y677387238393",
                    ct);
                break;
            case "Дети":
                session.CurrentMenu = "children";
                await ShowChildrenMenu(callbackQuery, ct);
                break;
            case "Опека и попечительство":
                session.CurrentMenu = "guardianship_and_custody";
                await ShowSubsection(callbackQuery, "Опека и попечительство",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/261afcf4fcb6fbcd17626156b4916b1b0350222c/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/5fe429f995581bcd1a1216901f2b84eba415ce6b/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/2ce1bf1392504b0a78562bf6cf4af9ccdcbf163a/\r\n\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/cfb267ce352d492b4d9b146205ba88348b05ba06/",
                    ct);
                break;
            case "Алименты ":
                session.CurrentMenu = "alimony";
                await ShowSubsection(callbackQuery, "Алименты ",
                    "https://base.garant.ru/10105807/c74d6d7c95e27021146be056ebac8f37/",
                    ct);
                break;
            case "Место жительства ребенка":
                session.CurrentMenu = "child_residence";
                await ShowSubsection(callbackQuery, "Место жительства ребенка",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/a0ad3eae55e66a972e69d952db4a21a40b10d22a/",
                    ct);
                break;
            case "Порядок общения с ребенком":
                session.CurrentMenu = "communication_order";
                await ShowSubsection(callbackQuery, "Порядок общения с ребенком",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/568c0dfec86adb4ffaaa013f338d52b75e73a93a/",
                    ct);
                break;
            case "Усыновление/удочерение":
                session.CurrentMenu = "adoption";
                await ShowSubsection(callbackQuery, "Усыновление/удочерение",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/c89471ae464eac415d61f06c2d8ee47a8f38df0d/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/aec6a578e6821b1fe69262fc2df384576edb615e/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/115c04cff624086e35d9aa0ee777c1cfbc7f8d9c/\r\n\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/ef34350ac8a228c304eaa8540e308acc341f29c8/\r\n\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/4ee53c7f603a7e0da37515c24c5c157c03a3fef7/\r\n\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/2c816e258ae6b0f3be4f8dbbadc6a1f7a6ff0ea7/",
                    ct);
                break;
            case "Родительские права":
                session.CurrentMenu = "parents";
                await ShowParentRightsMenu(callbackQuery, ct);
                break;
            case "Права и обязанности родителей":
                session.CurrentMenu = "parents_rights";
                await ShowSubsection(callbackQuery, "Права и обязанности родителей",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/ec3c0efbeb59b8523466fa966ea33bb208ccafd4/\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/6ef44561bc44714ff21426ceca1e8390b9e970cf/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_8982/2236d37faf59dafdc4b2bc53c6b05841fe616ee9/",
                    ct);
                break;
            case "Лишение родительских прав":
                session.CurrentMenu = "forfeit";
                await ShowSubsection(callbackQuery, "Лишение родительских прав",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/6af1956e4267ebdc87f7ccf3381d57e47940f49e/?ysclid=m9k5x9kg8j975794467",
                    ct);
                break;
            case "Восстановление в родит. правах":
                session.CurrentMenu = "restoration";
                await ShowSubsection(callbackQuery, "Восстановление в родительских правах",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/0a539786b6bd4e5790d483342fd92c86b18aded4/?ysclid=m9k60ejabh793258459",
                    ct);
                break;
            case "Имущественные вопросы":
                session.CurrentMenu = "property";
                await ShowPropertyMenu(callbackQuery, ct);
                break;
            case "Раздел имущества":
                session.CurrentMenu = "property_division";
                await ShowSubsection(callbackQuery, "Раздел имущества",
                    "https://www.consultant.ru/document/cons_doc_LAW_8982/ff4380f81fa499927e7dc1d442880aa81e558b05/?ysclid=m9k6cwjl3o88205663",
                    ct);
                break;
            case "Наследство":
                session.CurrentMenu = "inheritance";
                await ShowSubsection(callbackQuery, "Наследство",
                    "https://www.consultant.ru/document/cons_doc_LAW_34154/fccb811aaff0a78d6f1f77517a72c0063a807a7e/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_34154/ded009db389f384c41e25cbd01e10c72a784a54d/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_34154/8cfc2c1867828d6abc8e4285a40e181bf2f10b95/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_34154/399d34a6194d38a7c8c83deba042ec1f4dcbe8af/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_34154/8b16b1d54dc1a1a0cb68695ad846a49a19b4c1a1/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_34154/b1eb518de03f10ae323b0d7cc7c648c1a15dd2da/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_34154/1aded59dfdb3fee85e7b24b3d8e959d4501fb1bc/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_34154/1633ef51c6506c9d4fb11a0f350ed55e8012983d/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_34154/4a7a16c8ebee9064e938c398cc579bb5fcbd661f/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_34154/e1bfb4f7253efff09a17780fdd25de78361e6269/",
                    ct);
                break;
            case "Дарение":
                session.CurrentMenu = "enfeoffment";
                await ShowSubsection(callbackQuery, "Дарение",
                    "https://www.consultant.ru/document/cons_doc_LAW_9027/e92736ea135e1b4b4f24d328a683d6954e73a27c/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_9027/1a4f072ff8391b2421057eda12bcd15f721b1097/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_9027/38e5ae3585f72de3522eeb3d3420eb185d57c18d/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_9027/b1a993705399bf4cbb20df769e04d055c4d1f17a/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_9027/d21ebba51af622880adb9551b9ca17da1d72762f/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_9027/9b927ae332e239c70afdcf5208cfc8b681d8cb50/\r\n\r\nhttps://www.consultant.ru/document/cons_doc_LAW_9027/ba6eb31d50ad0b311309bdef40c04c752b39bf91/",
                    ct);
                break;
            case "Юридическая консультация":
                session.CurrentMenu = "consultation";
                await ShowConsultationMenu(callbackQuery, ct);
                break;
            case "Чат с юристом":
                session.CurrentMenu = "chat";
                await ShowSubsection(callbackQuery, "Чат с юристом",
                    "Чат с юристом",
                    ct);
                break;
            case "Запись на консультацию":
                session.CurrentMenu = "registration";
                await ShowSubsection(callbackQuery, "Запись на консультацию",
                    "Запись на консультацию",
                    ct);
                break;
            case "Поиск информации":
                session.CurrentMenu = "search";
                await ShowSearchMenu(callbackQuery, ct);
                break;
            case "Шаблоны документов":
                session.CurrentMenu = "templates";
                await ShowTemplateMenu(callbackQuery, ct);
                break;
            case "О нас":
                session.CurrentMenu = "about";
                await ShowInfoMenu(callbackQuery, ct);
                break;

        }
    }

    private async Task ShowMenu(CallbackQuery callbackQuery, string menuId, CancellationToken ct)
    {
        switch (menuId)
        {
            case "main":
                await ShowMainMenu(callbackQuery, ct);
                break;
            case "marriage":
                await ShowMarriageMenu(callbackQuery, ct);
                break;
            case "marriage_entry":
                await ShowMarriageMenu(callbackQuery, ct);
                break;
            case "marriage_agreement":
                await ShowMarriageMenu(callbackQuery, ct);
                break;
            case "spouses_rights":
                await ShowMarriageMenu(callbackQuery, ct);
                break;
            case "spouses_property":
                await ShowMarriageMenu(callbackQuery, ct);
                break;
            case "divorce":
                await ShowDivorceMenu(callbackQuery, ct);
                break;
            case "divorce_procedure":
                await ShowDivorceMenu(callbackQuery, ct);
                break;
            case "property_division_when_divorce":
                await ShowDivorceMenu(callbackQuery, ct);
                break;
            case "alimony_when_divorce":
                await ShowDivorceMenu(callbackQuery, ct);
                break;
            case "children_when_divorce":
                await ShowDivorceMenu(callbackQuery, ct);
                break;
            case "children":
                await ShowChildrenMenu(callbackQuery, ct);
                break;
            case "guardianship_and_custody":
                await ShowChildrenMenu(callbackQuery, ct);
                break;
            case "alimony":
                await ShowChildrenMenu(callbackQuery, ct);
                break;
            case "child_residence":
                await ShowChildrenMenu(callbackQuery, ct);
                break;
            case "communication_order":
                await ShowChildrenMenu(callbackQuery, ct);
                break; 
            case "adoption":
                await ShowChildrenMenu(callbackQuery, ct);
                break;
            case "parents":
                await ShowParentRightsMenu(callbackQuery, ct);
                break;
            case "parents_rights":
                await ShowParentRightsMenu(callbackQuery, ct);
                break;
            case "forfeit":
                await ShowParentRightsMenu(callbackQuery, ct);
                break;
            case "restoration":
                await ShowParentRightsMenu(callbackQuery, ct);
                break;
            case "property":
                await ShowPropertyMenu(callbackQuery, ct);
                break;
            case "property_division":
                await ShowPropertyMenu(callbackQuery, ct);
                break;
            case "inheritance":
                await ShowPropertyMenu(callbackQuery, ct);
                break;
            case "enfeoffment":
                await ShowPropertyMenu(callbackQuery, ct);
                break;
            case "consultation":
                await ShowConsultationMenu(callbackQuery, ct);
                break;
            case "chat":
                await ShowConsultationMenu(callbackQuery, ct);
                break;
            case "registration":
                await ShowConsultationMenu(callbackQuery, ct);
                break;
            case "search":
                await ShowSearchMenu(callbackQuery, ct);
                break; 
            case "templates":
                await ShowTemplateMenu(callbackQuery, ct);
                break;
            case "about":
                await ShowInfoMenu(callbackQuery, ct);
                break;
            default:
                await ShowMainMenu(callbackQuery, ct);
                break;
        }
    }

    private async Task ShowMainMenu(CallbackQuery callbackQuery, CancellationToken ct)
    {
        var buttons = MenuHelper.GetMainMenuButtons();

        await EditMenuMessage(
            callbackQuery,
            "<b>Главное меню</b>\n\nВыберите раздел:",
            buttons,
            ct);
    }

    private async Task ShowMarriageMenu(CallbackQuery callbackQuery, CancellationToken ct)
    {
        var buttons = new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithCallbackData("Вступление в брак") },
            new[] { InlineKeyboardButton.WithCallbackData("Брачный договор") },
            new[] { InlineKeyboardButton.WithCallbackData("Права и обязанности супругов") },
            new[] { InlineKeyboardButton.WithCallbackData("Имущество супругов") },
            new[] { InlineKeyboardButton.WithCallbackData("Главное меню") }
        };

        await EditMenuMessage(
            callbackQuery,
            "<b>Брак и супружество</b>\n\nВыберите подраздел:",
            buttons,
            ct);
    }

    private async Task ShowDivorceMenu(CallbackQuery callbackQuery, CancellationToken ct)
    {
        var buttons = new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithCallbackData("Процедура развода") },
            new[] { InlineKeyboardButton.WithCallbackData("Раздел имущества (При разводе)") },
            new[] { InlineKeyboardButton.WithCallbackData("Алименты") },
            new[] { InlineKeyboardButton.WithCallbackData("Дети (При разводе)") },
            new[] { InlineKeyboardButton.WithCallbackData("Главное меню") }
        };

        await EditMenuMessage(
            callbackQuery,
            "<b>Развод</b>\n\nВыберите подраздел:",
            buttons,
            ct);
    }
    private async Task ShowChildrenMenu(CallbackQuery callbackQuery, CancellationToken ct)
    {
        var buttons = new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithCallbackData("Опека и попечительство") },
            new[] { InlineKeyboardButton.WithCallbackData("Алименты") },
            new[] { InlineKeyboardButton.WithCallbackData("Место жительства ребенка") },
            new[] { InlineKeyboardButton.WithCallbackData("Порядок общения с ребенком") },
            new[] { InlineKeyboardButton.WithCallbackData("Усыновление/удочерение") },
            new[] { InlineKeyboardButton.WithCallbackData("Главное меню") }
        };

        await EditMenuMessage(
            callbackQuery,
            "<b>Дети</b>\n\nВыберите подраздел:",
            buttons,
            ct);
    }
    private async Task ShowParentRightsMenu(CallbackQuery callbackQuery, CancellationToken ct)
    {
        var buttons = new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithCallbackData("Права и обязанности родителей") },
            new[] { InlineKeyboardButton.WithCallbackData("Лишение родительских прав") },
            new[] { InlineKeyboardButton.WithCallbackData("Восстановление в родит. правах") },
            new[] { InlineKeyboardButton.WithCallbackData("Главное меню") }
        };

        await EditMenuMessage(
            callbackQuery,
            "<b>Родительские права</b>\n\nВыберите подраздел:",
            buttons,
            ct);
    }
    private async Task ShowPropertyMenu(CallbackQuery callbackQuery, CancellationToken ct)
    {
        var buttons = new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithCallbackData("Раздел имущества") },
            new[] { InlineKeyboardButton.WithCallbackData("Наследство") },
            new[] { InlineKeyboardButton.WithCallbackData("Дарение") },
            new[] { InlineKeyboardButton.WithCallbackData("Главное меню") }
        };

        await EditMenuMessage(
            callbackQuery,
            "<b>Имущественные вопросы</b>\n\nВыберите подраздел:",
            buttons,
            ct);
    }
    private async Task ShowConsultationMenu(CallbackQuery callbackQuery, CancellationToken ct)
    {
        var buttons = new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithCallbackData("Чат с юристом") },
            new[] { InlineKeyboardButton.WithCallbackData("Запись на консультацию") },
            new[] { InlineKeyboardButton.WithCallbackData("Главное меню") }
        };

        await EditMenuMessage(
            callbackQuery,
            "<b>Юридическая консультация</b>\n\nВыберите подраздел:",
            buttons,
            ct);
    }
    private async Task ShowInfoMenu(CallbackQuery callbackQuery, CancellationToken ct)
    {
        var buttons = new List<InlineKeyboardButton[]>
        {
           
            new[] { InlineKeyboardButton.WithCallbackData("Главное меню") }
        };

        await EditMenuMessage(
            callbackQuery,
            "<b>О нас</b>",
            buttons,
            ct);
    } 
    private async Task ShowSearchMenu(CallbackQuery callbackQuery, CancellationToken ct)
    {
        var buttons = new List<InlineKeyboardButton[]>
        {
           
            new[] { InlineKeyboardButton.WithCallbackData("Главное меню") }
        };

        await EditMenuMessage(
            callbackQuery,
            "<b>Поиск информации</b>",
            buttons,
            ct);
    }  
    private async Task ShowTemplateMenu(CallbackQuery callbackQuery, CancellationToken ct)
    {
        var buttons = new List<InlineKeyboardButton[]>
        {
            
            new[] { InlineKeyboardButton.WithCallbackData("Главное меню") }
        };

        await EditMenuMessage(
            callbackQuery,
            "<b>Шаблоны документов</b>",
            buttons,
            ct);
    }

    private async Task ShowSubsection(CallbackQuery callbackQuery, string title, string content, CancellationToken ct)
    {
        var buttons = new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithCallbackData("← Назад") },
            new[] { InlineKeyboardButton.WithCallbackData("Главное меню") }
        };

        await EditMenuMessage(
            callbackQuery,
            $"<b>{title}</b>\n\n{content}",
            buttons,
            ct);
    }

    private async Task EditMenuMessage(CallbackQuery callbackQuery, string text,
        List<InlineKeyboardButton[]> buttons, CancellationToken ct)
    {
        try
        {
            await _telegramClient.EditMessageTextAsync(
                callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId,
                text,
                ParseMode.Html,
                replyMarkup: new InlineKeyboardMarkup(buttons),
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error editing message: {ex.Message}");
        }
    }
}