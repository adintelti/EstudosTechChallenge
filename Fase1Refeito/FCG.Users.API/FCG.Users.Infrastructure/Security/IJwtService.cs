using FCG.Users.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Users.Infrastructure.Security
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
