using APIJWT.NET5.Models;
using APIJWT.NET5.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIJWT.NET5.Repository
{
   public interface IAuthRepo
    {
        Task<AuthModel> RegisterAsync(RegisterViewModel model);
        Task<AuthModel> GetTokenAsync(GetTokenViewModel model);
        Task<string> AddRoleAsync(AddRoleViewModel model);
    }
}
