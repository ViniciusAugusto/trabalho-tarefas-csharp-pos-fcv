using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Tarefas.Extended
{
    public static class UserExtended
    {
        public static string GetFullName(this IPrincipal user)
        {
            var claim = ((ClaimsIdentity)user.Identity).FindFirst("FullName");
            return claim == null ? null : claim.Value;
        }

        public static string GetEmail(this IPrincipal user)
        {
            var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Email);
            return claim == null ? null : claim.Value;
        }

        public static string GetNickName(this IPrincipal user)
        {
            var claim = ((ClaimsIdentity)user.Identity).FindFirst("NickName");
            return claim == null ? null : claim.Value;
        }
    }
}