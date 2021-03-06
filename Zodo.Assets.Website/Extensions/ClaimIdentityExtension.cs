﻿using System;
using System.Security.Claims;

namespace Zodo.Assets.Website
{
    public static class ClaimIdentityExtension
    {
        public static string FindFirstValue(this ClaimsPrincipal identity, string claimType)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            var claim = identity.FindFirst(claimType);
            if (claim != null)
            {
                return claim.Value;
            }
            return string.Empty;
        }

        public static string FindName(this ClaimsPrincipal identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            return identity.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        }

        public static int FindId(this ClaimsPrincipal identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            var idStr = identity.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            int.TryParse(idStr, out var id);
            return id;
        }

        public static string GetWeixinUserId(this ClaimsPrincipal identity)
        {
            if (identity == null)
            {
                throw new ArgumentException("identity");
            }

            var userid = identity.FindFirstValue("WeixinUserId");
            return userid.ToLower();
        }

        public static string GetWeixinUserName(this ClaimsPrincipal identity)
        {
            if (identity == null)
            {
                throw new ArgumentException("identity");
            }

            var userName = identity.FindFirstValue("WeixinUserName");
            return userName;
        }

        public static int GetWeixinDeptId(this ClaimsPrincipal identity)
        {
            if (identity == null)
            {
                throw new ArgumentException("identity");
            }

            var deptId = identity.FindFirstValue("WeixinDeptId");

            int.TryParse(deptId, out var result);
            return result;
        }

        public static string GetWeixinDeptName(this ClaimsPrincipal identity)
        {
            if (identity == null)
            {
                throw new ArgumentException("identity");
            }

            var deptName = identity.FindFirstValue("WeixinDeptName");
            return deptName;
        }
    }
}
