using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace GameCommon
{
    public static class NetworkInfo
    {
        public static IPAddress GetCurrentIP()
        {
            IPAddress[] adresses = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress currentIPAdress = null;
            bool IsFound = false;
            foreach (var adress in adresses)
            {
                if (adress.GetAddressBytes().Length == 4 && !IsFound)
                {
                    currentIPAdress = adress;
                    IsFound = true;
                }
            }
            return currentIPAdress;
        }

        public static IPAddress GetBroadcastAddress(IPAddress address, IPAddress mask)
        {
            byte[] bytesAddress = address.GetAddressBytes();
            byte[] bytesMask = mask.GetAddressBytes();

            for(int  i = 0; i < 4; i++)
            {
                if(bytesMask[i] == 0)
                {
                    bytesAddress[i] = 255;
                }
            }
            IPAddress result = new IPAddress(bytesAddress);
            return result;
        }

        public static IPAddress GetIPv4Mask(IPAddress address)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach(var adapter in adapters)
            {
                foreach(UnicastIPAddressInformation unicastAdressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastAdressInformation.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastAdressInformation.Address))
                            return unicastAdressInformation.IPv4Mask;
                    }
                }
            }

            return null;
        }
    }
}
