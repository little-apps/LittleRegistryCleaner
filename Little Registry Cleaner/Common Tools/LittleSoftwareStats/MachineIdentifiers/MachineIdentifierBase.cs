using System;
using System.Text;
using System.Security.Cryptography;

namespace LittleSoftwareStats.MachineIdentifiers
{
    abstract public class MachineIdentifierBase : IMachineIdentifier
    {
        private byte[] _IdentifierHash = null;

        virtual public byte[] IdentifierHash 
        {
            get
            {
                if (_IdentifierHash == null)
                {
                    _IdentifierHash = GetIdentifierHash();
                }
                return _IdentifierHash;
            }
        }

        abstract protected byte[] GetIdentifierHash();

        virtual public bool Match(byte[] hash)
        {
            if (ReferenceEquals(IdentifierHash, hash))
                return true;

            if (IdentifierHash == null || hash == null)
                return false;

            if (IdentifierHash.Length != hash.Length)
                return false;

            for (int n = 0; n < IdentifierHash.Length; n++)
            {
                if (IdentifierHash[n] != hash[n])
                    return false;
            }
            return true;
        }

        protected byte[] ComputeHash(string value)
        {
            MD5 hasher = new MD5CryptoServiceProvider();
            byte[] hash = hasher.ComputeHash(Encoding.ASCII.GetBytes(value));
            return hash;
        }
    }
}
