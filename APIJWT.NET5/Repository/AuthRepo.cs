using APIJWT.NET5.Helper;
using APIJWT.NET5.Models;
using APIJWT.NET5.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APIJWT.NET5.Repository
{
    public class AuthRepo : IAuthRepo
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        JWT _Jwt;
        public AuthRepo(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JWT> Jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _Jwt = Jwt.Value;
        }

        public async Task<string> AddRoleAsync(AddRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null)
            {
                return "Invalid UserName Or Role";
            }
            if (await _userManager.IsInRoleAsync(user,model.RoleName))
            {
                return "User Already Assign To This Role";
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            return result.Succeeded ? string.Empty : "Something went wrong";
            //if (result.Succeeded)
            //{
            //    return string.Empty;
            //}
            //return "Something went wrong";
        }

        public async Task<AuthModel> GetTokenAsync(GetTokenViewModel model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null ||! await _userManager.CheckPasswordAsync(user,model.Password))
            {
                authModel.Message = "Email or Password Incorrect";
                return authModel;
            }
            authModel.IsAutheticated = true;
            var roleList = await _userManager.GetRolesAsync(user);
            var JWTSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
                ExpireOn = JWTSecurityToken.ValidTo,
                IsAutheticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(JWTSecurityToken),
                Roles = roleList.ToList()
            };



        }

        public async Task<AuthModel> RegisterAsync(RegisterViewModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthModel { Message = "Email Is Already Register !" };
            }
            if (await _userManager.FindByNameAsync(model.UserName) is not null)
            {
                return new AuthModel { Message = "UserName Is Already Register !" };
            }

            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,

            };
          var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                string errors = "";
                foreach (var error in result.Errors)
                {
                    errors += error.Description;
                    errors += ',';
                }
                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "User");
            var JWTSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
                ExpireOn = JWTSecurityToken.ValidTo,
                IsAutheticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(JWTSecurityToken)
            };
        }


        async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),

            }.Union(userClaims)
            .Union(roleClaims);

          var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwt.Key));
          var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var JwtSecurityToken = new JwtSecurityToken(
                issuer: _Jwt.Issuer,
                audience: _Jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_Jwt.DurationInDays),
                signingCredentials: signingCredentials

                );

            return JwtSecurityToken;
        }
    }
}
