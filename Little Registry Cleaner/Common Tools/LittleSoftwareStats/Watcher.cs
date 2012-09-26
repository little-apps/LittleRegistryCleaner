using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Threading;
using LittleSoftwareStats.MachineIdentifiers;

namespace LittleSoftwareStats
{
    public class Watcher
    {
        private Events _array = new Events();
        private Cache _cache = new Cache();

        private IMachineIdentifierProvider _identifierService;
        private string _uniqueId;
        private string UniqueId
        {
            get
            {
                if (string.IsNullOrEmpty(this._uniqueId))
                {
                    this._identifierService = new MachineIdentifierProvider(new IMachineIdentifier[] { new MachineNameIdentifier(), new NetworkAdapterIdentifier(), new VolumeInfoIdentifier() });

                    this._uniqueId = this._identifierService.MachineHash;
                }

                return this._uniqueId;
            }
        }

        private string _sessionId;
        private string SessionId
        {
            get
            {
                if (!string.IsNullOrEmpty(this._sessionId))
                {
                    return this._sessionId;
                } 
                else
                {
                    this._sessionId = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                    return this._sessionId;
                }
            }
        }

        protected int _flowNumber = 0;
        protected int FlowNumber
        {
            get
            {
                this._flowNumber = this._flowNumber + 1;
                return this._flowNumber;
            }
        }

        private bool _started = false;
        public bool Started
        {
            get { return this._started; }
        }

        public Watcher()
        {
        }

        public void Start(string appId, string appVer, bool enabled = true) {
            if (this.Started || !Config.Enabled)
                return;

            Event e = new Event("strApp", this.SessionId);

            Config.AppId = appId;
            Config.AppVer = appVer;

            // Get os + hardware config
            OperatingSystem.OperatingSystem osInfo = OperatingSystem.OperatingSystem.GetOperatingSystemInfo();
            Hardware.Hardware hwInfo = osInfo.Hardware;

            e.Add("ID", this.UniqueId);
            e.Add("aid", appId);
            e.Add("aver", appVer);

            e.Add("osv", osInfo.Version);
            e.Add("ossp", osInfo.ServicePack);
            e.Add("osar", osInfo.Architecture);
            e.Add("osjv", osInfo.JavaVersion);
            e.Add("osnet", osInfo.FrameworkVersion);
            e.Add("osnsp", osInfo.FrameworkSP);
            e.Add("oslng", osInfo.Lcid);
            e.Add("osscn", hwInfo.ScreenResolution);

            e.Add("cnm", hwInfo.CPUName);
            e.Add("car", hwInfo.CPUArchitecture);
            e.Add("cbr", hwInfo.CPUBrand);
            e.Add("cfr", hwInfo.CPUFrequency);
            e.Add("ccr", hwInfo.CPUCores);
            e.Add("mtt", hwInfo.MemoryTotal);
            e.Add("mfr", hwInfo.MemoryFree);
            e.Add("dtt", hwInfo.DiskTotal);
            e.Add("dfr", hwInfo.DiskFree);

            this._array.Add(e);

            this._started = true;
        }

        public void Stop()
        {
            if (!this.Started)
                return;

            this._array.Add(new Event("stApp", this.SessionId));

            try
            {
                string data = this._cache.GetPostData(this._array);

                Utils.SendPostData(data);
                this._cache.Delete();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                this._cache.SaveCacheToFile(this._array);
            }

            this._started = false;
        }

        public void Event(string categoryName, string eventName)
        {
            if (!this.Started)
                return;

            Event e = new Event("ev", this.SessionId, this.FlowNumber);

            e.Add("ca", categoryName);
            e.Add("nm", eventName);

            this._array.Add(e);
        }

        public void EventValue(string categoryName, string eventName, string eventValue)
        {
            if (!this.Started)
                return;

            Event e = new Event("evV", this.SessionId, this.FlowNumber);

            e.Add("ca", categoryName);
            e.Add("nm", eventName);
            e.Add("vl", eventValue);

            this._array.Add(e);
        }

        public void EventPeriod(string categoryName, string eventName, int eventDuration, bool eventCompleted)
        {
            if (!this.Started)
                return;

            Event e = new Event("evP", this.SessionId, this.FlowNumber);

            e.Add("ca", categoryName);
            e.Add("nm", eventName);
            e.Add("tm", eventDuration);
            e.Add("ec", (eventCompleted) ? (1) : (0));

            this._array.Add(e);
        }

        public void Log(string logMessage)
        {
            if (!this.Started)
                return;

            Event e = new Event("lg", this.SessionId, this.FlowNumber);

            e.Add("ms", logMessage);

            this._array.Add(e);
        }

        public enum Licenses { Free, Trial, Registered, Demo, Cracked };

        public void License(Licenses l)
        {
            if (!this.Started)
                return;

            Event e = new Event("ctD", this.SessionId, this.FlowNumber);

            e.Add("nm", "License");

            string licenseType = "";
            switch (l)
            {
                case Licenses.Free:
                    {
                        licenseType = "F";
                        break;
                    }
                case Licenses.Trial:
                    {
                        licenseType = "T";
                        break;
                    }
                case Licenses.Demo:
                    {
                        licenseType = "D";
                        break;
                    }
                case Licenses.Registered:
                    {
                        licenseType = "R";
                        break;
                    }
                case Licenses.Cracked:
                    {
                        licenseType = "C";
                        break;
                    }
            }

            e.Add("vl", licenseType);

            this._array.Add(e);
        }

        public void CustomData(string dataName, string dataValue)
        {
            if (!this.Started)
                return;

            Event e = new Event("ctD", this.SessionId, this.FlowNumber);

            e.Add("nm", dataName);
            e.Add("vl", dataValue);

            this._array.Add(e);
        }

        public void Exception(Exception ex)
        {
            if (!this.Started)
                return;

            Event e = new Event("exC", this.SessionId, this.FlowNumber);

            e.Add("msg", ex.Message);
            e.Add("stk", ex.StackTrace);
            e.Add("src", ex.Source);
            e.Add("tgs", ex.TargetSite);

            this._array.Add(e);
        }

        public void Exception(string exceptionMsg, string stackTrace, string exceptionSrc, string targetSite)
        {
            if (!this.Started)
                return;

            Event e = new Event("exC", this.SessionId, this.FlowNumber);

            e.Add("msg", exceptionMsg);
            e.Add("stk", stackTrace);
            e.Add("src", exceptionSrc);
            e.Add("tgs", targetSite);

            this._array.Add(e);
        }

        public void Install()
        {
            if (!this.Started)
                return;

            Event e = new Event("ist", this.SessionId, this.FlowNumber);

            e.Add("ID", this.UniqueId);
            e.Add("aid", Config.AppId);
            e.Add("aver", Config.AppVer);

            this._array.Add(e);
        }

        public void Uninstall()
        {
            if (!this.Started)
                return;

            Event e = new Event("ust", this.SessionId, this.FlowNumber);

            e.Add("aid", Config.AppId);
            e.Add("aver", Config.AppVer);

            this._array.Add(e);
        }
    }
}
