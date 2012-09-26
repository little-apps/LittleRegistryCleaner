using System;
using System.Management;
using System.Net.NetworkInformation;

namespace LittleSoftwareStats.MachineIdentifiers
{
    public class NetworkAdapterIdentifier : MachineIdentifierBase, IMachineIdentifier
    {
        protected override byte[] GetIdentifierHash()
        {
            string identifier = "NOTFOUND";
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

                if (nics != null && nics.Length > 0) {
                    foreach (NetworkInterface nic in nics)
                    {
                        if (nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        {
                            identifier = nic.GetPhysicalAddress().ToString();
                            break;
                        }
                    }
                }

            }
            catch { }
            return base.ComputeHash(identifier);
        }
    }
}
