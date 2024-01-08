using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyCode_Backend_Server.Models
{
    public class User : IdentityUser
    {
        public ICollection<Code> Code { get; set; }

        public User(ICollection<Code> code)
        {
            Code = code;
        }

        public User()
        {
            Code = new List<Code>();
        }
    }
}
