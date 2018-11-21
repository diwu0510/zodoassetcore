using HZC.Database;
using System;

namespace Zodo.Assets.Core
{
    [MyDataTable("Asset_Loan")]
    public class Loan : Entity
    {
        /// <summary>
        /// �ʲ�ID
        /// </summary>
        public int AssetId { get; set; }

        /// <summary>
        /// �ʲ����
        /// </summary>
        public string AssetCode { get; set; }

        /// <summary>
        /// �ʲ�����
        /// </summary>
        public string AssetName { get; set; }

        /// <summary>
        /// ��Դ����
        /// </summary>
        public int FromDeptId { get; set; }

        /// <summary>
        /// ��Դ��������
        /// </summary>
        public string FromDeptName { get; set; }

        /// <summary>
        /// ��Դλ��
        /// </summary>
        public string FromPosition { get; set; }

        /// <summary>
        /// ԭʹ����
        /// </summary>
        public int FromAccountId { get; set; }

        /// <summary>
        /// ԭʹ��������
        /// </summary>
        public string FromAccountName { get; set; }

        /// <summary>
        /// Ŀ�겿��
        /// </summary>
        public int TargetDeptId { get; set; }

        /// <summary>
        /// Ŀ�겿������
        /// </summary>
        public string TargetDeptName { get; set; }

        /// <summary>
        /// Ŀ��λ��
        /// </summary>
        public string TargetPosition { get; set; }

        /// <summary>
        /// Ŀ��Ա��
        /// </summary>
        public int TargetAccountId { get; set; }

        /// <summary>
        /// Ŀ��Ա������
        /// </summary>
        public string TargetAccountName { get; set; }

        /// <summary>
        /// ͼƬ
        /// </summary>
        public string Pics { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        public DateTime LoanAt { get; set; }

        /// <summary>
        /// Ԥ�ƹ黹����
        /// </summary>
        public DateTime ExpectedReturnAt { get; set; }

        /// <summary>
        /// �Ƿ�黹
        /// </summary>
        public bool IsReturn { get; set; }

        /// <summary>
        /// �黹����
        /// </summary>
        public DateTime? ReturnAt { get; set; }

        /// <summary>
        /// ���ԭ��
        /// </summary>
        public string Remark { get; set; }
    }
}
