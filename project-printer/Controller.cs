using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIES.IJP.RX;

namespace TestClasses
{
    //Контроллер за состоянием
    public class Controller
    {
        IJP ijp;
        public CheckerIJP checker;

        public Controller(IJP ijp)
        {
            this.ijp = ijp;
            checker = new CheckerIJP(ijp);
        }

        ~Controller() { }

        //можно ли начать печать
        public bool CanStartPrint()
        {
            if (ijp != null && checker.AllIsOk())
            {
                var status = ijp.GetStatus();
                if
                (
                    status.CommunicationConnection == IJPCommunicationConnectionStatus.Online &&
                    status.Warning == IJPWarningCode.NoWarning &&
                    status.ReceiveEnableDisable == IJPReceivableStatus.Enabled &&
                    status.Operation == IJPRemoteOperationStatus.Suspend
                )
                    return true;
            }
            return false;
        }

        //проверка целостности чернил
        public bool InkIsOk()
        {
            if
            (
                ijp.GetCirculationControl().EnvironmentSetup.TemperatureDifferenceIncrease == false &&
                ijp.GetCirculationControl().OperationTimes.Ink > 0 &&
                ijp.GetCirculationControl().EnvironmentSetup.InkConcentrationControl == true
            )
                return true;
            return false;
        }

        //удачно ли завершилась печать
        public bool PrintingResultSuccess()
        {
            if
            (
                ijp.GetPrintingJobProgress().Status == IJPPrintingJobStatus.Completion &&
                ijp.GetPrintingManagement().Error == IJPPrintingManagementError.Confirmation
            )
                return true;
            return false;
        }

        //проверка печатает ли принтер в данный момент
        public bool IsPrintInProgress()
        {
            IIJPPrintingJobProgress printingJobProgress = ijp.GetPrintingJobProgress();

            switch (printingJobProgress.Status)
            {
                case IJPPrintingJobStatus.Completion:
                    return false;

                //если не завершилась + принтер движется то печать в прогрессе
                case IJPPrintingJobStatus.Noncompletion:
                    return ijp.GetPrintingManagement().DeviceMovement;

                case IJPPrintingJobStatus.AbnormalEnd:
                    return false;

                default:
                    return false;

            }
        }
    }
}
