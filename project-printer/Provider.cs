using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClasses
{
    //Класс связи с внешним интерфейсом через делегаты
    public static class Provider
    {
        public static Action<string[]> DataOutputMethod;
        //public static Action<IJPStatus> StatusOutputMethod;
    }
}
