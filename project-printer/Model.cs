using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIES.IJP.RX;

namespace TestClasses
{
    //Модель, согласно паттерну работы с графическими приложениями MVC
    class IJPModel
    {
        public IJPModel(IJP ijp)
        {
            this.ijp = ijp;
            this.manage = new ManageClass(ijp);
            this.information = new InformationIJP(ijp);
        }

        public IJP ijp;
        public ManageClass manage;
        public InformationIJP information;
    }
}
