using System;
using System.Globalization;

using ClientCore;
using ClientCore.Extensions;

using ClientGUI;

using DTAClient.Domain;

using Rampastring.Tools;
using Rampastring.XNAUI;

namespace DTAClient.DXGUI.Generic
{
    /// <summary>
    /// Shows the "daily luck" result dialog. Shared by the main menu and the extras window.
    /// </summary>
    public static class DailyLuckDialog
    {
        public static void Show(WindowManager windowManager)
        {
            DateTime today = DateTime.Today;
            int luckValue = DailyLuck.GetDailyLuckValue(today);

            string description;
            if (luckValue >= 80)
                description = "Excellent! Today is your lucky day.".L10N("Client:Main:DailyLuckExcellent");
            else if (luckValue >= 60)
                description = "Good. Fortune is on your side today.".L10N("Client:Main:DailyLuckGood");
            else if (luckValue >= 40)
                description = "Average. A perfectly ordinary day.".L10N("Client:Main:DailyLuckAverage");
            else if (luckValue >= 20)
                description = "Poor. Better not count on luck today.".L10N("Client:Main:DailyLuckPoor");
            else
                description = "Terrible. Maybe just stay in bed today.".L10N("Client:Main:DailyLuckTerrible");

            string message = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + Environment.NewLine +
                string.Format("Luck value: {0} / {1}".L10N("Client:Main:DailyLuckValue"), luckValue, DailyLuck.MAX_VALUE) +
                Environment.NewLine + Environment.NewLine + description;

            Logger.Log($"Daily luck for {ProgramConstants.PLAYERNAME} on {today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}: {luckValue}");

            XNAMessageBox.Show(windowManager,
                "Daily Luck".L10N("Client:Main:DailyLuckTitle"), message);
        }
    }
}
