namespace Zodo.Assets.Application
{
    public class AccountListDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ա������
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// �ֻ�����
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// �̶��绰
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// ���ڲ���
        /// </summary>
        public int DeptId { get; set; }

        /// <summary>
        /// ���ڲ�������
        /// </summary>
        public string DeptName { get; set; }
    }
}
