using HZC.Infrastructure;
using HZC.SearchUtil;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class MenuService : BaseService<Menu>
    {
        public List<MenuDto> FetchDto()
        {
            return db.FetchBySql<MenuDto>("SELECT Id,Name,ParentId,Icon,Url,Sort FROM Base_Menu WHERE IsDel=0").ToList();
        }

        public override string ValidCreate(Menu entity, IAppUser user)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                return "�˵����Ʋ���Ϊ��";
            }

            if (string.IsNullOrWhiteSpace(entity.Icon))
            {
                return "�˵�ͼ�겻��Ϊ��";
            }

            return string.Empty;
        }

        public override string ValidDelete(Menu entity, IAppUser user)
        {
            var count = db.GetCount<Menu>(MySearchUtil.New()
                .AndEqual("IsDel", false)
                .AndEqual("ParentId", entity.Id));
            return count > 0 ? "�¼��˵���Ϊ�գ���ֹɾ��" : string.Empty;
        }

        public override string ValidUpdate(Menu entity, IAppUser user)
        {
            return ValidCreate(entity, user);
        }
    }
}
