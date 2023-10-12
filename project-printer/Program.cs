using HIES.IJP.RX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Numerics;
using System.Windows.Forms;
using System.Drawing;
using test;
namespace TestClasses
{
    class Program
    {
        public static void Main()
        {
            var ijp = new IJP();
            var ijpModel = new IJPModel(ijp);
            var form = new MyForm(ijpModel);

            //здесь поменять на new AdvancedTests(form.DataOutputMethod, ijpModel)
            //для запуска второй пачки тестов
            var tests = new BaseTests(form.DataOutputMethod, ijpModel);
            form.tests = tests.GetTests();
            //Поменять для запуска тестов в случае проблем с доступностью кнопки
            //form.buttonRunTests.Enabled = true;
            Application.Run(form);
        }
    }
}
