using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIES.IJP.RX;

namespace TestClasses
{
    //Класс для тестирования приложения
    //Общая структура тестового метода
    //сигнатура - public void DoSmth() {}
    //изнутри теста можно обращаться к чему угодно
    //все значения, которые необходимо вернуть - возвращать через метод вывода на экран класса Provider
    //метод DataOutput принимает массив строк 
    //!!!Все тестовые методы необходимо занести в список List<Test> как показано ниже!!!
    internal class Tests
    {
        protected internal IJPModel ijpModel;

        public Tests(Action<string[]> dataOutputMethod, IJPModel ijpModel)
        {
            this.ijpModel = ijpModel;
            Provider.DataOutputMethod += dataOutputMethod;
        }

        //проверка соединения
        public void Test_CorrectlyConnecting()
        {
            var onlineStatus = ijpModel.ijp.GetComPort();
            var comConnectionStatus = ijpModel.ijp.GetStatus().CommunicationConnection;

            if ((int)onlineStatus == 0)
            {
                throw new Exception("Принтер оффлайн");
            }

            if ((int)comConnectionStatus == 48)
            {
                throw new Exception("Статус соединения связи - оффлайн");
            }
        }

        public void Test_Disconnect()
        {
            ijpModel.ijp.Disconnect();
        }

        public virtual List<Test> GetTests()
        {
            throw new NotImplementedException();
        }
    }

    //Класс для обложения тестами базовых функций
    internal class FunctionTests : Tests
    {
        public FunctionTests(Action<string[]> dataOutputMethod, IJPModel ijpModel) : base(dataOutputMethod, ijpModel) { }
        public override List<Test> GetTests()
        {
            return new List<Test>()
            {
                new Test("Тест инициализации", FunctionTest0_InitTest),
                new Test("Тест соединнения", Test_CorrectlyConnecting),
                new Test("Статус принтера", FunctionTest1_StatusIJP),
                new Test("Выбор сообщения", FunctionTest2_SelectMessage),
                new Test("Сохранение сообщения", FunctionTest3_SaveMessage),
                new Test("Управление сообщением", FunctionTest4_ManageMessage),
                new Test("Переименование сообщения", FunctionTest5_RenameMessage),
                new Test("Правило замены", FunctionTest6_SubstitutionRule),
                new Test("Условия подсчета времени", FuntcionTest7_TimeCountConditions),
                new Test("Коды сдвига", FunctionTest8_ShiftCode),
                new Test("Условия подсчета", FunctionTest9_CountCondition),
                new Test("Различные настройки печати", FunctionTest10_VariousPrintSetup),
                new Test("Дистанционное управление", FunctionTest11_RemoteOperation),
                new Test("Управление группами", FunctionTest12_GroupManage),
                new Test("Настройки коммуникации", FunctionTest13_CommunicationEnvironment),
                new Test("Управление операциями", FunctionTest14_OperationManagement),
                new Test("Настройки дисплея", FunctionTest15_DisplayEnvSetup),
                new Test("Контроль циркуляции", FunctionTest16_CirculationControl),
                new Test("Прогресс работы печати", FunctionTest17_PrintingJobProgress),
                new Test("Настройки пользовательской среды", FunctionTest18_UserEnvironmentSetup),
                new Test("Управление печатью", FunctionTest19_PrintingManagement),
                new Test("Отключение", Test_Disconnect)
            };
        }
        public void FunctionTest0_InitTest()
        {
            Provider.DataOutputMethod(new string[]
            {
                "Тесты на заявленные функции"
            });
        }
        public void FunctionTest1_StatusIJP()
        {
            Provider.DataOutputMethod(new string[]
            {
                ijpModel.ijp.GetStatus().CommunicationConnection.ToString(),
                ijpModel.ijp.GetStatus().ReceiveEnableDisable.ToString(),
                ijpModel.ijp.GetStatus().Operation.ToString(),
                ijpModel.ijp.GetStatus().Warning.ToString(),
                ijpModel.ijp.GetStatus().AnalysisInformation1.ToString(),
                ijpModel.ijp.GetStatus().AnalysisInformation2.ToString(),
                ijpModel.ijp.GetStatus().AnalysisInformation3.ToString(),
                ijpModel.ijp.GetStatus().AnalysisInformation4.ToString()
            });
        }

        public void FunctionTest2_SelectMessage()
        {
            //Считается, что на момент вызова теста в систему установлено и сохранено одно сообщение
            if (
                ijpModel.ijp.GetMessage() == null ||
                ijpModel.ijp.CallMessage(1) == null ||
                ijpModel.ijp.GetMessage() != ijpModel.ijp.CallMessage(1)
               )
                throw new Exception("Сообщение не загружено, или загружено больше 1 сообщения");
        }

        public void FunctionTest3_SaveMessage()
        {
            IJPMessage message = new IJPMessage();
            message.AddColumn();
            message.Items[0].Text = "ABC2";
            message.Items[0].Bold = 10;
            message.CharacterHeight = 100;
            ijpModel.ijp.SaveMessage(0, 2, "SampleData2");
        }

        public void FunctionTest4_ManageMessage()
        {
            if (ijpModel.ijp.CallMessage(2) == null)
                throw new Exception("Сообщение под номером 2 не зарегистрировано");
            var messageInfos = ijpModel.ijp.ListMessage(1, 2);
            if (messageInfos.Length != 2)
                throw new Exception("Список содержит число сообщений, отличное от 2");
            Provider.DataOutputMethod(new string[] { messageInfos[0].Nickname, messageInfos[1].Nickname });
            ijpModel.ijp.DeleteMessage(2);
        }

        public void FunctionTest5_RenameMessage()
        {
            var message = ijpModel.ijp.CallMessage(1);
            if (message == null)
                throw new Exception("Сообщения под номером 1 не существует");
            ijpModel.ijp.DeleteMessage(1);
            ijpModel.ijp.SaveMessage(0, 1, "NewSampleData1");
            if (ijpModel.ijp.CallMessage(1) != ijpModel.ijp.GetMessage())
                throw new Exception("Сообщение, сохраненное под номером 1 не соответствует установленному");
            Provider.DataOutputMethod(new string[] { ijpModel.ijp.CallMessage(1).Nickname });
        }

        public void FunctionTest6_SubstitutionRule()
        {
            var subRule = new IJPSubstitutionRule();
            IJPSetuper.SubstitutionRuleSetup(subRule, "SubRule1", 1, 0, 10, "Ten", 2021, "Now");
            ijpModel.ijp.SetSubstitutionRule(subRule);
            var subRuleNew = ijpModel.ijp.GetSubstitutionRule(1);
            if (subRuleNew == null)
                throw new Exception("Под номером 1 не зарегистрировано правила замены");
            Provider.DataOutputMethod(new string[]
            {
                subRuleNew.GetDaySetup(10),
                subRuleNew.GetYearSetup(2021)
            });
        }

        public void FuntcionTest7_TimeCountConditions()
        {
            var tcc = new IJPTimeCountCondition();
            tcc.LowerRange = "00:00:00";
            tcc.UpperRange = "11:59:59";
            //Скорее всего тут 16сс Юникод
            ijpModel.ijp.CallMessage(1).TimeCount = tcc;
        }

        public void FunctionTest8_ShiftCode()
        {
            var sc = ijpModel.ijp.CallMessage(1).ShiftCodes;
            if (sc.Count == 0)
                throw new Exception("Коллекция кодов сдвига пуста");
            Provider.DataOutputMethod(new string[] { $"Колличество элементов в коллекции сдвигов {sc.Count}" });
            for (int i = 0; i < sc.Count; i++)
                Provider.DataOutputMethod(new string[] { $"Сдвиг: {sc[i].String}, Время: {sc[i].StartTime.Hour} : {sc[i].StartTime.Minute}" });
        }

        public void FunctionTest9_CountCondition()
        {
            var cc = ijpModel.ijp.CallMessage(1).CountConditions;
            if (cc.Count == 0)
                throw new Exception("Коллекция условий подсчета пуста");
            Provider.DataOutputMethod(new string[] { $"Колличество элементов в коллекции сдвигов {cc.Count}" });
            for (int i = 0; i < cc.Count; i++)
                Provider.DataOutputMethod(new string[] { $"Значение условия подсчета: {cc[i].Value}" });
        }

        public void FunctionTest10_VariousPrintSetup()
        {
            var ps = ijpModel.ijp.CallMessage(1).VariousPrintSetup;
            Provider.DataOutputMethod(new string[]
            {
                ps.BarcodePrinting.ToString(),
                ps.CalendarOffset.ToString(),
                ps.DINPrint.ToString(),
                ps.EANPrefix.ToString(),
                ps.QRErrorCorrectionLevel.ToString(),
                ps.ToString()
            });
        }

        public void FunctionTest11_RemoteOperation()
        {
            ijpModel.ijp.SetRemoteOperation(IJPRemoteOperation.Reset);
        }

        public void FunctionTest12_GroupManage()
        {
            ijpModel.ijp.CreateGroup(1, "NameOfGroup");
            ijpModel.ijp.RenameGroup(1, "NameOfGroupChanged");
            ijpModel.ijp.RenumberGroup(1, 2);
            var listGroup = ijpModel.ijp.ListGroup(0, 2);
            foreach (var item in listGroup)
                if (item != null)
                    Provider.DataOutputMethod(new string[]
                    {
                            $"Number: {item.Number}",
                            $"Name: {item.Name}"
                    });
        }

        public void FunctionTest13_CommunicationEnvironment()
        {
            var ce = ijpModel.ijp.GetCommunicationEnvironmentSetup();
            Provider.DataOutputMethod(new string[]
            {
                ce.CommunicationAndSignalError.ToString(),
                ce.CommunicationMode.ToString(),
                ce.Condition.ToString(),
                ce.DataExchange.ToString()
            });
        }

        public void FunctionTest14_OperationManagement()
        {
            var om = ijpModel.ijp.GetOperationManagement();
            Provider.DataOutputMethod(new string[]
            {
                om.PrintCount.ToString()
            });
        }

        public void FunctionTest15_DisplayEnvSetup()
        {
            var des = ijpModel.ijp.GetDisplayEnvironmentSetup();
            Provider.DataOutputMethod(new string[]
            {
                des.KeyboardLayout.ToString(),
                des.ArabicInputMethod.ToString(),
                des.ClockDisplayFormat.ToString(),
                des.Display.ToString()
            });
        }

        public void FunctionTest16_CirculationControl()
        {
            var cc = ijpModel.ijp.GetCirculationControl();
            Provider.DataOutputMethod(new string[]
            {
                cc.EnvironmentSetup.CoolingFanUnit.ToString(),
                cc.OperationTimes.PrintCount.ToString()
            });
        }

        public void FunctionTest17_PrintingJobProgress()
        {
            var pjp = ijpModel.ijp.GetPrintingJobProgress();
            Provider.DataOutputMethod(new string[]
            {
                pjp.Count.ToString(),
                pjp.Status.ToString()
            });
        }

        public void FunctionTest18_UserEnvironmentSetup()
        {
            var ues = ijpModel.ijp.GetUserEnvironmentSetup();
            Provider.DataOutputMethod(new string[]
            {
                ues.ChangeCharacterOrientation.ToString(),
                ues.ChangeMode.ToString(),
                ues.EndMessageNumber.ToString(),
                ues.StartMessageNumber.ToString(),
                ues.PrintCharactersOneByOne.ToString()
            });
        }

        public void FunctionTest19_PrintingManagement()
        {
            var pm = ijpModel.ijp.GetPrintingManagement();
            Provider.DataOutputMethod(new string[]
            {
                pm.AlarmType.ToString(),
                pm.DeviceMovement.ToString(),
                pm.Enable.ToString(),
                pm.Error.ToString(),
                pm.Message.ToString()
            });
        }
    }

    //Класс для обложения тестами наших наработок
    internal class AdvancedTests : Tests
    {
        public AdvancedTests(Action<string[]> dataOutputMethod, IJPModel ijpModel) : base(dataOutputMethod, ijpModel) { }
        public override List<Test> GetTests()
        {
            return new List<Test>()
            {
                new Test("Инициализация", Test0_InitTest),
                new Test("Проверка соединения", Test_CorrectlyConnecting),
                new Test("Проверка класса печати информации", Test2_AllAvailableInformation),

                //запускать этот тест осторожно, хз что будет если принтер начнет печатать просто так
                //new Test("Проверка функции, которая запускает печать", Test3_StartPrinting),
                //new Test("Проверка состояния принтера после начала печати", Test4_AfterPrintingCheck),

                //Дальше будет фрагмент с печатью DM - кода 
                //new Test("Проверка функции, которая запускает печать DM кода", Test5_DMCodePrint),
                //new Test("Проверка состояния принтера после начала печати", Test4_AfterPrintingCheck),
                new Test("Отключение", Test_Disconnect)
            };
        }

        public void Test0_InitTest()
        {
            Provider.DataOutputMethod(new string[]
            {
                "Тесты сложных функций"
            });
        }

        public void Test2_AllAvailableInformation()
        {
            Provider.DataOutputMethod(new string[]
            {
                Convert.ToString(ijpModel.information.GetAnalysisInfo())
              + Convert.ToString(ijpModel.information.GetBasicSoftwareVersion())
              + Convert.ToString(ijpModel.information.GetControllerSoftwareVersion())
              + Convert.ToString(ijpModel.information.GetHeatingUnitOpTime())
              + Convert.ToString(ijpModel.information.GetInkFilterOpTime())
              + Convert.ToString(ijpModel.information.GetInkOpTime())
              + Convert.ToString(ijpModel.information.GetInkType())
              + Convert.ToString(ijpModel.information.GetInputMode())
              + Convert.ToString(ijpModel.information.GetIntakeFilterOpTime())
              + Convert.ToString(ijpModel.information.GetMakeupFilterOpTime())
              + Convert.ToString(ijpModel.information.GetMakeupOpTime())
              + Convert.ToString(ijpModel.information.GetPrinterCount())
              + Convert.ToString(ijpModel.information.GetPrintedCount())
              + Convert.ToString(ijpModel.information.GetPumpOpTime())
              + Convert.ToString(ijpModel.information.GetRecoveryFilterOpTime())
              + Convert.ToString(ijpModel.information.GetSerialNumber())
              + Convert.ToString(ijpModel.information.GetTypeId())
              + Convert.ToString(ijpModel.information.GetViscometerOpTime())
            });
        }

        public void Test3_StartPrinting()
        {
            ijpModel.manage.StartPrinting(1);
        }

        public void Test4_AfterPrintingCheck()
        {
            Controller controller = new Controller(ijpModel.ijp);
            Provider.DataOutputMethod(new string[] { controller.IsPrintInProgress().ToString(), controller.PrintingResultSuccess().ToString() });
        }

        public void Test5_DMCodePrint()
        {
            if (ijpModel.ijp.GetFreeUserPattern(1) == null)
                throw new NullReferenceException("Шаблон по номеру 1 не зарегистрирован в системе");

            var message = ijpModel.ijp.CallMessage(1);

            if(message == null)
                throw new NullReferenceException("Сообщение по номеру 1 не зарегистрировано в системе");

            ijpModel.ijp.DeleteMessage(1);
            message.Items[0].Text = "0xF209"; //16сс - код шаблона
            message.Items[0].Barcode = IJPBarcode.DM20x20;
            message.Items[0].Bold = 2;

            ijpModel.ijp.SetMessage(message);
            ijpModel.ijp.SaveMessage(1, 1, "DM - code");

            ijpModel.manage.StartPrinting(1);
        }
    }

    internal class BaseTests : Tests
    {
        public BaseTests(Action<string[]> dataOutputMethod, IJPModel ijpModel) : base(dataOutputMethod, ijpModel) { }
        public override List<Test> GetTests()
        {
            return new List<Test>()
            {
                new Test("Инициализация", BaseTest0_InitTest),
                new Test("Тест 1 проверка соединения", Test_CorrectlyConnecting),
                new Test("Тест 2 получения статуса", BaseTest2_AcquireEquipmentStatus),
                new Test("Тест 3 установки информации на печать", BaseTest3_SettingIJPPrintDataWhileEditing),
                new Test("Тест 4 получения информации на печать", BaseTest4_AcquiringIJPPrintDataWhileEditing),
                new Test("Тест 5 получения списка уже загруженной информации на печать", BaseTest5_AcquirungAListOfIJPRegisteredPrintData),
                new Test("Тест 6 вызова уже зарегистрированной информации на печать для измеенения", BaseTest6_CallingIJPRegisteredPrintDataToEdit),
                new Test("Тест 7 установки календаря для печати", BaseTest7_SettingCalendarCharacterForPrintContent),

                //перед запуском теста 7 указать пути к изображению в коде теста ниже
                new Test("Тест 8 загрузки изображения DM/QR кода", BaseTest8_RegisteringUserPatternFreeSize),
                new Test("Тест 9 установки текущей даты времени на часы принтера", BaseTest9_SettingCurrentTimeOfIJP),
                new Test("Тест 10 установка календарного времени на печать", BaseTest10_SettingCalendarTimeOfIJP),
                new Test("Тест 11 проверка метода AllIsOk", BaseTest11_AllIsOk),
                new Test("Тест 12 проверка метода проверяющего состояние чернил", BaseTest12_InkChecks),
                new Test("Тест 13 проверка метода проверяющего может ли принтер начать печать", BaseTest13_CanStartPrint),
                new Test("Отключение", Test_Disconnect)
            };
        }
        //BТесты на базовые функции
        public void BaseTest0_InitTest()
        {
            Provider.DataOutputMethod(new string[]
            {
                "Базовые тесты"
            });
        }
        //Вместо него добавлю тест корректности соединения
        public void BaseTest1_CurrectlyConnectionTest()
        {
            Provider.DataOutputMethod(new string[] { ijpModel.ijp.GetComPort().ToString() /*???*/});
        }
        public void BaseTest2_AcquireEquipmentStatus()
        {
            Provider.DataOutputMethod(new string[] { ijpModel.ijp.GetStatus().CommunicationConnection.ToString() });
        }
        public void BaseTest3_SettingIJPPrintDataWhileEditing()
        {
            var messageSetuper = new MessageSetupManager.SeparativeOrCollectiveMessageSetuper(new IJPMessage());
            messageSetuper.AddColumn();
            messageSetuper.message.Items[0].Text = "Test";
            messageSetuper.message.Items[0].Bold = 5;
            messageSetuper.message.CharacterHeight = 80;
            ijpModel.ijp.SetMessage(messageSetuper.message);
            ijpModel.ijp.SaveMessage(0, 1, "SampleData1");
        }
        public void BaseTest4_AcquiringIJPPrintDataWhileEditing()
        {
            Provider.DataOutputMethod(new string[] { ijpModel.ijp.GetMessage().Nickname });
        }
        public void BaseTest5_AcquirungAListOfIJPRegisteredPrintData()
        {
            foreach (var item in ijpModel.ijp.ListMessage(1, 1))
                Provider.DataOutputMethod(new string[] { item.Nickname });
        }
        public void BaseTest6_CallingIJPRegisteredPrintDataToEdit()
        {
            Provider.DataOutputMethod(new string[] { ijpModel.ijp.CallMessage(1).Nickname });
        }
        public void BaseTest7_SettingCalendarCharacterForPrintContent()
        {
            var message = new IJPMessage();
            message.Items[0].Text = "Date Time:{YYYY/MM/DD}";
            Provider.DataOutputMethod(new string[] { message.CalendarConditions.ToString() });
        }
        //тест добавления DM/QR кода в память принтера
        public void BaseTest8_RegisteringUserPatternFreeSize()
        {
            //Проверить путь к Bitmap файлу
            //Для этого нажать правой кнопкой мыши на bmp файл в проекте и нажать - копировать полный путь
            //Вставить полный путь ниже
            var userPattern = new IJPFreeUserPattern(1, new System.Drawing.Bitmap("Тут пистать путь"));
            ijpModel.ijp.SetFreeUserPattern(userPattern);
            Provider.DataOutputMethod(new string[] { ijpModel.ijp.GetFreeUserPattern(1).Pattern.ToString() });
        }
        public void BaseTest9_SettingCurrentTimeOfIJP()
        {
            ijpModel.ijp.SetCurrentDateTime(DateTime.Now);
            Provider.DataOutputMethod(new string[] { ijpModel.ijp.GetCurrentDateTime().ToString() });
        }
        public void BaseTest10_SettingCalendarTimeOfIJP()
        {
            ijpModel.ijp.SetPrintingCalendarDateControl(true);
            ijpModel.ijp.SetPrintingCalendarDateTime(new DateTime(2000, 1, 1));
            Provider.DataOutputMethod(new string[]
            {
                ijpModel.ijp.GetPrintingCalendarDateControl().ToString(),
                ijpModel.ijp.GetPrintingCalendarDateTime().ToString()
            });
        }

        //тест проверки на ошибки/предупреждения принтера
        public void BaseTest11_AllIsOk()
        {
            CheckerIJP checker = new CheckerIJP(ijpModel.ijp);
            Provider.DataOutputMethod(new string[] { checker.AllIsOk().ToString() });
        }

        //тест проверки чернил
        public void BaseTest12_InkChecks()
        {
            Controller controller = new Controller(ijpModel.ijp);
            Provider.DataOutputMethod(new string[] { controller.InkIsOk().ToString() });
        }

        //тест на проверку может ли принтер начать печать и печатает ли он сейчас
        public void BaseTest13_CanStartPrint()
        {
            Controller controller = new Controller(ijpModel.ijp);
            Provider.DataOutputMethod(new string[]
            {   controller.CanStartPrint().ToString(),
                controller.IsPrintInProgress().ToString()
            });
        }
    }
}
