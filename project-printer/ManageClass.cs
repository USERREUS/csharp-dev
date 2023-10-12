using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIES.IJP.RX;

namespace TestClasses
{
    //Класс управления IJP
    public class ManageClass
    {
        IJP ijp;
        Controller controller;
        System.Timers.Timer timer;

        public ManageClass(IJP ijp)
        {
            this.ijp = ijp;
            controller = new Controller(ijp);
        }

        //установка информации для печати
        public void SettingUpPrintData(string text, byte bold, byte characterHeight, IJPFormatSetup setup)
        {
            if (controller.checker.AllIsOk())
            {
                try
                {
                    //создание сообщения
                    IIJPMessage message = new IJPMessage();
                    message.SetFormatSetup(setup);

                    if (message.FormatSetup == IJPFormatSetup.FreeLayout)
                        message.AddItem();
                    else
                        message.AddColumn();

                    message.Items[0].Text = text;
                    message.Items[0].Bold = bold;
                    message.CharacterHeight = characterHeight;

                    ijp.SetMessage(message);

                    AttachNickame(1, 1, "startMsg");
                }
                catch (Exception exception)
                {
                    Provider.DataOutputMethod(new string[] { exception.Message });
                }
            }
        }

        //возвращает список эл-ты которого несут информацию о сообщениях
        public IIJPMessageInfo[] GetListRegisteredPrintData(int start, int end)
        {
            return ijp.ListMessage(start, end);
        }

        //присваивает имя сообщения + сохраняет в памяти принтера
        public void AttachNickame(byte groupN, byte msgN, string nickname)
        {
            ijp.SaveMessage(groupN, msgN, nickname);
        }

        //получение сохраненных в памяти принтера сообщений
        public IIJPMessage RecallingRegisteredPrintData(byte registeredNo)
        {
            return ijp.CallMessage(registeredNo);
        }

        //установление сообщения, которое печатает дату встроенного календаря
        //пример dateFormat = YYYY/MM/DD
        public void SetCalendarCharacter(string dateFormat, IIJPMessage message)
        {
            message.Items[0].Text = $"Data Time:{dateFormat}";
        }

        //зарегистрировать пользовательский шаблон свободного типа
        public void RegisteredFreeUserPattern(int number, System.Drawing.Bitmap bitmap)
        {
            IJPFreeUserPattern newPattern = new IJPFreeUserPattern(number, bitmap);
            ijp.SetFreeUserPattern(newPattern);
        }

        //установить номер шаблона, который будет печатать
        //устанавливается в 16сс юникодом
        //для свободных шаблонов диапазон 0xF209 до 0xF23A
        public void SetFreeUserPattern(IJPMessage message, string patternUnicode)
        {
            message.Items[0].Text = patternUnicode;
        }

        //установка времени и даты на принтере
        public void SetDateTime(DateTime dateTime)
        {
            ijp.SetCurrentDateTime(dateTime);
        }

        //установка время и даты календаря (это время и дата которые печатаются)
        public void SetPrintingCalendarTime(DateTime dateTime)
        {
            ijp.SetPrintingCalendarDateControl(true);
            ijp.SetPrintingCalendarDateTime(dateTime);
        }

        //старт печати
        public void StartPrinting(uint numOfPrintJobs)
        {
            try
            {
                //инициализация таймера
                timer = new System.Timers.Timer();
                timer.Interval = 500; //0.5 секунд

                try
                {
                    //если не все ОК с принтером то печать останавливается
                    //внутри события Elapsed
                    timer.Elapsed += CheckIfAllIsOk_Elapsed;
                    timer.Enabled = true;

                    try
                    {
                        if (!controller.CanStartPrint())
                            throw new IJPException("Не удается осуществить начало печати");
                        //запуск печати
                        if (!controller.IsPrintInProgress())
                        {
                            ijp.StartPrintingJob(numOfPrintJobs);
                            Provider.DataOutputMethod(new string[] { "Печать началась" });
                        }
                        else
                        {
                            Provider.DataOutputMethod(new string[] { "Печать уже идет" });
                        }

                    }
                    catch (IJPException e)
                    {
                        //остановка печати + ошибка
                        Provider.DataOutputMethod(new string[] { e.Message });

                        if (controller.IsPrintInProgress())
                            ijp.StopPrintingJob();
                    }
                }

                catch (Exception e)
                {
                    ijp.StopPrintingJob();
                    Provider.DataOutputMethod(new string[] { "Ошибка при работе с Elapsed событием" });
                    Provider.DataOutputMethod(new string[] { e.Message });
                }

            }
            catch (Exception e)
            {
                Provider.DataOutputMethod(new string[] { "Ошибка при инициализации таймера" });
                Provider.DataOutputMethod(new string[] { e.Message });
            }


        }

        //события для Elapsed таймера
        public void CheckIfAllIsOk_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            if (!controller.IsPrintInProgress())
            {
                timer.Enabled = false;
                timer.Dispose();
            }

            if (!controller.checker.AllIsOk())
            {
                timer.Enabled = false;
                //остановка печати
                ijp.StopPrintingJob();
                timer.Dispose();
            }
        }

    }
}
