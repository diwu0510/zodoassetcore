using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class AssetCateService
    {
        private MyDbUtil db = new MyDbUtil();

        public Result<int> Create(AssetCate cate, IAppUser user)
        {
            try
            {
                string error = Validate(cate);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do<int>(ResultCodes.��֤ʧ��, 0, error);
                }

                cate.BeforeCreate(user);
                var id = db.Create<AssetCate>(cate);
                if (id > 0)
                {
                    AssetCateUtil.Clear();
                    return ResultUtil.Success<int>(id);
                }
                else
                {
                    return ResultUtil.Do<int>(ResultCodes.���ݿ����ʧ��, 0, "����д��ʧ��");
                }
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception<int>(ex, 0);
            }
        }

        public Result<int> Update(AssetCate cate, IAppUser user)
        {
            try
            {
                string error = Validate(cate);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do<int>(ResultCodes.��֤ʧ��, 0, error);
                }

                if (cate.ParentId == cate.Id)
                {
                    return ResultUtil.Do<int>(ResultCodes.��֤ʧ��, 0, "���ܽ���������Ϊ�ϼ�");
                }

                var children = AssetCateUtil.GetSelfAndChildrenIds(cate.Id);
                if (children.Contains(cate.ParentId))
                {
                    return ResultUtil.Do<int>(ResultCodes.��֤ʧ��, 0, "���ܽ��ϼ�����ָ��Ϊ������");
                }

                cate.BeforeUpdate(user);
                var row = db.Update<AssetCate>(cate);
                if (row > 0)
                {
                    AssetCateUtil.Clear();
                    return ResultUtil.Success<int>(cate.Id);
                }
                else
                {
                    return ResultUtil.Do<int>(ResultCodes.���ݿ����ʧ��, 0, "����д��ʧ��");
                }
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception<int>(ex, 0);
            }
        }

        public Result<int> Save(AssetCate cate, IAppUser user)
        {
            if (cate.Id <= 0)
            {
                return Create(cate, user);
            }
            else
            {
                return Update(cate, user);
            }
        }

        public Result Delete(int id, IAppUser user)
        {
            try
            {
                var entity = db.Load<AssetCate>(id);
                if (entity == null)
                {
                    return ResultUtil.Do(ResultCodes.���ݲ�����, "��������ݲ�����");
                }

                var childrenCount = db.GetCount<AssetCate>(MySearchUtil.New()
                    .AndEqual("ParentId", id)
                    .AndEqual("IsDel", false));
                if (childrenCount > 0)
                {
                    return ResultUtil.Do(ResultCodes.��֤ʧ��, "�������Ϊ�գ���ֹɾ��");
                }

                var assetsCount = db.GetCount<Asset>(MySearchUtil.New()
                    .AndEqual("AssetCateId", id)
                    .AndEqual("IsDel", false));
                if (assetsCount > 0)
                {
                    return ResultUtil.Do(ResultCodes.��֤ʧ��, "�����ʲ���Ϊ�գ���ֹɾ��");
                }

                var row = db.Remove<AssetCate>(id);
                if (row > 0)
                {
                    AssetCateUtil.Clear();
                    return ResultUtil.Success();
                }
                else
                {
                    return ResultUtil.Do(ResultCodes.���ݿ����ʧ��, "���ݿ�д��ʧ��");
                }
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex);
            }
        }

        public List<AssetCateDto> Fetch()
        {
            MySearchUtil util = MySearchUtil.New().AndEqual("IsDel", false);
            var all = db.Fetch<AssetCate>(util);
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
            return db.Load<AssetCate>(id);
        }

        private string Validate(AssetCate cate)
        {
            if (string.IsNullOrWhiteSpace(cate.Name))
            {
                return "������Ʋ���Ϊ��";
            }

            return string.Empty;
        }
    }
}
