using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIJWT.NET5.Models
{
    public class AuthModel
    {
        public string Message { get; set; }
        public bool IsAutheticated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime ExpireOn { get; set; }
    }
}
