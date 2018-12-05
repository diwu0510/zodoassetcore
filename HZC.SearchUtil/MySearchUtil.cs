using Dapper;
using HZC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HZC.SearchUtil
{
    public class MySearchUtil
    {
        #region ����
        private string _sqlParameterPrefix;

        public MySearchUtil(DbTypes dbType = DbTypes.SqlServer)
        {
            _sqlParameterPrefix = DbTypeUtil.GetDbParamPrefix(dbType);
        }

        public static MySearchUtil New(DbTypes dbType = DbTypes.SqlServer)
        {
            return new MySearchUtil(dbType);
        }
        #endregion

        #region �ֶ�
        private int _idx;
        private string _conditionClauses = string.Empty;
        private readonly List<string> _cols = new List<string>();
        private readonly List<string> _orderby = new List<string>();
        private readonly DynamicParameters _params = new DynamicParameters();
        #endregion

        #region ����
        /// <summary>
        /// where �Ӿ�
        /// </summary>
        public string ConditionClaus => string.IsNullOrWhiteSpace(_conditionClauses) ? "1=1" : _conditionClauses;

        /// <summary>
        /// ��ѯ����
        /// </summary>
        public DynamicParameters Parameters => _params;

        /// <summary>
        /// ��ҳ���ݲ�ѯ����
        /// </summary>
        public DynamicParameters PageListParameters
        {
            get
            {
                _params.Add("RecordCount", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                return _params;
            }
        }

        /// <summary>
        /// orderby �Ӿ�
        /// </summary>
        public string OrderByClaus => _orderby.Count == 0 ? string.Empty : string.Join(",", _orderby);

        /// <summary>
        /// Ҫ��ѯ����
        /// </summary>
        public string Columns => _cols.Count == 0 ? "*" : string.Join(",", _cols);

        #endregion

        #region ֱ��ָ���ַ���
        public MySearchUtil And(string conditionString)
        {
            if (string.IsNullOrWhiteSpace(conditionString)) return this;

            if (!string.IsNullOrWhiteSpace(_conditionClauses))
            {
                _conditionClauses += " And ";
            }
            _conditionClauses += conditionString;

            return this;
        }

        public MySearchUtil Or(string conditionString)
        {
            if (string.IsNullOrWhiteSpace(conditionString)) return this;

            if (!string.IsNullOrWhiteSpace(_conditionClauses))
            {
                _conditionClauses = "(" + _conditionClauses + ") Or ";
            }
            _conditionClauses += conditionString;

            return this;
        }

        public MySearchUtil AndOr(string conditionString)
        {
            if (string.IsNullOrWhiteSpace(conditionString)) return this;

            if (!string.IsNullOrWhiteSpace(_conditionClauses))
            {
                _conditionClauses += " And ";
            }
            _conditionClauses += "(" + conditionString + ")";

            return this;
        }
        #endregion

        #region And
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="column">����</param>
        /// <param name="value">ֵ</param>
        /// <returns></returns>
        public MySearchUtil AndEqual(string column, object value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return And(column + "=" + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndNotEqual(string column, object value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return And(column + "<>" + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndGreaterThan(string column, object value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return And(column + ">" + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// ���ڵ���
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndGreaterThanEqual(string column, object value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return And(column + ">=" + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// С��
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndLessThan(string column, object value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return And(column + "<" + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// С�ڵ���
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndLessThanEqual(string column, object value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return And(column + "<=" + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndContains(string column, string value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, "%" + value + "%");
            return And(column + " LIKE " + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndContains(string[] columns, string value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, "%" + value + "%");

            if (!columns.Any()) return this;
            if (!string.IsNullOrWhiteSpace(_conditionClauses))
            {
                _conditionClauses += " AND ";
            }
            _conditionClauses += "(";
            var claus = new List<string>();
            foreach (var column in columns)
            {
                claus.Add(column + " LIKE " + _sqlParameterPrefix +  paramName);
            }
                
            _conditionClauses += string.Join(" Or ", claus) + ")";

            return this;
        }

        /// <summary>
        /// �����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndStartWith(string column, string value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value + "%");
            return And(column + " LIKE " + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// �Ұ���
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndEndWith(string column, string value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, "%" + value);
            return And(column + " LIKE " + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// In�Ӿ�-����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndIn(string column, int[] value)
        {
            if (!value.Any())
            {
                return this;
            }

            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return And(column + " IN " + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// In�Ӿ�-����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndIn(string column, float[] value)
        {
            if (!value.Any())
            {
                return this;
            }

            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return And(column + " IN " + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// In�Ӿ�-����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndIn(string column, long[] value)
        {
            if (!value.Any())
            {
                return this;
            }

            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return And(column + " IN " + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// In�Ӿ�-����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndIn(string column, decimal[] value)
        {
            if (!value.Any())
            {
                return this;
            }

            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return And(column + " IN " + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// In�Ӿ�-�ı�
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndIn(string column, string[] value)
        {
            if (!value.Any())
            {
                return this;
            }

            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return And(column + " IN " + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// In�Ӿ�-����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MySearchUtil AndIn(string column, DateTime[] value)
        {
            if (!value.Any())
            {
                return this;
            }

            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return And(column + " IN " + _sqlParameterPrefix + paramName);
        }

        /// <summary>
        /// ��Ϊ��
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MySearchUtil AndNotNull(string column)
        {
            return And(column + " IS NOT NULL");
        }

        /// <summary>
        /// ��Ϊ���Ҳ�Ϊ���ַ���
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MySearchUtil AndNotNullOrEmpty(string column)
        {
            return And(column + " IS NOT NULL And " + column + " <> ''");
        }

        /// <summary>
        /// Ϊ��
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MySearchUtil AndIsNull(string column)
        {
            return And(column + " IS NULL");
        }

        /// <summary>
        /// Ϊ�ջ���ַ���
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MySearchUtil AndIsNullOrEmpty(string column)
        {
            return And("(" + column + " IS NULL Or " + column + " = '')");
        }
        #endregion

        #region Or
        public MySearchUtil OrEqual(string column, object value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return Or(column + "=" + _sqlParameterPrefix + paramName);
        }

        public MySearchUtil OrGreaterThan(string column, object value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return Or(column + ">" + _sqlParameterPrefix + paramName);
        }

        public MySearchUtil OrGreaterThanEqual(string column, object value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return Or(column + ">=" + _sqlParameterPrefix + paramName);
        }

        public MySearchUtil OrLessThan(string column, object value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return Or(column + "<" + _sqlParameterPrefix + paramName);
        }

        public MySearchUtil OrLessThanEqual(string column, object value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return Or(column + "<=" + _sqlParameterPrefix + paramName);
        }

        public MySearchUtil OrContains(string column, string value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, "%" + value + "%");
            return Or(column + " LIKE " + _sqlParameterPrefix + paramName);
        }

        public MySearchUtil OrStartWith(string column, string value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, value + "%");
            return Or(column + " LIKE " + _sqlParameterPrefix + paramName);
        }

        public MySearchUtil OrEndWith(string column, string value)
        {
            var paramName = "p" + _idx++;
            _params.Add(paramName, "%" + value);
            return Or(column + " LIKE " + _sqlParameterPrefix + paramName);
        }

        public MySearchUtil OrIn(string column, object[] value)
        {
            if (!value.Any())
            {
                return this;
            }

            var paramName = "p" + _idx++;
            _params.Add(paramName, value);
            return Or(column + " IN " + _sqlParameterPrefix + paramName);
        }
        #endregion

        #region ����
        public MySearchUtil OrderBy(string column)
        {
            _orderby.Add(column);
            return this;
        }

        public MySearchUtil OrderByDesc(string column)
        {
            _orderby.Add(column + " DESC");
            return this;
        }
        #endregion
    }
}
