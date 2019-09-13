using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Util
{
    public static class BarCodeUtil
    {
        public static string GetBarCodeString(long number)
        {
            if (number.ToString().Length == 1)
                return "0000000" + number.ToString();
            else if (number.ToString().Length == 2)
                return "000000" + number.ToString();
            else if (number.ToString().Length == 3)
                return "00000" + number.ToString();
            else if (number.ToString().Length == 4)
                return "0000" + number.ToString();
            else if (number.ToString().Length == 5)
                return "000" + number.ToString();
            else if (number.ToString().Length == 6)
                return "00" + number.ToString();
            else if (number.ToString().Length == 7)
                return "0" + number.ToString();
             else
                return number.ToString();           
        }
    }
}
