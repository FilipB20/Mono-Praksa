using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MonoPraksa.Model;

namespace MonoPraksa.Service;

public interface IAuthService
{
    Task<bool> RegisterAsync(UserRegisterDto dto);
    Task<string?> LoginAsync(UserLoginDto dto);
}
