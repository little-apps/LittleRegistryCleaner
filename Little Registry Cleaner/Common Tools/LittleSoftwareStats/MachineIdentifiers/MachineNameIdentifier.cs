using System;

namespace LittleSoftwareStats.MachineIdentifiers
{
    public class MachineNameIdentifier : MachineIdentifierBase, IMachineIdentifier
    {
        protected override byte[] GetIdentifierHash()
        {
            return base.ComputeHash(Environment.MachineName);
        }
    }
}
