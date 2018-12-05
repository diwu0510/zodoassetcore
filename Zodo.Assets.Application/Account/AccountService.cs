using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class AccountService
    {
        private readonly MyDbUtil _db = new MyDbUtil();

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Result<int> Create(Account entity, IAppUser user)
        {
            try
            {
                var error = Validate(entity);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do<int>(ResultCodes.��֤ʧ��, 0, error);
                }

                entity.BeforeCreate(user);
                var id = _db.Create<Account>(entity);
                return id > 0 ? ResultUtil.Success<int>(id) : ResultUtil.Do<int>(ResultCodes.���ݿ����ʧ��, 0, "����д��ʧ��");
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception<int>(ex, 0);
            }
        }

        /// <summary>
        /// �޸�
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Result<int> Update(Account entity, IAppUser user)
        {
            try
            {
                var error = Validate(entity);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do(ResultCodes.��֤ʧ��, 0, error);
                }

                entity.BeforeUpdate(user);
                var row = _db.Update(entity);
                if (row <= 0) return ResultUtil.Do(ResultCodes.���ݿ����ʧ��, 0, "����д��ʧ��");
                _db.Execute(@"UPDATE [Asset_Asset] SET DeptId=@DeptId WHERE AccountId=@AccountId",
                    new {AccountId = entity.Id, DeptId = entity.DeptId});
                return ResultUtil.Success(entity.Id);
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex, 0);
            }
        }

        /// <summary>
        /// ���棻id > 0 �������ݣ�id == 0 ������ݣ�
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Result<int> Save(Account entity, IAppUser user)
        {
            return entity.Id <= 0 ? Create(entity, user) : Update(entity, user);
        }

        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Result Delete(int id, IAppUser user)
        {
            try
            {
                var entity = _db.Load<Account>(id);
                if (entity == null)
                {
                    return ResultUtil.Do(ResultCodes.���ݲ�����, "��������ݲ�����");
                }

                var assetCount = _db.GetCount<Asset>(MySearchUtil.New()
                    .AndEqual("AccountId", id)
                    .AndEqual("IsDel", false));
                if (assetCount > 0)
                {
                    return ResultUtil.Do(ResultCodes.��֤ʧ��, "���û����ʲ���Ϊ�գ���ֹɾ��");
                }

                var row = _db.Remove<Account>(id);
                return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.���ݿ����ʧ��, "���ݿ�д��ʧ��");
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex);
            }
        }

        /// <summary>
        /// ��ȡ��ҳ�б�
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageList<AccountListDto> PageList(AccountSearchParam input, int pageIndex, int pageSize)
        {
            var util = input.ToSearchUtil();
            return _db.Query<AccountListDto>(util, pageIndex, pageSize, "AccountView");
        }

        /// <summary>
        /// ����ʵ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Account Load(int id)
        {
            var entity = _db.Load<Account>(id);
            if (entity == null || entity.IsDel)
            {
                return null;
            }

            return entity;
        }

        /// <summary>
        /// ɾ��ʵ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result Delete(int id)
        {
            var account = Load(id);
            if (account == null)
            {
                return ResultUtil.Do(ResultCodes.��֤ʧ��, "��������ݲ�����");
            }

            var assetCount = _db.GetCount<Asset>(MySearchUtil.New()
                .AndEqual("AccountId", id)
                .AndEqual("IsDel", false)
                .AndNotEqual("State", "����"));

            if (assetCount > 0)
            {
                return ResultUtil.Do(ResultCodes.��֤ʧ��, "���û����ʲ���Ϊ��");
            }

            var row = _db.Remove<Account>(id);
            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.���ݿ����ʧ��);
        }

        /// <summary>
        /// ��ȡ�û�������Ϣ
        /// </summary>
        /// <param name="deptId">����id</param>
        /// <returns></returns>
        public List<AccountBaseDto> GetAccountsBaseInfo(int? deptId, string key = "")
        {
            var param = new AccountSearchParam
            {
                IsStrict = true
            };
            if (deptId.HasValue)
            {
                param.Dept = (int)deptId;
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                param.Key = key;
            }
            return _db.Fetch<AccountBaseDto>(param.ToSearchUtil(), "Base_Account", "Id,Name,DeptId").ToList();
        }

        #region ˽�з���
        private string Validate(Account entity)
        {
            return string.IsNullOrWhiteSpace(entity.Name) ? "Ա����������Ϊ��" : string.Empty;
        }
        #endregion
    }
}
