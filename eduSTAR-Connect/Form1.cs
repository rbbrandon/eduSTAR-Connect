using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace eduSTAR_Connect
{
    public partial class Form1 : Form
    {
        private bool _RebootRequired = false;
        private bool _Silent = false;
        private bool _PlaintextPassword = false;
        private string _Cert = Constants.DEFAULT_CERT;
        private string _Password = Constants.DEFAULT_PASSWORD;
        private string _XML = Constants.DEFAULT_XML;

        public Form1()
        {
            InitializeComponent();

            Helper.DisableCloseButton(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ProcessParams();

            if (!File.Exists(_Cert))
            {
                if (!_Silent)
                    MessageBox.Show("Unable to find \"" + _Cert + "\"", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            if (!File.Exists(_XML))
            {
                _XML = Path.GetTempPath() + "eduSTAR.xml";
                File.WriteAllText(_XML, Resource1.Wi_Fi_eduSTAR);
            }

            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            progressBar.Text = "Deleting Department Certificates";
            DeleteCerts();

            progressBar.Text = "Deleting Existing eduSTAR WiFi Profile";
            progressBar.Value = 20;
            DeleteProfile();

            progressBar.Text = "Installing Root Certificate";
            progressBar.Value = 25;
            InstallRootCert();

            progressBar.Text = "Installing CA Certificates";
            progressBar.Value = 35;
            InstallCACerts();

            progressBar.Text = "Installing Personal Certificate";
            progressBar.Value = 50;
            InstallPersonalCert();

            progressBar.Text = "Installing eduSTAR WiFi Profile";
            progressBar.Value = 75;
            InstallProfile();

            progressBar.Text = "Resetting Network";
            progressBar.Value = 80;
            ResetNetwork();

            progressBar.Text = "Connecting to eduSTAR WiFi";
            progressBar.Value = 90;
            ConnectToEduSTAR();

            progressBar.Text = "Complete";
            progressBar.Value = 100;
            if (_Silent)
            {
                if (_RebootRequired)
                    Reboot();
                else
                    System.Threading.Thread.Sleep(2000);
            }
            else
            {
                this.Hide();

                if (_RebootRequired)
                {
                    if (MessageBox.Show("Connection complete.\r\nThe computer will reboot when you click \"OK\", so be sure to save any unsaved work beforehand.",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                        Reboot();
                }
                else
                    MessageBox.Show("Connection complete.\r\nIf the wireless is still not connecting, or reads as \"Unidentied Network\", please reboot the computer.",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.Close();
        }


        private void ProcessParams()
        {
            string[] args = Environment.GetCommandLineArgs();
            bool passwordSpecified = false;

            for (int i = 1; i < args.Count(); i++)
            {
                string arg = args[i].ToLower().Replace('/', '-');

                if (arg == "-reboot" || arg == "-r")
                {
                    _RebootRequired = true;
                    continue;
                }

                if (arg == "-silent" || arg == "-s")
                {
                    _Silent = true;
                    continue;
                }

                if (arg == "-plaintext")
                {
                    _PlaintextPassword = true;
                    continue;
                }

                if (i + 1 < args.Count())
                {
                    if (arg == "-cert")
                    {
                        _Cert = args[i + 1];
                        i++;
                        continue;
                    }

                    if (arg == "-pass")
                    {
                        passwordSpecified = true;
                        _Password = args[i + 1];
                        i++;
                        continue;
                    }

                    if (arg == "-xml")
                    {
                        _XML = args[i + 1];
                        i++;
                        continue;
                    }
                }
            }

            if (_PlaintextPassword && passwordSpecified)
                _Password = Helper.Base64Encode(_Password);
        }

        private void DeleteCerts()
        {
            X509Store store;

            // Delete personal certificates belonging to DEET
            foreach (StoreLocation location in Enum.GetValues(typeof(StoreLocation)))
            {
                store = new X509Store(StoreName.My, location);
                
                store.Open(OpenFlags.ReadWrite);

                foreach (X509Certificate2 mCert in store.Certificates)
                {
                    foreach (string issuer in Constants.CERT_ISSUERS)
                    {
                        if (mCert.Issuer.Contains(issuer))
                            store.Remove(mCert);
                    }
                }

                store.Close();
            }

            // Remove DEET Root certificates
            store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            foreach (X509Certificate2 mCert in store.Certificates)
            {
                foreach (string thumbprint in Constants.CERT_THUMBPRINTS_ROOT)
                {
                    if (mCert.Thumbprint.Replace(" ", "").ToLower() == thumbprint)
                    {
                        store.Remove(mCert);
                        //byte[] certBytes = mCert.Export(X509ContentType.Pkcs12);
                        //Clipboard.SetText(Clipboard.GetText() + "public static byte[] " + mCert.GetNameInfo(X509NameType.SimpleName, false) + " = new byte[] {\r\n            " + ByteArrayToString(certBytes) + "\r\n        };\r\n");
                    }
                }
            }
            store.Close();

            // Remove DEET Intermediate CA certificates
            store = new X509Store(StoreName.CertificateAuthority, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            foreach (X509Certificate2 mCert in store.Certificates)
            {
                foreach (string thumbprint in Constants.CERT_THUMBPRINTS_CA)
                {
                    if (mCert.Thumbprint.Replace(" ", "").ToLower() == thumbprint)
                    {
                        store.Remove(mCert);
                        //byte[] certBytes = mCert.Export(X509ContentType.Pkcs12);
                        //Clipboard.SetText(Clipboard.GetText() + "public static byte[] " + mCert.GetNameInfo(X509NameType.SimpleName, false) + " = new byte[] {\r\n            " + ByteArrayToString(certBytes) + "\r\n        };\r\n");
                    }
                }
            }
            store.Close();
        }

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            int count = 1;
            foreach (byte b in ba)
            {
                if (count == 16)
                {
                    hex.AppendFormat("0x{0:x2},\r\n            ", b);
                    count = 0;
                }
                else
                    hex.AppendFormat("0x{0:x2}, ", b);

                count++;
            }

            string hexString = hex.ToString();
            if (hexString.Substring(hexString.Length - 2, 2) == ", ")
                return hexString.Substring(0, hexString.Length - 2);
            else
                return hexString.Substring(0, hexString.Length - 15);

        }

        private void DeleteProfile()
        {
            RunCommand("netsh wlan delete profile name=eduSTAR");
        }

        private void InstallProfile()
        {
            RunCommand("netsh wlan add profile filename=\"" + _XML + "\" user=all");
        }

        private void InstallRootCert()
        {
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);

            X509Certificate2 cert = new X509Certificate2(educationCerts.educationRootCA);
            store.Add(cert);

            cert = new X509Certificate2(educationCerts.DEET);
            store.Add(cert);

            store.Close();
        }

        private void InstallCACerts()
        {
            X509Store store = new X509Store(StoreName.CertificateAuthority, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);

            X509Certificate2 cert = new X509Certificate2(educationCerts.servicesCERTSCA);
            store.Add(cert);

            cert = new X509Certificate2(educationCerts.educationSubCA01);
            store.Add(cert);

            cert = new X509Certificate2(educationCerts.educationSubCA02);
            store.Add(cert);

            cert = new X509Certificate2(educationCerts.schoolsSubCA01);
            store.Add(cert);

            cert = new X509Certificate2(educationCerts.schoolsSubCA02);
            store.Add(cert);

            store.Close();
        }

        private void InstallPersonalCert()
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            
            X509Certificate2 cert = new X509Certificate2(_Cert, Helper.Base64Decode(_Password), X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
            store.Add(cert);

            store.Close();
        }

        private void ResetNetwork()
        {
            RunCommand("netsh int ip reset");
            RunCommand("netsh winsock reset");
        }

        private void ConnectToEduSTAR()
        {
            RunCommand("netsh wlan connect name=eduSTAR ssid=eduSTAR");
        }

        private void Reboot()
        {
            RunCommand("shutdown -r -f -t 0");
        }

        private void RunCommand(string command)
        {
            Process myProcess = new Process();
            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.StartInfo.Arguments = "/C " + command;
            myProcess.Start();
            myProcess.WaitForExit();
        }
    }
}
