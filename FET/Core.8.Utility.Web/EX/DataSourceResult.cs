using System.Collections;

namespace Core.Utility.Web.EX
{
    //
    // 摘要:
    //     Provides the processed data after paging, sorting, filtering and grouping are
    //     applied.
    public class DataSourceResult
    {
        //public DataSourceResult();

        //
        // 摘要:
        //     The result of the data aggregation.
        //public IEnumerable<AggregateResult> AggregateResults { get; set; }
        //
        // 摘要:
        //     The data collection.
        public IEnumerable Data { get; set; }
        //
        // 摘要:
        //     A list of errors that occurred during the creation of the data result.
        public object Errors { get; set; }
        //
        // 摘要:
        //     The total number of data entries.
        public int Total { get; set; }
    }
}
