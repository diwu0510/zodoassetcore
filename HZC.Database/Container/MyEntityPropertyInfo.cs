namespace HZC.Database
{
    /// <summary>
    /// ʵ��������Ϣ
    /// </summary>
    internal class MyEntityPropertyInfo
    {
        /// <summary>
        /// ʵ����������
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ʵ�����Զ�Ӧ������
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        public bool IsPrimaryKey { get; set; } = false;

        /// <summary>
        /// ����ʱ����
        /// </summary>
        public bool InsertIgnore { get; set; } = false;

        /// <summary>
        /// ����ʱ����
        /// </summary>
        public bool UpdateIgnore { get; set; } = false;

        /// <summary>
        /// ����͸���ʱ������
        /// </summary>
        public bool Ignore { get; set; } = false;
    }
}
