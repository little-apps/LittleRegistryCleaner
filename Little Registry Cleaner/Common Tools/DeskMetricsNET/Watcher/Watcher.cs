// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Watcher.cs                                      //
//     Copyright (c) 2010-2011 DeskMetrics Limited                       //
//                                                                       //
//     http://deskmetrics.com                                            //
//     http://support.deskmetrics.com                                    //
//                                                                       //
//     support@deskmetrics.com                                           //
//                                                                       //
//     This code is provided under the DeskMetrics Modified BSD License  //
//     A copy of this license has been distributed in a file called      //
//     LICENSE with this source code.                                    //
//                                                                       //
// **********************************************************************//

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Security;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using Common_Tools.DeskMetrics.Json;

namespace Common_Tools.DeskMetrics
{
    public class Watcher : IDisposable
    {
		#region attributes
        Thread StopThread;
        /// <summary>
        /// Thread Lock
        /// </summary>
        private System.Object ObjectLock = new System.Object();

        /// <summary>
        /// Field User GUID
        /// </summary>
        private object _userGUID;

        public object UserGUID
        {
            get {
                if (_userGUID == null)
                    _userGUID = User.GetID();
                return _userGUID; 
            }
        }
        /// <summary>
        /// Field Session Id
        /// </summary>
        private object _sessionGUID;

        public object SessionGUID
        {
            get {
                if (_sessionGUID == null)
                {
                    _sessionGUID = User.GetSessionID();
                }
                return _sessionGUID;
            }
            //private set { _sessionGUID = value; }
        }
        /// <summary>
        /// Field Json
        /// </summary>
        private List<string> _json;
        /// <summary>
        /// Field Application Id
        /// </summary>
        private string _applicationId;
        /// <summary>
        /// Field Error Message
        /// </summary>
        private string _error;

        /// <summary>
        /// Field ApplicationVersion
        /// </summary>
        private string _applicationVersion;
        /// <summary>
        /// Field Component Name
        /// </summary>
        private string _componentName;
        /// <summary>
        /// Field Component Version
        /// </summary>
        private string _componentVersion;

        private int _flowglobalnumber = 0;

        private bool _started = false;

        public bool Started
        {
            get { return _started; }
        }

        private bool _enabled = true;

        public string ApplicationId
        {
            get
            {
                return _applicationId;
            }
            set
            {
                _applicationId = value;
				Cache.ApplicationId = ApplicationId;
            }
        }

        public string ApplicationVersion
        {
            get
            {
                return _applicationVersion;
            }
            set
            {
                _applicationVersion = value;
            }
        }

        public List<string> JSON
        {
            get
            {
                if (_json == null)
                    _json = new List<string>();
                return _json;
            }
            set
            {
                _json = value;
            }
        }

        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public string ComponentName
        {
            get
            {
                Assembly thisAsm = this.GetType().Assembly;
                object[] attrs = thisAsm.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                _componentName = ((AssemblyTitleAttribute)attrs[0]).Title;
                return _componentName;
            }

        }

        public string ComponentVersion
        {
            get
            {
                _componentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return _componentVersion;
            }

        }

        

        private Services _services;

        public Services Services
        {
            get {
                if (_services == null)
                    _services = new Services(this);
                return _services; 
            }
            private set { _services = value; }
        }

        
		
		private Cache _cache;
		internal Cache Cache
		{
			get
			{
				if (_cache == null)
					_cache = new Cache();
				return _cache;
			}
		}
		
		private CurrentUser _user;
		
		public CurrentUser User {
			get {
				if (_user==null)
					_user = new CurrentUser();
				return _user;
			}
		}

		
		internal void CheckApplicationCorrectness()
		{
			if (string.IsNullOrEmpty(ApplicationId.Trim()))
				throw new Exception("You must specify an non-empty application ID");
		}
		
		#endregion
		/// <summary>
		/// Starts the application tracking.
		/// </summary>
		/// <param name="ApplicationId">
		/// Your app ID. You can get it at http://analytics.deskmetrics.com/
		/// </param>
		/// <param name="ApplicationVersion">
		/// Your app version.
		/// </param>
        public void Start(string ApplicationId, string ApplicationVersion)
        {
			this.ApplicationId = ApplicationId;
            this.ApplicationVersion = ApplicationVersion;
			CheckApplicationCorrectness();
			
			lock (ObjectLock)
                if (Enabled)
            		StartAppJson();
			_started = true;
        }
		
		private void StartAppJson()
		{
			var startjson = new StartAppJson(this);
			JSON.Add(JsonBuilder.GetJsonFromHashTable(startjson.GetJsonHashTable()));
		}

		private void TryInitializeStop()
		{
			if (StopThread == null)
				StopThread = new Thread(_StopThreadFunc);
		}
		
		private bool IsStopThreadInitialized()
		{
			return StopThread != null && !StopThread.IsAlive;
		}

		private void RunStopThread()
		{
			StopThread = new Thread(_StopThreadFunc);
            StopThread.Name = "StopSender";
            StopThread.Start();
		}
		
        /// <summary>
        /// Stops the application tracking and send the collected data to DeskMetrics
        /// </summary>
        public void Stop()
        {
			CheckApplicationCorrectness();
            lock (ObjectLock)
            {
            	TryInitializeStop();    
                if (IsStopThreadInitialized())
					RunStopThread();
            }
        }
		
		private string GenerateStopJson()
		{
			var json = new StopAppJson();
            JSON.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
            string SingleJSON = JsonBuilder.GetJsonFromList(JSON);
			return SingleJSON;
		}
		
		private string AppendCacheDataToJson(string json)
		{
			string CacheData = Cache.GetCacheData();
            if (!string.IsNullOrEmpty(CacheData))
                json = json + "," + CacheData;
			return json;
		}

        private void _StopThreadFunc()
        {
            lock (ObjectLock)
            {
				CheckApplicationCorrectness();
                JSON.Add(AppendCacheDataToJson(GenerateStopJson()));
				try
				{
                	Services.PostData(Settings.ApiEndpoint,JsonBuilder.GetJsonFromList(JSON));
					JSON.Clear();
					Cache.Delete();
				}
                catch (Exception e)
				{
					Cache.Save(JSON);
					throw e;
				}
            }
        }

        /// <summary>
        /// Register an event occurence
        /// </summary>
        /// <param name="EventCategory">EventCategory Category</param>
        /// <param name="eventName">EventCategory Name</param>
        public void TrackEvent(string EventCategory, string EventName)
        {
            lock (ObjectLock)
                if (Started)
                {
					CheckApplicationCorrectness();
                    var json = new EventJson(EventCategory, EventName, GetFlowNumber());
                    JSON.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
        }

		/// <summary>
		/// Tracks an event related to time and intervals
		/// </summary>
		/// <param name="EventCategory">
		/// The event category
		/// </param>
		/// <param name="EventName">
		/// The event name
		/// </param>
		/// <param name="EventTime">
		/// The event duration 
		/// </param>
		/// <param name="Completed">
		/// True if the event was completed.
		/// </param>
        public void TrackEventPeriod(string EventCategory, string EventName, int EventTime,bool Completed)
        {
            lock (ObjectLock)
            {
                if (Started)
                {
					CheckApplicationCorrectness();
                    var json = new EventPeriodJson(EventCategory, EventName, GetFlowNumber(), EventTime,Completed);
                    JSON.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
            }
        }
		
		/// <summary>
		/// Tracks an installation
		/// </summary>
		/// <param name="version">
		/// Your app version
		/// </param>
		/// <param name="appid">
		/// Your app ID. You can get it at http://analytics.deskmetrics.com/
		/// </param>
		public void TrackInstall(string version,string appid)
		{
			lock (ObjectLock)
            {
                var json = new InstallJson(version);
				ApplicationId = appid;
                _started = true;
				Services.SendData(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
            }
		}
		/// <summary>
		/// Tracks an uninstall
		/// </summary>
		/// <param name="version">
		/// Your app version
		/// </param>
		/// <param name="appid">
		/// Your app ID. You can get it at http://analytics.deskmetrics.com/
		/// </param>
		public void TrackUninstall(string version,string appid)
		{
			lock (ObjectLock)
            {
                var json = new UninstallJson(version);
				ApplicationId = appid;
                _started = true;
				Services.SendData(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
            }
		}
        
        /// <summary>
        /// Tracks an exception
        /// </summary>
        /// <param name="ApplicationException">
        /// The exception object to be tracked
        /// </param>
        public void TrackException(Exception ApplicationException)
        {
            lock (ObjectLock)
                if (Started && ApplicationException != null)
                {
					CheckApplicationCorrectness();
                    var json = new ExceptionJson(ApplicationException,GetFlowNumber());
                    JSON.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
        }        

        /// <summary>
        /// </summary>
        void IDisposable.Dispose()
        {
            try
            {
                this.Stop();
            }
            catch (Exception e)
            {
                _error = e.Message;
            }
        }

        protected int GetFlowNumber()
        {
            lock (ObjectLock)
            {
                try
                {
                    _flowglobalnumber = _flowglobalnumber + 1;
                    return _flowglobalnumber;
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Tracks an event with custom value
        /// </summary>
        /// <param name="EventCategory">
        /// The event category
        /// </param>
        /// <param name="EventName">
        /// The event name
        /// </param>
        /// <param name="EventValue">
        /// The custom value
        /// </param>
        public void TrackEventValue(string EventCategory, string EventName, string EventValue)
        {
            lock (ObjectLock)
                if (Started)
                {
					CheckApplicationCorrectness();
                    var json = new EventValueJson(EventCategory, EventName, EventValue, GetFlowNumber());
                    JSON.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
        }

        /// <summary>
        /// Tracks custom data
        /// </summary>
        /// <param name="CustomDataName">
        /// The custom data name
        /// </param>
        /// <param name="CustomDataValue">
        /// The custom data value
        /// </param>
        public void TrackCustomData(string CustomDataName, string CustomDataValue)
        {
            lock (ObjectLock)
                if (Started)
                {
					CheckApplicationCorrectness();
                    var json = new CustomDataJson(CustomDataName, CustomDataValue, GetFlowNumber());
                    JSON.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
        }

        /// <summary>
        /// Tracks a log
        /// </summary>
        /// <param name="Message">
        /// The log message
        /// </param>
        public void TrackLog(string Message)
        {
            lock (ObjectLock)
            {
                if (Started)
                {
                    CheckApplicationCorrectness();
                    var json = new LogJson(Message,GetFlowNumber());
                    JSON.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
            }
        }

		
		/// <summary>
		/// Try to track real time customized data and caches it to send later if any network error occurs.
		/// </summary>
		/// <param name="CustomDataName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="CustomDataValue">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// True if it was sended in real time, false otherwise
		/// </returns>
		public bool TrackCachedCustomDataR(string CustomDataName, string CustomDataValue)
		{
			try
			{
				TrackCustomDataR(CustomDataName,CustomDataValue);
			}
			catch (Exception)
            {
				var json = new CustomDataRJson(CustomDataName, CustomDataValue, GetFlowNumber(), ApplicationId, ApplicationVersion);
            	JSON.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
				return false;
            }
			return true;
		}

		/// <summary>
		/// Tracks a custom data without cache support
		/// </summary>
		/// <param name="CustomDataName">
		/// Self-explanatory ;)
		/// </param>
		/// <param name="CustomDataValue">
		/// Self-explanatory ;)
		/// </param>
		/// <returns>
		/// 
		/// </returns>
        public void TrackCustomDataR(string CustomDataName, string CustomDataValue)
        {
            lock (ObjectLock)
            {
                if (Started)
                {
					CheckApplicationCorrectness();
                    var json = new CustomDataRJson(CustomDataName, CustomDataValue, GetFlowNumber(), ApplicationId, ApplicationVersion);
                    Services.PostData(Settings.ApiEndpoint, JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
            }
        }

        public void SendDataAsync()
        {
            Services.SendDataAsync(JsonBuilder.GetJsonFromList(JSON));
            JSON.Clear();
        }
    } 
}
