using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIES.IJP.RX;

namespace TestClasses
{
    //Класс настройки принтера
    public static class IJPSetuper
    {
        //установка дисплея
        public static void DisplayEnvironmentSetup
            (
                IJPDisplayEnvironmentSetup displayES,
                IJPArabicInputMethod method,
                IJPClockDisplayFormat format,
                IJPDisplayOffTime offTime,
                IJPKeyboardLayout layout,
                bool conformWindow,
                bool iconDisplay
            )
        {
            displayES.ArabicInputMethod = method;
            displayES.ClockDisplayFormat = format;
            displayES.Display = offTime;
            displayES.KeyboardLayout = layout;
            displayES.IconDisplay = iconDisplay;
            displayES.ConformWindow = conformWindow;
        }

        //настройка управления печатью
        public static void PrintingManagementSetup
            (
                IJPPrintingManagement printingManagement,
                bool alarmType,
                bool deviceMovement,
                bool enable,
                IJPPrintingManagementError error,
                bool message
            )
        {
            printingManagement.AlarmType = alarmType;
            printingManagement.DeviceMovement = deviceMovement;
            printingManagement.Enable = enable;
            printingManagement.Error = error;
            printingManagement.Message = message;
        }

        //настройка пользовательского окружения
        public static void UserEnvironmentSetup
            (
                IJPUserEnvironmentSetup userES,
                IJPChangeCharacterOrientation changeCO,
                IJPChangeMode changeMode,
                //...13 полей
                byte startMsgNumber
            )
        {
            userES.ChangeCharacterOrientation = changeCO;
            userES.ChangeMode = changeMode;
            //...13 полей
            userES.StartMessageNumber = startMsgNumber;
        }

        //настройка правила замены (для календаря)
        public static void SubstitutionRuleSetup
            (
                IJPSubstitutionRule subRule,
                string name,
                byte number,
                byte startYear,
                int day, string setupDay,
                //...
                int year, string setupYear
            )
        {

            subRule.Name = name;
            subRule.Number = number;
            subRule.StartYear = startYear;
            subRule.SetDaySetup(day, setupDay);
            //...
            subRule.SetYearSetup(year, setupYear);
        }

        //настройка предметов сообщения 
        public static void MessageItemSetup
            (
                IJPMessageItem messageItem,
                int start, int length, byte value,
                IJPBarcode barcode,
                byte bold,
                //...16 полей
                string text
            )
        {
            messageItem.Barcode = barcode;
            messageItem.Bold = bold;
            messageItem.SetInterCharacterAdjustment(start, length, value);
            //...16 полей
            messageItem.Text = text;

        }

        //настройка сообщения
        public static void MessageSetup
            (
                IJPMessage message,
                byte characterHeigth,
                IJPCharacterOrientation characterOrientation,
                //...
                IJPVariousPrintSetup variousPrintSetup
            )
        {
            message.CharacterHeight = characterHeigth;
            message.CharacterOrientation = characterOrientation;
            //...
            message.VariousPrintSetup = variousPrintSetup;
        }

        //разные настройки печати
        public static void VariousPrintSetup
            (
                IJPVariousPrintSetup variousPrintSetup,
                IJPVariousPrintSetupBarcodePrinting barcodePrinting,
                IJPVariousPrintSetupCalendarOffset calendarOffset,
                bool dINPrint,
                IJPVariousPrintSetupEANPrefixInput eANPrefixInput,
                IJPVariousPrintSetupQRErrorCorrectionLevel qRErrorCorrectionLevel
            )
        {
            variousPrintSetup.BarcodePrinting = barcodePrinting;
            variousPrintSetup.CalendarOffset = calendarOffset;
            variousPrintSetup.DINPrint = dINPrint;
            variousPrintSetup.EANPrefix = eANPrefixInput;
            variousPrintSetup.QRErrorCorrectionLevel = qRErrorCorrectionLevel;
        }
    }
}
