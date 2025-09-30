using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Const.VO
{
    public class LoginResultVO
    {
        public string ErrorMsg { get; set; } = string.Empty;

        public TokenInfoVO Token { get; set; }
    }
}
