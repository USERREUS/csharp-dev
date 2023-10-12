using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIES.IJP.RX;

namespace test
{
    //Класс настройки сообщения для печати
    public class MessageSetupManager
    {
        public class LayoutMessageSetuper
        {
            public IJPMessage message;
            public LayoutMessageSetuper(IJPMessage message)
            {
                if (message == null)
                    throw new NullReferenceException();
                this.message = message;
            }
        }

        //настройщик сообщений со свободной компановкой    
        public class FreeLayoutMessageSetuper : LayoutMessageSetuper
        {
            public FreeLayoutMessageSetuper(IJPMessage message) : base(message)
            {
                if (message.FormatSetup != IJPFormatSetup.FreeLayout)
                    throw new IJPException("Компановка не является свободной");
            }
            public void AddItem() { message.AddItem(); }
            public void RemoveItem(IIJPMessageItem item) { message.RemoveItem(item); }
            public void RemoveItemAt(int index) { message.RemoveItemAt(index); }
        }

        ////настройщик сообщений с разделяющей или общей компановкой 
        public class SeparativeOrCollectiveMessageSetuper : LayoutMessageSetuper
        {
            public SeparativeOrCollectiveMessageSetuper(IJPMessage message) : base(message)
            {
                if (message.FormatSetup == IJPFormatSetup.FreeLayout)
                    throw new IJPException("Компановка не является раздельной или коллективной");
            }
            public int AddColumn() { return message.AddColumn(); }
            public int DeleteColumn(int nPos) { return message.DeleteColumn(nPos); }
            public int InsertColumn(int nPos) { return message.InsertColumn(nPos); }
            public int SetRow(int colPos, byte rowCount) { return message.SetRow(colPos, rowCount); }
        }
    }
}
