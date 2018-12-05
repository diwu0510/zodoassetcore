using HZC.SearchUtil;

namespace Zodo.Assets.Application
{
    public class AccountSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public int Dept { get; set; }

        /// <summary>
        /// �Ƿ��ϸ�ƥ�䡣Ϊtrueʱ��������ѡ�в����µ�Ա����Ĭ�ϣ�����ѡ�в��ż����������ŵ�Ա����
        /// </summary>
        public bool IsStrict { get; set; } = false;

        public MySearchUtil ToSearchUtil()
        {
            var util = MySearchUtil.New().AndEqual("IsDel", false).OrderByDesc("Id");

            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains(new[] { "Name", "Mobile", "Phone", "Email" }, Key.Trim());
            }

            if (Dept <= 0)
            {
                return util;
            }

            if (!IsStrict)
            {
                var deptIds = DeptUtil.GetSelfAndChildrenIds(Dept);
                util.AndIn("DeptId", deptIds);
            }
            else
            {
                util.AndEqual("DeptId", Dept);
            }

            return util;
        }
    }
}
