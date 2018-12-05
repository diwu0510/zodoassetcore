using System;
using System.Collections.Concurrent;

namespace HZC.Database
{
    /// <summary>
    /// ����ʵ�������
    /// </summary>
    internal class MyContainer
    {
        /// <summary>
        /// ʵ�弰ʵ����Ϣ���ֵ�
        /// </summary>
        private static ConcurrentDictionary<string, MyEntityInfo> _dict = new ConcurrentDictionary<string, MyEntityInfo>();

        #region ��������
        public static MyEntityInfo Get(Type type)
        {
            if (_dict.TryGetValue(type.Name, out var result)) return result;

            result = new MyEntityInfo(type);
            _dict.TryAdd(type.Name, result);
            return result;
        }
        #endregion
    }
}
