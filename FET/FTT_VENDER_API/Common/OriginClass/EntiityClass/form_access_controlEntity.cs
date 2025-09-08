namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
{
    public class form_access_controlEntity
    {
        public string orderid { get; set; }
        public string form_type { get; set; }
        public string User_Type { get; set; }
        public string status { get; set; }
        public string allow_status { get; set; }
        public string allow_wording { get; set; }
        public string require_field { get; set; }
        public string option_field { get; set; }
        public string approve { get; set; }
        public string eof { get; set; }
        public string bof { get; set; }
        public string lastorderid { get; set; }
        public string beginorderid { get; set; }
        public string carbon { get; set; }
        public string remark { get; set; }
        public string usertype { get; set; }
    }

    public class form_access_controlDTO : form_access_controlEntity
    {
        public int No { get; set; }
        public string STATUS_NAME { get; set; }
        public string SQL { get; set; }
    }


}
