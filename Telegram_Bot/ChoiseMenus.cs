using Alalamy_Stock_Entities;
using Alalamy_Stock_Entities.Consoles;
using Alalamy_Stock_Entities.Invoices;
using AlalamyStock_Business;
using AlalamyStock_Business.Profits;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using static Telegram_Bot.ChoiseMenus.SalesMenu;

namespace Telegram_Bot
{
    public static class ChoiseMenus
    {
        public delegate Task ChoiseProccess(ITelegramBotClient bot, Update update);

        public const string StepBackString = "الرجوع 🔙";

        public const string ShowDetailsString = "عرض التفاصيل 🔍";
        static void UpdatePreviousOperation(ChoiseMenus.ChoiseProccess? Operation)
        {
            if (Operation != null)
            {
                LastOpertaion.LastOperation = Operation;
            }

        }

        static async void ShowDetailsAccourdingToSpecificString(ChoiseProccess lastOperation,string st, ITelegramBotClient bot, Update update)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
                {
                new KeyboardButton[] {StepBackString}
                });


            if (update.Message != null)
            {
                await bot.SendMessage(chatId:update.Message.Chat.Id,text : st,replyMarkup:keyboard );
            }

            UpdatePreviousOperation(lastOperation);
        }
        public static async void ShowDetails(ITelegramBotClient bot, Update update)
        {
            if(LastOpertaion.LastShowDetailsAbleMenue == null && update.Message != null)
            {
                await bot.SendMessage(chatId: update.Message.Chat.Id, text: "لا توجد قائمة مختارة لعرض تفاصيلها الرجاء اختيار قائمة مبيعات او فواتير لعرض تفاصيلها");
                return;
            }

            

            switch(LastOpertaion.LastShowDetailsAbleMenue)
            {
                ////////////Sales
                case LastOpertaion.enLastShowDetailsAbleMenue.TodaySales:
                    ShowDetailsAccourdingToSpecificString(SalesMenu.ShowTodaySalesProccess,await SalesMenu.GetTodaySalesDetailsString(), bot,update);
                    break;
                case LastOpertaion.enLastShowDetailsAbleMenue.ThisWeekSales:
                    ShowDetailsAccourdingToSpecificString(SalesMenu.ShowThisWeekSalesProccess, await SalesMenu.GetThisWeekSalesDetailsString(),bot,update);
                    break;
                case LastOpertaion.enLastShowDetailsAbleMenue.ThisMonthSales:
                    ShowDetailsAccourdingToSpecificString(SalesMenu.ShowThisMonthSalesProccess, await SalesMenu.GetThisMonthSalesDetailsString(), bot, update);
                    break;
                ////////////Invoices
                case LastOpertaion.enLastShowDetailsAbleMenue.TodayInvoices:
                    ShowDetailsAccourdingToSpecificString(InvoicesMenue.ShowTodayInvoicesMenue, await InvoicesMenue.GetTodayInvoicesDetailsString(), bot, update);
                    break;
                case LastOpertaion.enLastShowDetailsAbleMenue.ThisWeekInvoices:
                    ShowDetailsAccourdingToSpecificString(InvoicesMenue.ShowThisWeekInvoicesMenue, await InvoicesMenue.GetThisWeekInvoicesDetailsString(), bot, update);
                    break;
                case LastOpertaion.enLastShowDetailsAbleMenue.ThisMonthInvoices:
                    ShowDetailsAccourdingToSpecificString(InvoicesMenue.ShowThisMonthInvoicesMenue, await InvoicesMenue.GetThisMonthInvoicesDetailsString(), bot, update);
                    break;
            }
        }

        public static class StartMenu
        {
            public const string ShowSalesString = "عرض المبيعات 💰";
            public const string ShowAvaiableConsolesString = "عرض الاجهزة المتاحة 🎮";
            public const string ShowInvoicesString = "عرض الفواتير 🧾";

            public static async Task ShowStartMenu(ITelegramBotClient bot, Update update)
            {
                var keyboard = new ReplyKeyboardMarkup(new[]
                {
            new KeyboardButton[] {ShowSalesString},
            new KeyboardButton[] {ShowAvaiableConsolesString},
            new KeyboardButton[] {ShowInvoicesString },
            });

                if (update.Message != null)
                {
                    await bot.SendMessage(
                    chatId: update.Message.Chat.Id,
                    text: "اختار:",
                    replyMarkup: keyboard
                );
                }

                LastOpertaion.LastShowDetailsAbleMenue = null;
            }

            

        }

        public static class SalesMenu
        {
            public const string TodaysSalesString = "مبيعات اليوم 📅";
            public const string ThisWeekSalesString = "مبيعات الاسبوع 📆";
            public const string ThisMonthSalesString = "مبيعات الشهر 📆";

            public delegate Task<decimal> GetProfitFunc();
            public static async Task ShowSalesMenue(ITelegramBotClient bot, Update update)
            {
                var keyboard = new ReplyKeyboardMarkup(new[]
               {
            new KeyboardButton[] {TodaysSalesString },
            new KeyboardButton[] {ThisWeekSalesString },
            new KeyboardButton[] {ThisMonthSalesString },
            new KeyboardButton[] { StepBackString }
            });

                if (update.Message != null)
                {
                    await bot.SendMessage(
                    chatId: update.Message.Chat.Id,
                    text: "اختار",
                    replyMarkup: keyboard
                );
                }
                UpdatePreviousOperation(StartMenu.ShowStartMenu);
            }

            public static async Task ShowSalesAccourdingToFunction(GetProfitFunc getProfitFunction, string SalesMessageString, ITelegramBotClient bot, Update update)
            {
                decimal TodaysProfit = await getProfitFunction();

                string ProfitsString = ProfitsMenu.GetProfitsString(TodaysProfit, SalesMessageString);

                var keyboard = new ReplyKeyboardMarkup(new[]
               {
            new KeyboardButton[] {ShowDetailsString},
            new KeyboardButton[] { StepBackString }
            });

                if (string.IsNullOrEmpty(ProfitsString))
                {
                    ProfitsString = "لا توجد مبيعات";
                }
                if (update.Message != null)
                {
                    await bot.SendMessage(
                    chatId: update.Message.Chat.Id,
                    text: ProfitsString,
                    replyMarkup: keyboard
                );
                }
                UpdatePreviousOperation(ShowSalesMenue);
            }

            ////////////////Menues
            public static async Task ShowTodaySalesProccess(ITelegramBotClient bot, Update update)
            {
                await ShowSalesAccourdingToFunction(clsSalesLogic.GetTodayProfit, "اجمالي مبيعات اليوم هي : ", bot, update);
                LastOpertaion.LastShowDetailsAbleMenue = LastOpertaion.enLastShowDetailsAbleMenue.TodaySales;
            }
            public static async Task ShowThisWeekSalesProccess(ITelegramBotClient bot, Update update)
            {
                await ShowSalesAccourdingToFunction(clsSalesLogic.GetThisWeekProfit, "اجمالي مبيعات هذا الاسبوع هي : ", bot, update);
                LastOpertaion.LastShowDetailsAbleMenue = LastOpertaion.enLastShowDetailsAbleMenue.ThisWeekSales;
            }
            public static async Task ShowThisMonthSalesProccess(ITelegramBotClient bot, Update update)
            {
                await ShowSalesAccourdingToFunction(clsSalesLogic.GetThisMonthProfit, "اجمالي مبيعات هذا الشهر هي : ", bot, update);
                LastOpertaion.LastShowDetailsAbleMenue = LastOpertaion.enLastShowDetailsAbleMenue.ThisMonthSales;
            }

            ////////////////Details
            public delegate Task<DataTable> DetailStringFunc();
            public static async Task<string> GetSalesDetailsStringByDetailFunc(DetailStringFunc func,string Header)
            {
                List<EnSalesDetails>? details = clsSalesLogic.GetSalesDetailsListFromDataTable(await func());

                if (details == null)
                {
                    return "لا توجد تفاصيل لعرضها";
                }

                StringBuilder detString = new StringBuilder(Header);

                for(int counter =0;counter<details.Count;counter++)
                {
                    detString.AppendLine($"تم بيع عدد {{{details[counter].QuantityOfSoldProduct}}} من منتج {{{details[counter].ProductName}}} باجمالي مبيعات {{{details[counter].TotalSalesOfProduct}}}");
                    
                    if(counter != details.Count - 1)
                    {
                        detString.AppendLine("----------------------------------");
                    }
                }

                return detString.ToString();
            }
            public static async Task<string> GetTodaySalesDetailsString()
            {
                string Header = "مبيعات هذا اليوم\n---------------------------------\n";
                return await GetSalesDetailsStringByDetailFunc(clsSalesLogic.GetTodaySalesDetails, Header);
            }
            public static async Task<string> GetThisWeekSalesDetailsString()
            {
                string Header = "مبيعات هذا الاسبوع\n---------------------------------\n";
                return await GetSalesDetailsStringByDetailFunc(clsSalesLogic.GetThisWeekSalesDetails,Header);
            }
            public static async Task<string> GetThisMonthSalesDetailsString()
            {
                string Header = "مبيعات هذا الشهر\n---------------------------------\n";
                return await GetSalesDetailsStringByDetailFunc(clsSalesLogic.GetThisMonthSalesDetails,Header);
            }

        }

        public static class ProfitsMenu
        {
            public static string GetProfitsString(decimal Profit, string ShowMessage)
            {
                StringBuilder builder = new StringBuilder(ShowMessage);

                builder.Append($"{Profit} جنيه 💰");

                return builder.ToString();

            }

            public static async Task ShowTodayProfitDetails(ITelegramBotClient bot, Update update)
            {

                var keyboard = new ReplyKeyboardMarkup(new[]
               {
            new KeyboardButton[] { StepBackString }
            });


                if (update.Message != null)
                {
                    await bot.SendMessage(
                    chatId: update.Message.Chat.Id,
                    text: "مثال",
                    replyMarkup: keyboard
                );
                }
                UpdatePreviousOperation(SalesMenu.ShowTodaySalesProccess);

            }

        }

        public static class AvaiableConsolesMenu
        {
            static string GetAvaiableConsolesStringFromConsolesList(List<ConsolesShowData> consolesList)
            {
                if (consolesList.Count == 0)
                {
                    return "لا توجد اي اجهزة متاحة"; 
                }


                StringBuilder builder = new StringBuilder(
                    @$"
عدد الاجهزة المتاحة : {{{consolesList.Count}}}
-------------------------------------------
");

                for (int counter = 0;counter < consolesList.Count;counter++)
                {
                    string PurchasePrice = consolesList[counter].PurchasePrice != null ?
                        $"{{{consolesList[counter].PurchasePrice}}}" : "{غير معروف}";

                    builder.AppendLine(
                        @$"جهاز رقم {{{consolesList[counter].ID}}}
الرقم التسلسلي {{{consolesList[counter].SerialNumber}}}
نوع الجهاز {{{consolesList[counter].Type}}}
سعر الشراء  {PurchasePrice}
معلومات النظام : سوفت {{{consolesList[counter].SoftWare}}}");

                    if(counter != consolesList.Count -1)
                    {
                        builder.AppendLine(
"--------------------------------------"
                            );
                    }
                }
               
                return builder.ToString();
            }

            public static async Task ShowAvaiableConsolesMenue(ITelegramBotClient bot, Update update)
            {
                List<ConsolesShowData> AvaiableConsole = await clsConsoles.GetAllAvaiableConsolesToShowList(); ;

                string AvaiableConsolesString = GetAvaiableConsolesStringFromConsolesList(AvaiableConsole);

                var keyboard = new ReplyKeyboardMarkup(new[]
               {
            new KeyboardButton[] { StepBackString }
            });

                if (update.Message != null)
                {
                    await bot.SendMessage(
                    chatId: update.Message.Chat.Id,
                    text: AvaiableConsolesString,
                    replyMarkup: keyboard
                );
                }
                UpdatePreviousOperation(StartMenu.ShowStartMenu);
                LastOpertaion.LastShowDetailsAbleMenue = null;
            }
        }

        public static class InvoicesMenue
        {
            public const string TodaysInvoicesString = "فواتير اليوم 📅";
            public const string ThisWeekInvoicesString = "فواتير هذا الاسبوع 📆";
            public const string ThisMonthInvoicesString = "فواتير هذا الشهر 📆";

            public delegate Task<int> GetInvoicesFunction();
            public static async Task ShowInvoicesMenue(ITelegramBotClient bot, Update update)
            {
                var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new KeyboardButton[] {TodaysInvoicesString},
                new KeyboardButton[] {ThisWeekInvoicesString },
                new KeyboardButton[] {ThisMonthInvoicesString },
            new KeyboardButton[] { StepBackString }
            });

                if (update.Message != null)
                {
                    await bot.SendMessage(
                    chatId: update.Message.Chat.Id,
                    text: "اختار",
                    replyMarkup: keyboard
                );
                }
                UpdatePreviousOperation(StartMenu.ShowStartMenu);
            }

           
            public static async Task ShowInvoicesAccourdingToFunction(GetInvoicesFunction func,string MessageHeader, ITelegramBotClient bot, Update update)
            {
                var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new KeyboardButton[] {ShowDetailsString},
                new KeyboardButton[] { StepBackString }
               });

                string TodayInvoicesCountString = $"{MessageHeader} {{{await func()}}}";

                if (update.Message != null)
                {
                    await bot.SendMessage(
                    chatId: update.Message.Chat.Id,
                    text: TodayInvoicesCountString,
                    replyMarkup: keyboard
                );
                }
                UpdatePreviousOperation(InvoicesMenue.ShowInvoicesMenue);
            }

            ////////////////Menues
            public static async Task ShowTodayInvoicesMenue(ITelegramBotClient bot, Update update)
            {
                await ShowInvoicesAccourdingToFunction(clsInvoices.GetTodayInvoicesCount, "عدد فواتير اليوم هو ", bot, update);
                LastOpertaion.LastShowDetailsAbleMenue = LastOpertaion.enLastShowDetailsAbleMenue.TodayInvoices;
            }

            public static async Task ShowThisWeekInvoicesMenue(ITelegramBotClient bot, Update update)
            {
                await ShowInvoicesAccourdingToFunction(clsInvoices.GetThisWeekInvoicesCount, "عدد فواتير هذا الاسبوع هو ", bot, update);
                LastOpertaion.LastShowDetailsAbleMenue = LastOpertaion.enLastShowDetailsAbleMenue.ThisWeekInvoices;
            }

            public static async Task ShowThisMonthInvoicesMenue(ITelegramBotClient bot, Update update)
            {
                await ShowInvoicesAccourdingToFunction(clsInvoices.GetThisMonthInvoicesCount, "عدد فواتير هذا الشهر هو ", bot, update);
                LastOpertaion.LastShowDetailsAbleMenue = LastOpertaion.enLastShowDetailsAbleMenue.ThisMonthInvoices;
            }

            ////////////////Details

            public delegate Task<List<InvoicesDetailsData>> InvoiceDetailsListFunc();
            public static async Task<string> GetInvoicesDetailsStringByDetailListFunc(InvoiceDetailsListFunc func, string Header)
            {
                List<InvoicesDetailsData>? details = await func();

                if (details == null)
                {
                    return "لا توجد فواتير لعرضها";
                }

                StringBuilder detString = new StringBuilder(Header);

                for (int counter = 0; counter < details.Count; counter++)
                {
                    string InvoiceData =
$@" فاتورة رقم {{{details[counter].InvoiceID}}}
تم عمل الفاتورة من خلال {{{details[counter].EmployeeName}}}
للعميل {{{details[counter].CustomerName}}} رقمه {{{details[counter].CustomerPhoneNumber}}}
بتاريخ {{{details[counter].InvoiceDate}}}
باجمالي مبلغ {{{details[counter].TotalPrice}}}
تم دفع منهم {{{details[counter].PaidPrice}}}
";

                    StringBuilder itemsString = new StringBuilder("");
                    foreach(InvoiceItemsDetails item in details[counter].Items)
                    {
                        string SerialNumberValue = string.IsNullOrEmpty(item.SerialNumber) ? "غير موجود" : item.SerialNumber;

                        string ComponentsValue = string.IsNullOrEmpty(item.Components) ? "غير موجود" : item.Components;

                        itemsString.AppendLine(
$@"ملحق رقم {{{item.InvoiceItemID}}}
اسم المنتج {{{item.ProductName}}}
حالة المنتج {{{item.Status}}}
الفاتورة تحتوي على {{{item.Quantity}}} من المنتج
اجمالي مبيعات المنتج في الفاتورة {{{item.ItemPaidPrice}}}
رقم الجهاز التسلسلي (للبلايستيشن فقط) {{{SerialNumberValue}}}
الملحقات (للبلايستيشن فقط) {{{ComponentsValue}}}
----------
");
                    }

                    detString.AppendLine(@$"{InvoiceData}
-------
ملحقات الفاتورة
-------
{itemsString}
");

                    if (counter != details.Count - 1)
                    {
                        detString.AppendLine("----------------------------------");
                    }
                }

                return detString.ToString();
            }

            
            public static async Task<string> GetTodayInvoicesDetailsString()
            {
                string Header = "فواتير هذا اليوم\n---------------------------------\n";

                return await GetInvoicesDetailsStringByDetailListFunc(clsInvoices.GetTodayInvoicesDetails,Header);
            }
            public static async Task<string> GetThisWeekInvoicesDetailsString()
            {
                string Header = "فواتير هذا الاسبوع\n---------------------------------\n";

                return await GetInvoicesDetailsStringByDetailListFunc(clsInvoices.GetThisWeekInvoicesDetails, Header);
            }
            public static async Task<string> GetThisMonthInvoicesDetailsString()
            {
                string Header = "فواتير هذا الشهر\n---------------------------------\n";

                return await GetInvoicesDetailsStringByDetailListFunc(clsInvoices.GetThisMonthInvoicesDetails, Header);
            }
        }

    }
}
