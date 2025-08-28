
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.DB
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// 儲存所有異動。
        /// </summary>
        void Commit();
    }
}
