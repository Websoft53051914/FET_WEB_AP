using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class user_login_logEntity
    {
        public DateTime createtime { get; set; }

   
        public string? rromipaddress { get; set; }


        public string? usertype { get; set; }

   
        public string? account { get; set; }

        public string? loginstatus { get; set; }


        public string? password { get; set; }

        public string? sessionId { get; set; }
    }

    public class user_login_logDTO : user_login_logEntity
    {

    }
}
