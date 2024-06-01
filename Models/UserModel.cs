using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaperWork.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public DateTimeOffset AccessTokenCreated { get; set; }
        public DateTimeOffset Created { get;}

    }
}
