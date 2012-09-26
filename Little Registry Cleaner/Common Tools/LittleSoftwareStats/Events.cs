using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace LittleSoftwareStats
{
    public class Events : CollectionBase
    {
        public Event this[int index]
        {
            get { return (Event)this.InnerList[index]; }
            set { this.InnerList[index] = value; }
        }

        public Events()
        {
        }

        public int Add(Event eventData)
        {
            return this.InnerList.Add(eventData);
        }

        public bool Contains(Event eventData)
        {
            return this.InnerList.Contains(eventData);
        }

        public int IndexOf(Event eventData)
        {
            return this.InnerList.IndexOf(eventData);
        }

    }

    public class Event : DictionaryBase
    {
        public DictionaryEntry this[int index]
        {
            get { return (DictionaryEntry)this.InnerHashtable[index]; }
            set { this.InnerHashtable[index] = value; }
        }

        public Event(string eventCode, string sessionId, int flowId = 0) 
        {
            this.Add("tp", eventCode);
            this.Add("ss", sessionId);
            this.Add("ts", Utils.GetUnixTime());

            if (flowId != 0)
                this.Add("fl", flowId);
        }

        public void Add(string name, object value) 
        {
            if (string.IsNullOrEmpty(name))
                return;

            this.InnerHashtable.Add(name, value);
        }
    }
}
