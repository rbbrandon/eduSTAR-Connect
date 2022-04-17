# eduSTAR Connect Utility

## USAGE:
    eduSTAR-Connect.exe [-? | -h | -help] [-r | -reboot] [-s | -silent] [-xml <path>] [-cert <path>] [-pass <password> [-plaintext]]

## OPTIONS:
    -? | -h | -help    Display this help message.
    -r | -reboot       Reboot the computer after eduSTAR is configured.
    -s | -silent       Don't display any popup messages.
    -xml <path>        Install the wireless profile xml specified by <path>. Default: <current dir>\eduSTAR.xml if exists, otherwise uses internal xml.
    -cert <path>       Install the certificate specified by <path>. Default: <current dir>\cert.pfx
    -pass <password>   Use the Base64-encoded (or plaintext) password specified by <password>. Default: default eduSTAR password
        -plaintext     Specifies that the password specified by "-pass" is plaintext. Default: disabled

## EXAMPLES:
    eduSTAR-Connect.exe                                                    ... Connect with all default values.
    eduSTAR-Connect.exe -r -s                                              ... Reboot when done, and don't display any popups.
    eduSTAR-Connect.exe -xml C:\Temp\eduSTAR.xml                           ... Install the WiFi profile at "C:\Temp\eduSTAR.xml".
    eduSTAR-Connect.exe -cert C:\Temp\cert.pfx                             ... Install the specified certificate using the default password.
    eduSTAR-Connect.exe -pass UGEkJHcwcmQ=                                 ... Install the default certificate using the specified Base64-encoded password.
    eduSTAR-Connect.exe -cert C:\Temp\cert.pfx -pass UGEkJHcwcmQ=          ... Install the specified certificate using the specified Base64-encoded password.
    eduSTAR-Connect.exe -cert C:\Temp\cert.pfx -pass Pa$$w0rd -plaintext   ... Install the specified certificate using the specified plaintext password.
