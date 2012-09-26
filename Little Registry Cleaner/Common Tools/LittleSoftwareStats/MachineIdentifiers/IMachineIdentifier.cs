using System;

namespace LittleSoftwareStats.MachineIdentifiers
{
    public interface IMachineIdentifier
    {
        byte[] IdentifierHash { get;}
        bool Match(byte[] identifierHash);
    }
}
