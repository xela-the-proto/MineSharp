using System.Net;

namespace Common.Utils.Net;

public class IpFinder
{
    public static string findLocalMachineIp()
    {
        // Get the Name of HOST  
        string hostName = Dns.GetHostName();
        Console.WriteLine(hostName);

        string IP = Dns.GetHostByName(hostName).AddressList[0].ToString();
        Console.WriteLine("IP Address of local machine is : " + IP);
        return IP;
    }
}