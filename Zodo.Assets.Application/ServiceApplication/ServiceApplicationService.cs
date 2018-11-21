using System;
using System.Collections.Generic;
using System.Text;
using HZC.Infrastructure;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class ServiceApplicationService : BaseService<ServiceApplication>
    {
        //public Result<int> Create(ServiceApplication entity, IAppUser user)
        //{
        //    var error = ValidCreate(entity, user);
        //    if (!string.IsNullOrWhiteSpace(error))
        //    {
        //        return ResultUtil.Do<int>(ResultCodes.��֤ʧ��, 0, error);
        //    }

        //    entity.BeforeCreate(user);
        //    var id = db.Create<ServiceApplication>(entity);
        //    return id > 0 ? ResultUtil.Success<int>(id) : ResultUtil.Do<int>(ResultCodes.���ݿ����ʧ��, 0);
        //}

        public PageList<ServiceApplication> List(ServiceApplicationSearchParam param, int pageIndex, int pageSize)
        {
            var pageList = db.Query<ServiceApplication>(param.ToSearchUtil(), pageIndex, pageSize);
            return pageList;
        }

        public override string ValidCreate(ServiceApplication entity, IAppUser user)
        {
            if (entity.DeptId <= 0)
            {
                return "���벿�Ų���Ϊ��";
            }

            if (string.IsNullOrWhiteSpace(entity.AccountName))
            {
                return "�����˲���Ϊ��";
            }

            if (entity.RequireCompleteAt == null || !ValidDate(entity.RequireCompleteAt))
            {
                return "Ҫ����ʱ�䲻�Ϸ�";
            }

            if (entity.ApplyAt == null || !ValidDate(entity.ApplyAt))
            {
                return "�������ڲ��Ϸ�";
            }

            return string.Empty;
        }

        public override string ValidDelete(ServiceApplication entity, IAppUser user)
        {
            return string.Empty;
        }

        public override string ValidUpdate(ServiceApplication entity, IAppUser user)
        {
            return string.Empty;
        }
    }
}
