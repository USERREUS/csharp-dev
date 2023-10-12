using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIES.IJP.RX;

namespace TestClasses
{
    //Класс различной информации
    public class InformationIJP
    {
        IJP ijp;
        //IIJPUnitInformation ijpUnitInfo; //информация о принтере
        //IIJPCirculationControl ijpCircControl; //информация о циркуляции воздуха/температуре
        //IIJPSoftwareVersion ijpSoftwareVersion; //информация о версиях

        public InformationIJP(IJP ijp)
        {
            this.ijp = ijp;
            //ijpUnitInfo = ijp.GetUnitInformation();
            //ijpCircControl = ijp.GetCirculationControl();
            //ijpSoftwareVersion = ijp.GetSoftwareVersion();
        }

        //получить ID чернил
        public string GetInkType() => ijp.GetUnitInformation().InkTypeName;

        //получить режим ввода
        //1 - по умолч, 2 - локальный язык
        public IJPInputMode GetInputMode() => ijp.GetUnitInformation().InputMode;

        //серийный номер принта как беззнаковое целое
        public uint GetSerialNumber() => ijp.GetUnitInformation().SerialNumber;

        //айди вида принтера
        public string GetTypeId() => ijp.GetUnitInformation().TypeName;

        //время работы элемента нагрева
        public ushort GetHeatingUnitOpTime() => ijp.GetCirculationControl().OperationTimes.HeatingUnit;

        //время работы чернил
        public uint GetInkOpTime() => ijp.GetCirculationControl().OperationTimes.Ink;

        //время работы фильтра чернил
        public uint GetInkFilterOpTime() => ijp.GetCirculationControl().OperationTimes.InkFilter;

        //время работы впускного фильтра
        public ushort GetIntakeFilterOpTime() => ijp.GetCirculationControl().OperationTimes.IntakeFilter;


        //время работы сборки(?)
        public uint GetMakeupOpTime() => ijp.GetCirculationControl().OperationTimes.Makeup;

        //время работы фильтра сборки
        public ushort GetMakeupFilterOpTime() => ijp.GetCirculationControl().OperationTimes.MakeupFilter;

        //счетчик печати
        public uint GetPrinterCount() => ijp.GetCirculationControl().OperationTimes.PrintCount;

        //время работы насоса/помпы
        public ushort GetPumpOpTime() => ijp.GetCirculationControl().OperationTimes.Pump;

        //время работы фильтра восстановления
        public ushort GetRecoveryFilterOpTime() => ijp.GetCirculationControl().OperationTimes.RecoveryFilter;

        //время работы вискозиметра
        public ushort GetViscometerOpTime() => ijp.GetCirculationControl().OperationTimes.ViscometerFilter;

        //версия базового софта
        public string GetBasicSoftwareVersion() => ijp.GetSoftwareVersion().BasicSoftware;

        //версия софта контроллера
        public string GetControllerSoftwareVersion() => ijp.GetSoftwareVersion().ControllerSoftware;

        //получить аналитическую информацию
        public byte[] GetAnalysisInfo()
        {
            byte[] arr = new byte[4];
            IIJPStatus status = ijp.GetStatus();

            arr[0] = status.AnalysisInformation1;
            arr[1] = status.AnalysisInformation2;
            arr[2] = status.AnalysisInformation3;
            arr[3] = status.AnalysisInformation4;

            return arr;
        }

        //получить кол-во напечатанных объектов
        public uint GetPrintedCount() => ijp.GetPrintingJobProgress().Count;

    }
}
