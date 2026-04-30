using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_Bot
{
    public static class LastOpertaion
    {
        public enum enLastShowDetailsAbleMenue {TodaySales = 1,ThisWeekSales=2,ThisMonthSales = 3
                ,TodayInvoices = 4, ThisWeekInvoices = 5,ThisMonthInvoices  = 6}
        public static ChoiseMenus.ChoiseProccess? LastOperation { set; get; }

        public static enLastShowDetailsAbleMenue? LastShowDetailsAbleMenue;


    }
}
