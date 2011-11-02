using System;
using Microsoft.Win32;

namespace Common_Tools.DeskMetrics
{
	public class CurrentUser
	{
		public CurrentUser ()
		{
		}
		private System.Object ObjectLock = new System.Object();
		
		private RegistryKey GetOrCreateDeskMetricsSubKey()
		{
			RegistryKey reg = Registry.CurrentUser.OpenSubKey("Sofware\\dskMetrics");
            if (reg == null)
                reg = Registry.CurrentUser.CreateSubKey("Software\\dskMetrics");
			return reg;
		}
		
        public void SetUserID(string UserID)
        {
            lock (ObjectLock)
            {
                using (RegistryKey reg = GetOrCreateDeskMetricsSubKey())
                {
                    if (reg == null)
                        return;

                    reg.SetValue("ID", UserID);
                    reg.Close();
                }
            }
        }

        public string GetSessionID()
        {
            lock (ObjectLock)
            {
                try
                {
                    return System.Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                }
                catch
                {
                    // Manually generate ID
                    string guid = "";
                    string salt = "ABCDEF0123456789";

                    Random rand = new Random();

                    for (int i = 0; i < 32; i++)
                        guid += salt[rand.Next(salt.Length)];

                    return guid;
                }
            }
        }

		public string CreateUserID(RegistryKey reg)
		{
			string UserID = reg.GetValue("ID").ToString();
            if (!string.IsNullOrEmpty(UserID))
                return UserID;

			UserID = GetSessionID();
            SetUserID(UserID);
            return UserID;
		}
		
        public string GetID()
        {
            lock (ObjectLock)
            {
                RegistryKey reg = Registry.CurrentUser.OpenSubKey("Software\\dskMetrics", true);
                if (reg == null)
                {
                    string _UserID = GetSessionID();
                    SetUserID(_UserID);
                    return _UserID;
                }
                return CreateUserID(reg);
            }
        }
	}
}

