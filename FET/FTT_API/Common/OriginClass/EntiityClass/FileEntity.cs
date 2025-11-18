

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class FileEntity
    {
        public string ID { get; set; }
        public string Status { get; set; }
        public string creator { get; set; }
        public DateTime createtime { get; set; }
        public string updater { get; set; }
        public DateTime updatetime { get; set; }


        public string filename { get; set; }
        public string destfilepath { get; set; }
        public string fileext { get; set; }
        public long filesize { get; set; }
    }

    public class FileDTO : FileEntity
    {
        public int No { get; set; }
    }
}
