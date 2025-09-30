using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Const.VO
{
    public class VerifyTokenResultVO
    {
        public bool IsAvailable { get; set; }

        public bool IsExpired { get; set; }

        public TokenInfoVO TokenInfoVO { get; set; }
    }
}
