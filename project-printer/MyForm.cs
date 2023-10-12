using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIES.IJP.RX;

namespace TestClasses
{
    //Форма Виндовс для тестирования
    class MyForm : Form
    {
        public List<Test> tests;
        //Элементы формы
        IJPModel ijpModel;
        TableLayoutPanel table;
        TableLayoutPanel tableEmbedded;
        //ПОТОМ УБРАТЬ ПУБЛИК
        public Button buttonRunTests;
        Button buttonConnect;
        Button buttonClear;
        Label labelStatus;
        Label labelIpAddress;
        Label labelTimeout;
        Label labelRetry;
        TextBox textBoxStatus;
        TextBox textBoxIpAddress;
        TextBox textBoxTimeout;
        TextBox textBoxRetry;

        public MyForm(IJPModel ijpModel)
        {
            tests = new List<Test>();
            this.ijpModel = ijpModel;
            Size = new System.Drawing.Size(500, 500);

            table = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill
            };

            tableEmbedded = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill
            };

            labelIpAddress = new Label()
            {
                Dock = DockStyle.Fill,
                Text = "IP Addr. :",
                Font = new Font("Times New Roman", 12)
            };

            textBoxIpAddress = new TextBox()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };

            labelTimeout = new Label()
            {
                Dock = DockStyle.Fill,
                Text = "Timeout:",
                Font = new Font("Times New Roman", 12)
            };

            textBoxTimeout = new TextBox()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };

            labelRetry = new Label()
            {
                Dock = DockStyle.Fill,
                Text = "Retry:",
                Font = new Font("Times New Roman", 12)
            };

            textBoxRetry = new TextBox()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };

            buttonRunTests = new Button()
            {
                Dock = DockStyle.Fill,
                Text = "Run Tests",
                Enabled = false,
                BackColor = Color.LightGray
            };

            buttonRunTests.Click += (sender, args) => RunTests();

            labelStatus = new Label()
            {
                Text = "Status IJP:",
                Dock = DockStyle.Bottom
            };

            textBoxStatus = new TextBox()
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Both,
                ReadOnly = true
            };

            buttonClear = new Button()
            {
                Dock = DockStyle.Fill,
                Text = "Clear",
                BackColor = Color.LightGray
            };

            buttonClear.Click += (sender, args) => textBoxStatus.Clear();

            buttonConnect = new Button()
            {
                Dock = DockStyle.Fill,
                Text = "Connect to IJP",
                BackColor = Color.LightGray
            };

            buttonConnect.Click += (sender, args) => Connect(ijpModel, textBoxIpAddress, textBoxTimeout, textBoxRetry, textBoxStatus);

            {
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 10));

                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                table.Controls.Add(tableEmbedded, 0, 0);
                table.Controls.Add(buttonConnect, 0, 1);
                table.Controls.Add(buttonRunTests, 0, 2);
                table.Controls.Add(labelStatus, 0, 3);
                table.Controls.Add(textBoxStatus, 0, 4);
                table.Controls.Add(buttonClear, 0, 5);

                table.Dock = DockStyle.Fill;
            }

            {
                tableEmbedded.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

                for (int i = 0; i < 6; i++)
                    tableEmbedded.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 6));

                tableEmbedded.Controls.Add(labelIpAddress, 0, 0);
                tableEmbedded.Controls.Add(textBoxIpAddress, 1, 0);
                tableEmbedded.Controls.Add(labelTimeout, 2, 0);
                tableEmbedded.Controls.Add(textBoxTimeout, 3, 0);
                tableEmbedded.Controls.Add(labelRetry, 4, 0);
                tableEmbedded.Controls.Add(textBoxRetry, 5, 0);
            }

            Controls.Add(table);

            FormClosing += (sender, eventArgs) =>
            {
                var result = MessageBox.Show("Действительно закрыть?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    eventArgs.Cancel = true;
            };
        }

        public void Connect(IJPModel ijpModel, TextBox tbIpAdress, TextBox tbTimeout, TextBox tbRetry, TextBox status)
        {
            if (tbIpAdress.Text.Length == 0 || tbTimeout.Text.Length == 0 || tbRetry.Text.Length == 0)
            {
                MessageBox.Show("Заполните данные и повторите попытку", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                int timeout = Convert.ToInt32(tbTimeout.Text);
                int retry = Convert.ToInt32(tbRetry.Text);
                ijpModel.ijp = new IJP(tbIpAdress.Text, timeout, retry);
                ijpModel.ijp.Connect();
                Provider.DataOutputMethod(new string[] 
                                                        { 
                                                            $"Статус подключения: {ijpModel.ijp.GetStatus().CommunicationConnection.ToString()}",
                                                             "Запуск тестов доступен"
                                                        });
                buttonRunTests.Enabled = true;
                ijpModel.information = new InformationIJP(ijpModel.ijp);
                ijpModel.manage = new ManageClass(ijpModel.ijp);
            }
            catch (Exception e)
            {
                status.Text += DateTime.Now + " : " + PrintOnTextBox(e.Message, e.StackTrace);
            }
        }

        public static string PrintOnTextBox(params string[] str)
        {
            string result = "";
            foreach (var item in str)
                result += item + Environment.NewLine;
            return result;
        }

        public void DataOutputMethod(params string[] str)
        {
            textBoxStatus.Text += PrintOnTextBox(str);
        }

        public void RunTests()
        {
            foreach (var test in tests)
                test.TestImplement();
        }
    }
}
