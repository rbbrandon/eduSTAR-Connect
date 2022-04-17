using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eduSTAR_Connect
{
    static class Constants
    {
        private static string _DEFAULT_PASSWORD = "cGFzc3dvcmQ=";
        private static string _CURRENT_DIR = AppDomain.CurrentDomain.BaseDirectory;
        private static string _DEFAULT_CERT = _CURRENT_DIR + "cert.pfx";
        private static string _DEFAULT_XML = _CURRENT_DIR + "eduSTAR.xml";
        private static List<string> _CERT_ISSUERS = new List<string>()
        {
            "CN=schoolsSubCA02",
            "CN=services-CERTS-CA"
        };
        private static List<string> _CERT_THUMBPRINTS_ROOT = new List<string>()
        {
            "1ceee53625955281620d146ea8efaabfd0c420c4", // oldOldRootCertThumbprint
            "4b07ed87295ff860580090531720c9f2f45e693f", // oldRootCertThumbprint
            "7041c4af52ec452a799ad27ba171c3096704aebd" //eduRootCA01Thumbrprint
        };
        private static List<string> _CERT_THUMBPRINTS_CA = new List<string>()
        {
            "0df14f1f8b5aaedba5908a3c7d182319e44392a3", // oldServicesCertThumbprint
            "3340c593fb9a3b47797ab5ce7e3402672ab56ab4", // eduSubCA01Thumbprint
            "7639333e98c71ac5e726f65b673bed679d4590cd", // eduSubCA02Thumbprint
            "c21e7efa5af92ef7709a94710c01fe61c6ce9326", // staSubCA01Thumbprint
            "ad98807f2720c790710ee87c1bcc34da24ea5c7a" //  staSubCA02Thumbprint
        };



        public static string DEFAULT_PASSWORD { get {return _DEFAULT_PASSWORD; } }
        public static string DEFAULT_CERT { get { return _DEFAULT_CERT; } }
        public static string DEFAULT_XML { get { return _DEFAULT_XML; } }
        public static IList<string> CERT_ISSUERS { get { return _CERT_ISSUERS.AsReadOnly(); } }
        public static IList<string> CERT_THUMBPRINTS_ROOT { get { return _CERT_THUMBPRINTS_ROOT.AsReadOnly(); } }
        public static IList<string> CERT_THUMBPRINTS_CA { get { return _CERT_THUMBPRINTS_CA.AsReadOnly(); } }

    }
}
