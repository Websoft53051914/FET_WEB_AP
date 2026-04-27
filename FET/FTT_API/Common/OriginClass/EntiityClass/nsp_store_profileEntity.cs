using System.ComponentModel.DataAnnotations;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    /// <summary>
    /// NSP門市資料實體類別
    /// </summary>
    public class nsp_store_profileEntity
    {
        [Key]
        public string ivr_code { get; set; }                    // PRIMARY KEY
        
        public string company_leaves { get; set; }              // 公司代碼，固定值 'FET'
        
        public string store_type { get; set; }                  // 門市類型：RETAIL 或 FRANCHISE
        
        public string channel { get; set; }                     // 通路
        
        public string area { get; set; }                        // 區域
        
        public string shop_name { get; set; }                   // 門市名稱
        
        public string email { get; set; }                       // 電子郵件
        
        public string owner_empno { get; set; }                 // 店長工號
        
        public string as_empno { get; set; }                    // 業務工號
        
        public string store_tel { get; set; }                   // 門市電話
        
        public string fax_tel { get; set; }                     // 傳真電話
        
        public string address { get; set; }                     // 地址
        
        public string STOREOPENTM_MON { get; set; }             // 週一開店時間
        
        public string STORECLOSETM_MON { get; set; }            // 週一關店時間
        
        public string STOREOPENTM_TUE { get; set; }             // 週二開店時間
        
        public string STORECLOSETM_TUE { get; set; }            // 週二關店時間
        
        public string STOREOPENTM_WED { get; set; }             // 週三開店時間
        
        public string STORECLOSETM_WED { get; set; }            // 週三關店時間
        
        public string STOREOPENTM_THU { get; set; }             // 週四開店時間
        
        public string STORECLOSETM_THU { get; set; }            // 週四關店時間
        
        public string STOREOPENTM_FRI { get; set; }             // 週五開店時間
        
        public string STORECLOSETM_FRI { get; set; }            // 週五關店時間
        
        public string STOREOPENTM_SAT { get; set; }             // 週六開店時間
        
        public string STORECLOSETM_SAT { get; set; }            // 週六關店時間
        
        public string STOREOPENTM_SUN { get; set; }             // 週日開店時間
        
        public string STORECLOSETM_SUN { get; set; }            // 週日關店時間
        
        public DateTime? ftt_synctime { get; set; }             // FTT系統同步時間
    }

    /// <summary>
    /// Oracle VIEW_DP2FTT 檢視表實體類別
    /// </summary>
    public class VIEW_DP2FTTEntity
    {
        public string STORESTYLE { get; set; }                  // 門市風格
        
        public string STORETYPE { get; set; }                   // 門市類型
        
        public string REGIONNAME { get; set; }                  // 區域名稱
        
        public string STORENAME { get; set; }                   // 門市名稱
        
        public string STOREID { get; set; }                     // 門市ID
        
        public string EMAIL { get; set; }                       // 電子郵件
        
        public string storemanager_empno { get; set; }          // 店長工號
        
        public string sales_empno { get; set; }                 // 業務工號
        
        public string CONTACTNUM1 { get; set; }                 // 聯絡電話1
        
        public string FAXNUM { get; set; }                      // 傳真號碼
        
        public string STOREADDRESS { get; set; }                // 門市地址
        
        public string STOREOPENTM_MON { get; set; }             // 週一開店時間
        
        public string STORECLOSETM_MON { get; set; }            // 週一關店時間
        
        public string STOREOPENTM_TUE { get; set; }             // 週二開店時間
        
        public string STORECLOSETM_TUE { get; set; }            // 週二關店時間
        
        public string STOREOPENTM_WED { get; set; }             // 週三開店時間
        
        public string STORECLOSETM_WED { get; set; }            // 週三關店時間
        
        public string STOREOPENTM_THU { get; set; }             // 週四開店時間
        
        public string STORECLOSETM_THU { get; set; }            // 週四關店時間
        
        public string STOREOPENTM_FRI { get; set; }             // 週五開店時間
        
        public string STORECLOSETM_FRI { get; set; }            // 週五關店時間
        
        public string STOREOPENTM_SAT { get; set; }             // 週六開店時間
        
        public string STORECLOSETM_SAT { get; set; }            // 週六關店時間
        
        public string STOREOPENTM_SUN { get; set; }             // 週日開店時間
        
        public string STORECLOSETM_SUN { get; set; }            // 週日關店時間
    }

    public class nsp_store_profileDTO : nsp_store_profileEntity
    {
        public int No { get; set; }
    }
}
