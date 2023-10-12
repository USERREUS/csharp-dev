using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIES.IJP.RX;

namespace TestClasses
{
    //Класс проверки состояния принтера
    public class CheckerIJP
    {
        IJP ijp;

        public CheckerIJP(IJP ijp)
        {
            this.ijp = ijp;
        }

        //поступали ли предупреждения
        public bool AlarmExist()
        {
            if (ijp.GetAlarmInformationCount() > 0)
                return false;
            return true;
        }

        //произошла ли ошибка во время печати
        public bool PrintingJobErrorExist()
        {
            if (ijp.GetPrintingJobProgress().Status == IJPPrintingJobStatus.AbnormalEnd)
                return true;
            return false;
        }

        //произошла ли ошибка при управлении печатью
        public bool PrintingManagerErrorExist()
        {
            var management = ijp.GetPrintingManagement();
            if (management.Enable)
                return management.AlarmType && management.Error == IJPPrintingManagementError.Warning;
            return true;
        }

        //поступает ли предупреждение или ошибка от принтера
        public bool IJPStatusWarningOrFaultExist()
        {
            var status = ijp.GetStatus();
            return status.Warning != IJPWarningCode.NoWarning;
        }

        //првоеряет все ли в порядке
        public bool AllIsOk()
        {
            return !AlarmExist() &&
                   !PrintingJobErrorExist() &&
                   !PrintingManagerErrorExist() &&
                   !IJPStatusWarningOrFaultExist();
        }
    }
}
