using SV20T1020285.DataLayers;
using SV20T1020285.DataLayers.SQLServer;
using SV20T1020285.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020285.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAcountDB;
        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;
            employeeAcountDB = new EmployeeAccountDAL(connectionString);
        }
        public static UserAccount? Authorize(string username, string password)
        {
            return employeeAcountDB.Authorize(username, password);
        }

        public static bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            return employeeAcountDB.ChangePassword(userName, oldPassword, newPassword);
        }
    }
}
