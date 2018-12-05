using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HZC.Database
{
    /// <summary>
    /// ʵ����Ϣ
    /// </summary>
    internal class MyEntityInfo
    {
        #region ����
        /// <summary>
        /// ʵ������
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ʵ���Ӧ�����ݱ����
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// �����б�
        /// </summary>
        public List<MyEntityPropertyInfo> Properties { get; set; }

        /// <summary>
        /// ��׼�������
        /// </summary>
        public string InsertSql { get; set; }

        /// <summary>
        /// ��׼�������
        /// </summary>
        public string UpdateSql { get; set; }
        #endregion

        #region ���췽��

        public MyEntityInfo(Type type)
        {
            Name = type.Name;
            Properties = new List<MyEntityPropertyInfo>();

            // ��ȡ����
            var tableAttrs = type.GetCustomAttributes(typeof(MyDataTableAttribute), false);
            if (tableAttrs == null || tableAttrs.Length == 0)
            {
                TableName = type.Name.Replace("Entity", "");
            }
            else
            {
                var attr = (MyDataTableAttribute)tableAttrs[0];
                TableName = attr.Name;
            }

            foreach (var p in type.GetProperties())
            {
                if (!ValidPropertyType(p))
                {
                    continue;
                }

                var colAttrs = p.GetCustomAttributes(typeof(MyDataFieldAttribute), false);
                if (colAttrs == null || colAttrs.Length == 0)
                {
                    Properties.Add(new MyEntityPropertyInfo
                    {
                        Name = p.Name,
                        ColumnName = p.Name
                    });
                }
                else
                {
                    var attr = (MyDataFieldAttribute)colAttrs[0];

                    Properties.Add(new MyEntityPropertyInfo
                    {
                        Name = p.Name,
                        ColumnName = string.IsNullOrWhiteSpace(attr.ColumnName) ? p.Name : attr.ColumnName,
                        InsertIgnore = attr.InsertIgnore,
                        UpdateIgnore = attr.UpdateIgnore,
                        Ignore = attr.Ignore,
                        IsPrimaryKey = attr.IsPrimaryKey || IsPrimaryKey(Name, p.Name)
                    });
                }
            }

            InsertSql = GetInsertSql();
            UpdateSql = GetUpdateSql();
        }
        #endregion

        #region ˽�з���
        private bool IsPrimaryKey(string entityName, string propertyName)
        {
            if (propertyName == "ID" || propertyName == "Id" || 
                propertyName == entityName + "ID" || propertyName == entityName + "Id")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// �ж�ʵ��������Ƿ��ǿ���ת��Ϊ�����е�����
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private bool ValidPropertyType(PropertyInfo property)
        {
            string name;
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                name = property.PropertyType.GetGenericArguments()[0].Name;
            }
            else
            {
                name = property.PropertyType.Name;
            }
            var types = new []
            {
                "Int", "Int32", "Int64", "Float", "Decimal", "Long", "Boolean", "DateTime",
                "String", "Double", "Char", "Single", "Byte", "UInt32", "UInt64", "Guid"
            };

            return types.Contains(name);
        }

        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <returns></returns>
        private string GetInsertSql()
        {
            var prefix = "@";
            var cols = new List<string>();
            var parameters = new List<string>();

            foreach (var p in Properties)
            {
                if (p.Ignore || p.InsertIgnore || p.IsPrimaryKey) continue;

                cols.Add(p.ColumnName);
                parameters.Add(prefix + p.Name);
            }

            var sb = new StringBuilder();
            sb.Append("INSERT INTO [" + TableName + "] (");
            sb.Append(string.Join(",", cols));
            sb.Append(") VALUES (");
            sb.Append(string.Join(",", parameters));
            sb.Append(");SELECT @@IDENTITY;");
            return sb.ToString();
        }

        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <returns></returns>
        private string GetUpdateSql()
        {
            var prefix = "@";
            var clauses = new List<string>();

            var keyName = "";
            var keyColumnName = "";

            foreach (var p in Properties)
            {
                if (!p.Ignore && !p.UpdateIgnore && !p.IsPrimaryKey)
                {
                    var clause = p.ColumnName + "=" + prefix + p.Name;
                    clauses.Add(clause);
                }
                else if (p.IsPrimaryKey)
                {
                    keyName = p.Name;
                    keyColumnName = p.ColumnName;
                }
            }

            var sb = new StringBuilder();
            sb.Append("UPDATE [" + TableName + "] SET ")
                .Append(string.Join(",", clauses))
                .Append(" WHERE ")
                .Append(keyColumnName)
                .Append("=")
                .Append(prefix)
                .Append(keyName);
            return sb.ToString();
        }
        #endregion
    }
}
