
using Const;
using static Const.Enums;

namespace FTT_API.Common
{

    //原
    public partial class SessionVO
    {
        public string empno { get; set; }
        public string empname { get; set; }
        public string engname { get; set; }
        public string ext { get; set; }
        public string username { get; set; }
        public string deptcode { get; set; }
        public string usertype { get; set; }
        public string ivrcode { get; set; }

        public string userrole { get; set; }


        public IEnumerable<FuncID> Functions { get; set; } = Enumerable.Empty<FuncID>();

    }
}
