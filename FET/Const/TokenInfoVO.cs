using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Const
{
    public class TokenInfoVO
    {
        public DateTime Iat { get; set; }

        public DateTime Exp { get; set; }

        public DateTime Nbf { get; set; }

        public string TokenId { get; set; }

        public string LogAccount { get; set; }

        public int Status { get; set; }
    }
}
