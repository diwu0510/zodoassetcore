using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace HZC.Infrastructure
{
    public class ConfigurationManager
    {
        /// <summary>
        /// ��������
        /// </summary>
        private static NameValueCollection _configurationCollection = new NameValueCollection();

        /// <summary>
        /// ���ü�����Ӧ����ջ
        /// </summary>
        private static Stack<KeyValuePair<string, FileSystemWatcher>> FileListeners = new Stack<KeyValuePair<string, FileSystemWatcher>>();

        /// <summary>
        /// Ĭ��·��
        /// </summary>
        private static string _defaultPath = Directory.GetCurrentDirectory() + "\\appsettings.json";

        /// <summary>
        /// ���������ļ�·��
        /// </summary>
        private static string _configPath;

        /// <summary>
        /// ���ýڵ�ؼ���
        /// </summary>
        private static string _configSection = "AppSettings";

        /// <summary>
        /// ���������ӵĺ�׺
        /// </summary>
        private static string _configUrlPostfix = "Url";

        /// <summary>
        /// �����޸�ʱ���
        /// </summary>
        private static long _timeStamp;

        /// <summary>
        /// ���������ؼ��ʣ����磺AppSettings.Url
        /// </summary>
        private static string ConfigUrlSection => _configSection + "." + _configUrlPostfix;


        static ConfigurationManager()
        {
            ConfigFinder(_defaultPath);
        }

        /// <summary>
        /// ȷ�������ļ�·��
        /// </summary>
        private static void ConfigFinder(string path)
        {
            _configPath = path;
            JObject configJson;
            while (true)
            {
                configJson = null;
                var configInfo = new FileInfo(_configPath);
                if (!configInfo.Exists) break;

                FileListeners.Push(CreateListener(configInfo));
                configJson = LoadJsonFile(_configPath);
                if (configJson[ConfigUrlSection] != null)
                    _configPath = configJson[ConfigUrlSection].ToString();
                else break;
            }

            if (configJson?[_configSection] == null) return;

            LoadConfiguration();
        }

        /// <summary>
        /// ��ȡ�����ļ�����
        /// </summary>
        private static void LoadConfiguration()
        {
            var configObject = LoadJsonFile(_configPath);
            if (configObject == null) return;

            var configColltion = new NameValueCollection();
            if (configObject[_configSection] != null)
            {
                foreach (var jToken in configObject[_configSection])
                {
                    var prop = (JProperty) jToken;
                    configColltion[prop.Name] = prop.Value.ToString();
                }
            }

            _configurationCollection = configColltion;
        }

        /// <summary>
        /// ����Json�ļ�
        /// </summary>
        /// <param name="filePath">�ļ�·��</param>
        /// <returns></returns>
        private static JObject LoadJsonFile(string filePath)
        {
            JObject configObject = null;
            try
            {
                var sr = new StreamReader(filePath, Encoding.Default);
                configObject = JObject.Parse(sr.ReadToEnd());
                sr.Close();
            }
            catch (Exception)
            {
                // ignored
            }

            return configObject;
        }

        /// <summary>
        /// ��Ӽ������ڵ�
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static KeyValuePair<string, FileSystemWatcher> CreateListener(FileInfo info)
        {

            var watcher = new FileSystemWatcher();
            watcher.BeginInit();
            watcher.Path = info.DirectoryName;
            watcher.Filter = info.Name;
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size;
            watcher.Changed += ConfigChangeListener;
            watcher.EndInit();

            return new KeyValuePair<string, FileSystemWatcher>(info.FullName, watcher);

        }

        private static void ConfigChangeListener(object sender, FileSystemEventArgs e)
        {
            var time = TimeStamp();
            lock (FileListeners)
            {
                if (time > _timeStamp)
                {
                    _timeStamp = time;
                    if (e.FullPath != _configPath || e.FullPath == _defaultPath)
                    {
                        while (FileListeners.Count > 0)
                        {
                            var listener = FileListeners.Pop();
                            listener.Value.Dispose();
                            if (listener.Key == e.FullPath) break;
                        }
                        ConfigFinder(e.FullPath);
                    }
                    else
                    {
                        LoadConfiguration();
                    }
                }
            }
        }

        private static long TimeStamp()
        {
            return (long)((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds * 100);
        }

        private static string _cConfigSection;
        public static string ConfigSection
        {
            get => _configSection;
            set => _cConfigSection = value;
        }


        private static string _cConfigUrlPostfix;
        public static string ConfigUrlPostfix
        {
            get => _configUrlPostfix;
            set => _cConfigUrlPostfix = value;
        }

        private static string _cDefaultPath;
        public static string DefaultPath
        {
            get => _defaultPath;
            set => _cDefaultPath = value;
        }

        public static NameValueCollection AppSettings => _configurationCollection;

        /// <summary>
        /// �ֶ�ˢ�����ã��޸����ú����ֶ����ô˷������Ա�������ò���
        /// </summary>
        public static void RefreshConfiguration()
        {
            lock (FileListeners)
            {
                //�޸�����
                if (_cConfigSection != null) { _configSection = _cConfigSection; _cConfigSection = null; }
                if (_cConfigUrlPostfix != null) { _configUrlPostfix = _cConfigUrlPostfix; _cConfigUrlPostfix = null; }
                if (_cDefaultPath != null) { _defaultPath = _cDefaultPath; _cDefaultPath = null; }
                //�ͷŵ�ȫ��������Ӧ��
                while (FileListeners.Count > 0)
                    FileListeners.Pop().Value.Dispose();
                ConfigFinder(_defaultPath);
            }
        }

    }
}
