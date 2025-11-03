namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
{
    public class vender_pw_historyEntity
    {
        public long Id {  get; set; }

        public string account { get; set; }
        public string pw { get; set; }

        public DateTime createtime { get; set; }


    }

    public class vender_pw_historyDTO : vender_pw_historyEntity
    {

    }
}
