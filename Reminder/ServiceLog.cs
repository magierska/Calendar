using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder
{
    public static class ServiceLog
    {
        public static void WriteErrorLog(Exception e)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\ReminderLogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + e.Source.ToString().Trim() + "; " + e.Message.ToString().Trim());
                sw.Flush();
                sw.Close();
            }
            catch(Exception)
            {

            }
        }

        public static void WriteErrorLog(string message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\ReminderLogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + message);
                sw.Flush();
                sw.Close();
            }
            catch(Exception)
            {

            }
        }
    }
}
