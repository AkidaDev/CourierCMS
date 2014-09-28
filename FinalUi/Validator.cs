using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalUi
{
    static class  Validator
    {
        public static bool ClientV(Client client)
        {
            return true;
        }
        public static bool PermissionV(Permission per)
        {
            return true;
        }

        public static bool EmployeeV(Employee emp)
        {
            return true;
        }
        public static bool StockV(Stock s)
        {
            return true;
        }
    }
}