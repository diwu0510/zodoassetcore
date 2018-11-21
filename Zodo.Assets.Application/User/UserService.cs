using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class UserService
    {
        private MyDbUtil db = new MyDbUtil();

        #region ����
        public Result<int> Create(User u, IAppUser user)
        {
            var error = Validate(u);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return ResultUtil.Do<int>(ResultCodes.��֤ʧ��, 0, error);
            }
            u.Pw = AESEncriptUtil.Encrypt("123456");
            u.Version = Guid.NewGuid().ToString("N");
            u.BeforeCreate(user);
            var id = db.Create<User>(u);
            return id > 0 ?  ResultUtil.Success<int>(id) : ResultUtil.Do<int>(ResultCodes.���ݿ����ʧ��, 0);
        }
        #endregion

        #region �޸�
        public Result<int> Update(User u, IAppUser user)
        {
            var error = Validate(u);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return ResultUtil.Do<int>(ResultCodes.��֤ʧ��, 0, error);
            }
            
            u.Version = Guid.NewGuid().ToString("N");
            u.BeforeUpdate(user);
            var id = db.Update<User>(u);
            return id > 0 ? ResultUtil.Success<int>(u.Id) : ResultUtil.Do<int>(ResultCodes.���ݿ����ʧ��, 0);
        }
        #endregion

        #region ����
        public Result<int> Save(User u, IAppUser user)
        {
            if (u.Id <= 0)
            {
                return Create(u, user);
            }
            else
            {
                return Update(u, user);
            }
        }
        #endregion

        #region ɾ��
        public Result Delete(int id)
        {
            var user = Load(id);
            if (user == null)
            {
                return ResultUtil.Do(ResultCodes.��֤ʧ��, "��������ݲ�����");
            }
            if (user.Name == "admin")
            {
                return ResultUtil.Do(ResultCodes.��֤ʧ��, "admin�û���ֹɾ��");
            }

            var row = db.Remove<User>(id);
            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.���ݿ����ʧ��);
        }
        #endregion

        #region ��������
        public Result ResetPw(int id, IAppUser u)
        {
            var user = Load(id);
            if (user == null)
            {
                return ResultUtil.Do(ResultCodes.��֤ʧ��, "��������ݲ�����");
            }
            user.Version = Guid.NewGuid().ToString("N");
            user.Pw = AESEncriptUtil.Encrypt("123456");
            user.BeforeUpdate(u);

            return Update(user, u);
        }
        #endregion

        #region ����ʵ��
        public User Load(int id)
        {
            return db.Load<User>(id);
        }
        #endregion

        #region �б�
        public List<AppUserDto> Fetch(UserSearchParam param)
        {
            return db.Fetch<User>(param.ToSearchUtil()).Select(u => new AppUserDto
            {
                Id = u.Id,
                Name = u.Name
            }).ToList();
        }
        #endregion

        #region �޸�����
        public Result ChangePw(int id, string oldPw, string newPw)
        {
            var entity = Load(id);
            if (entity == null || entity.IsDel)
            {
                return ResultUtil.Do(ResultCodes.��֤ʧ��, "������û������ڻ���ɾ��");
            }

            if (AESEncriptUtil.Decrypt(entity.Pw) != oldPw)
            {
                return ResultUtil.Do(ResultCodes.��֤ʧ��, "ԭʼ���벻��ȷ");
            }

            entity.Pw = AESEncriptUtil.Encrypt(newPw);
            string sql = "UPDATE Base_User SET Pw=@Pw WHERE Id=@Id";
            var row = db.Execute(sql, new { Id = id, Pw = entity.Pw });
            if (row > 0)
            {
                return ResultUtil.Success();
            }
            else
            {
                return ResultUtil.Do(ResultCodes.���ݿ����ʧ��);
            }
        }
        #endregion

        #region ��¼
        public User Login(string name, string pw)
        {
            return db.Load<User>(MySearchUtil.New()
                .AndEqual("Name", name)
                .AndEqual("IsDel", false));
        }
        #endregion

        #region ��ʼ��admin
        public User GetAdmin()
        {
            var u = db.Load<User>(MySearchUtil.New()
                .AndEqual("Name", "admin"));
            return u;
        }
        #endregion

        #region ˽�з���
        private string Validate(User entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                return "�û����Ʋ���Ϊ��";
            }

            var count = db.GetCount<User>(MySearchUtil.New()
                .AndEqual("Name", entity.Name.Trim())
                .AndNotEqual("Id", entity.Id)
                .AndEqual("IsDel", false));
            if (count > 0)
            {
                return "�û��Ѵ���";
            }
            return string.Empty;
        }
        #endregion
    }
}
