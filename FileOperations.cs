using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myOwnWebServer
{
    internal class FileOperations
    {
        public static void InitialLogCreation()
        {
            bool doesLogFileExist = File.Exists(Constants.kServerLogPath);

            try
            {
                using (StreamWriter writer = new StreamWriter(Constants.kServerLogPath, !doesLogFileExist))
                {
                    writer.WriteLine($"{DateTime.Now} [APPLICATION STARTED] - < Application started successfully >");
                }
            }
            catch (Exception)
            {
                CmdLineError.ServerLogError();
            }
        }

        public static void ServerStarted()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Constants.kServerLogPath, true))
                {
                    writer.WriteLine($"{DateTime.Now} [SERVER STARTED] - < Server started successfully >");
                }
            }
            catch (Exception)
            {
                CmdLineError.ServerLogError();
            }
        }
    }
}
