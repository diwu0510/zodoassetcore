using HZC.SearchUtil;
using System.Linq;

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
            MySearchUtil util = MySearchUtil.New().AndEqual("IsDel", false).OrderByDesc("Id");

            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains(new string[] { "Name", "Mobile", "Phone", "Email" }, Key.Trim());
            }

            if (Dept > 0)
            {
                if (!IsStrict)
                {
                    var deptIds = DeptUtil.GetSelfAndChildrenIds(Dept);
                    util.AndIn("DeptId", deptIds);
                }
                else
                {
                    util.AndEqual("DeptId", Dept);
                }
            }
            
            return util;
        }
    }
}
