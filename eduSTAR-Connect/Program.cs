using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JR.Utils.GUI.Forms;

namespace eduSTAR_Connect
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();

            foreach (string arg in args)
            {
                string normalisedArg = arg.Replace("/", "-").ToLower();
                if (normalisedArg == "-?" || normalisedArg == "-h" || normalisedArg == "-help")
                {
                    string filename = System.AppDomain.CurrentDomain.FriendlyName;

                    FlexibleMessageBox.FONT = new System.Drawing.Font("Consolas", 8);

                    FlexibleMessageBox.Show(
                        "*************************************************************************************************************************************************************\r\n" +
                        "                                                    eduSTAR Connect Utility - ©  2016, Robert Brandon \r\n" +
                        "*************************************************************************************************************************************************************\r\n" +
                        "USAGE:\r\n" +
                        "    " + filename + " [-? | -h | -help] [-r | -reboot] [-s | -silent] [-xml <path>] [-cert <path>] [-pass <password> [-plaintext]]\r\n" +
                        "\r\n" +
                        "OPTIONS:\r\n" +
                        "    -? | -h | -help    Display this help message.\r\n" +
                        "    -r | -reboot       Reboot the computer after eduSTAR is configured.\r\n" +
                        "    -s | -silent       Don't display any popup messages.\r\n" +
                        "    -xml <path>        Install the wireless profile xml specified by <path>. Default: <current dir>\\eduSTAR.xml if exists, otherwise uses internal xml.\r\n" +
                        "    -cert <path>       Install the certificate specified by <path>. Default: <current dir>\\cert.pfx\r\n" +
                        "    -pass <password>   Use the Base64-encoded (or plaintext) password specified by <password>. Default: default eduSTAR password\r\n" +
                        "        -plaintext     Specifies that the password specified by \"-pass\" is plaintext. Default: disabled\r\n" +
                        "\r\n" +
                        "EXAMPLES:\r\n" +
                        "    " + filename + "                                                    ... Connect with all default values.\r\n" +
                        "    " + filename + " -r -s                                              ... Reboot when done, and don't display any popups.\r\n" +
                        "    " + filename + " -xml C:\\Temp\\eduSTAR.xml                           ... Install the WiFi profile at \"C:\\Temp\\eduSTAR.xml\".\r\n" +
                        "    " + filename + " -cert C:\\Temp\\cert.pfx                             ... Install the specified certificate using the default password.\r\n" +
                        "    " + filename + " -pass UGEkJHcwcmQ=                                 ... Install the default certificate using the specified Base64-encoded password.\r\n" +
                        "    " + filename + " -cert C:\\Temp\\cert.pfx -pass UGEkJHcwcmQ=          ... Install the specified certificate using the specified Base64-encoded password.\r\n" +
                        "    " + filename + " -cert C:\\Temp\\cert.pfx -pass Pa$$w0rd -plaintext   ... Install the specified certificate using the specified plaintext password."
                    , "eduSTAR-Connect: Help");
                    return;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
