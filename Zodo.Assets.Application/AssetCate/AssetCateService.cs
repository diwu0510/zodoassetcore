using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class AssetCateService
    {
        private readonly MyDbUtil _db = new MyDbUtil();

        public Result<int> Create(AssetCate cate, IAppUser user)
        {
            try
            {
                var error = Validate(cate);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do(ResultCodes.��֤ʧ��, 0, error);
                }

                cate.BeforeCreate(user);
                var id = _db.Create(cate);
                if (id > 0)
                {
                    AssetCateUtil.Clear();
                    return ResultUtil.Success(id);
                }

                return ResultUtil.Do(ResultCodes.���ݿ����ʧ��, 0, "����д��ʧ��");
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex, 0);
            }
        }

        public Result<int> Update(AssetCate cate, IAppUser user)
        {
            try
            {
                var error = Validate(cate);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do(ResultCodes.��֤ʧ��, 0, error);
                }

                if (cate.ParentId == cate.Id)
                {
                    return ResultUtil.Do(ResultCodes.��֤ʧ��, 0, "���ܽ���������Ϊ�ϼ�");
                }

                var children = AssetCateUtil.GetSelfAndChildrenIds(cate.Id);
                if (children.Contains(cate.ParentId))
                {
                    return ResultUtil.Do(ResultCodes.��֤ʧ��, 0, "���ܽ��ϼ�����ָ��Ϊ������");
                }

                cate.BeforeUpdate(user);
                var row = _db.Update(cate);
                if (row > 0)
                {
                    AssetCateUtil.Clear();
                    return ResultUtil.Success(cate.Id);
                }

                return ResultUtil.Do(ResultCodes.���ݿ����ʧ��, 0, "����д��ʧ��");
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex, 0);
            }
        }

        public Result<int> Save(AssetCate cate, IAppUser user)
        {
            return cate.Id <= 0 ? Create(cate, user) : Update(cate, user);
        }

        public Result Delete(int id, IAppUser user)
        {
            try
            {
                var entity = _db.Load<AssetCate>(id);
                if (entity == null)
                {
                    return ResultUtil.Do(ResultCodes.���ݲ�����, "��������ݲ�����");
                }

                var childrenCount = _db.GetCount<AssetCate>(MySearchUtil.New()
                    .AndEqual("ParentId", id)
                    .AndEqual("IsDel", false));
                if (childrenCount > 0)
                {
                    return ResultUtil.Do(ResultCodes.��֤ʧ��, "�������Ϊ�գ���ֹɾ��");
                }

                var assetsCount = _db.GetCount<Asset>(MySearchUtil.New()
                    .AndEqual("AssetCateId", id)
                    .AndEqual("IsDel", false));
                if (assetsCount > 0)
                {
                    return ResultUtil.Do(ResultCodes.��֤ʧ��, "�����ʲ���Ϊ�գ���ֹɾ��");
                }

                var row = _db.Remove<AssetCate>(id);
                if (row > 0)
                {
                    AssetCateUtil.Clear();
                    return ResultUtil.Success();
                }

                return ResultUtil.Do(ResultCodes.���ݿ����ʧ��, "���ݿ�д��ʧ��");
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex);
            }
        }

        public List<AssetCateDto> Fetch()
        {
            var util = MySearchUtil.New().AndEqual("IsDel", false);
            var all = _db.Fetch<AssetCate>(util);
            return all.Select(a => new AssetCateDto
            {
                Id = a.Id,
                Name = a.Name,
                ParentId = a.ParentId,
                Sort = a.Sort
            }).ToList();
        }

        public AssetCate Load(int id)
        {
            return _db.Load<AssetCate>(id);
        }

        private string Validate(AssetCate cate)
        {
            return string.IsNullOrWhiteSpace(cate.Name) ? "������Ʋ���Ϊ��" : string.Empty;
        }
    }
}
