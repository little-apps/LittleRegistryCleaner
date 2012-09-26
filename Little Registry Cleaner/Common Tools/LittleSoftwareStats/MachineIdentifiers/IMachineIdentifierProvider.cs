using System;

namespace LittleSoftwareStats.MachineIdentifiers
{
    public interface IMachineIdentifierProvider
    {
        string MachineHash { get; }
        bool Match(byte[] machineHash);
    }
}
