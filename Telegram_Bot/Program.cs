using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data;
using AlalamyStock_Business;
using System.Text;
using System.Runtime.Versioning;
using AlalamyStock_Business.Profits;
using Telegram_Bot;


LastOpertaion.LastShowDetailsAbleMenue = null;

ErrorsLog log = new ErrorsLog();

log.DataAcessLayerExceptionOccured += HandleDatabaseError;

string BotToken = "8560465248:AAEOSQtZkbw_nMyZBzTie4RJprQWjke0OWE";

var bot = new TelegramBotClient(BotToken);


using CancellationTokenSource cts = new();

bot.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    cancellationToken: cts.Token
);

await bot.SetMyCommands(new[]
{
    new BotCommand { Command = "start", Description = "تشغيل البوت" }
});


Console.WriteLine("Bot is running...");
Console.ReadLine();




async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
{
    if(update.Message != null)
    {
        switch (update.Message.Text)
        {
            ////////////////Start
            case "/start":
                await ChoiseMenus.StartMenu.ShowStartMenu(bot, update);
                break;
            ////////////////Sales
            case ChoiseMenus.StartMenu.ShowSalesString:
                await ChoiseMenus.SalesMenu.ShowSalesMenue(bot, update);
                break;
            case ChoiseMenus.SalesMenu.TodaysSalesString:
                await ChoiseMenus.SalesMenu.ShowTodaySalesProccess(bot, update);
                break;
            case ChoiseMenus.SalesMenu.ThisWeekSalesString:
                await ChoiseMenus.SalesMenu.ShowThisWeekSalesProccess(bot, update);
                break;
            case ChoiseMenus.SalesMenu.ThisMonthSalesString:
                await ChoiseMenus.SalesMenu.ShowThisMonthSalesProccess(bot, update);
                break;
            ////////////////Avaiable Consoles
            case ChoiseMenus.StartMenu.ShowAvaiableConsolesString:
                await ChoiseMenus.AvaiableConsolesMenu.ShowAvaiableConsolesMenue(bot, update);
                break;
            ////////////////Invoices
            case ChoiseMenus.StartMenu.ShowInvoicesString:
                await ChoiseMenus.InvoicesMenue.ShowInvoicesMenue(bot, update);
                break;
            case ChoiseMenus.InvoicesMenue.TodaysInvoicesString:
                await ChoiseMenus.InvoicesMenue.ShowTodayInvoicesMenue(bot, update);
                break;
            case ChoiseMenus.InvoicesMenue.ThisWeekInvoicesString:
                await ChoiseMenus.InvoicesMenue.ShowThisWeekInvoicesMenue(bot, update);
                break;
            case ChoiseMenus.InvoicesMenue.ThisMonthInvoicesString:
                await ChoiseMenus.InvoicesMenue.ShowThisMonthInvoicesMenue(bot, update);
                break;
            ////////////////Details
            case ChoiseMenus.ShowDetailsString:
                ChoiseMenus.ShowDetails(bot, update);
                break;
            ////////////////Step Back
            case ChoiseMenus.StepBackString:
                if (LastOpertaion.LastOperation == null)
                {
                    await ChoiseMenus.StartMenu.ShowStartMenu(bot, update);
                }
                else
                {
                    await LastOpertaion.LastOperation.Invoke(bot, update);
                }
                break;
        }

    }
}

Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
{
    Console.WriteLine(ex.Message);
    return Task.CompletedTask;
}

void HandleDatabaseError(Exception ex)
{
    Console.WriteLine(ex.Message);
}
