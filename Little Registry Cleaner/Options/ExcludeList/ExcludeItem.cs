using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Little_Registry_Cleaner.ExcludeList
{
    [Serializable()]
    public class ExcludeItem : ICloneable
    {
        private string _pathRegistry = "";
        public string RegistryPath
        {
            get { return _pathRegistry; }
            set { 
                if (Utils.RegKeyExists(value))
                    _pathRegistry = value;
            }
        }

        private string _pathFolder = "";
        public string FolderPath
        {
            get { return _pathFolder; }
            set { 
                if (Directory.Exists(value))
                    _pathFolder = value;
            }
        }

        private string _pathFile = "";
        public string FilePath
        {
            get { return _pathFile; }
            set { 
                if (File.Exists(value))
                    _pathFile = value;
            }
        }

        /// <summary>
        /// Returns the assigned path (registry/file/folder)
        /// </summary>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_pathRegistry))
                return string.Copy(_pathRegistry);

            if (!string.IsNullOrEmpty(_pathFile))
                return string.Copy(_pathFile);

            if (!string.IsNullOrEmpty(_pathFolder))
                return string.Copy(_pathFolder);

            return this.GetType().Name;
        }

        /// <summary>
        /// The constructor for this class
        /// Only one parameter can not be NULL!
        /// </summary>
        public ExcludeItem(string regPath, string folderPath, string filePath)
        {
            if (!string.IsNullOrEmpty(regPath))
            {
                RegistryPath = regPath;
                return;
            }

            if (!string.IsNullOrEmpty(folderPath))
            {
                FolderPath = folderPath;
                return;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                FilePath = filePath;
                return;
            }
        }

        public Object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [Serializable()]
    public class ExcludeArray : CollectionBase
    {
        public ExcludeItem this[int index]
        {
            get { return (ExcludeItem)this.List[index]; }
            set { this.List[index] = value;  }
        }

        public ExcludeArray()
        {
        }

        public ExcludeArray(ExcludeArray c)
        {
            if (c.Count > 0)
            {
                foreach (ExcludeItem i in c)
                    this.List.Add(i.Clone());
            }
        }

        public int Add(ExcludeItem item)
        {
            return this.List.Add(item);
        }

        public void Remove(ExcludeItem item)
        {
            this.List.Remove(item);
        }

        public void Clear(ExcludeItem item)
        {
            this.List.Clear();
        }

        protected override void OnValidate(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.GetType() != typeof(ExcludeItem))
                throw new ArgumentException("value must be a ExcludeItem type", "value");

            base.OnValidate(value);
        }
    }
}
