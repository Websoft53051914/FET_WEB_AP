using AutoMapper;
using Core.Utility.Consts;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Utility.Utility
{
    public static partial class CommonUtility
    {

        /// <summary>
        /// 建立imapper
        /// 【Mapper、mapping、AutoMapper】
        /// </summary>
        /// <typeparam name="T1">互相轉換的第一個class</typeparam>
        /// <typeparam name="T2">互相轉換的第二個class</typeparam>
        /// <returns>IMapper</returns>
        public static IMapper CreateMapper<T1, T2>()
        {
            IMapper mapper = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.CreateMap<T1, T2>();
                cfg.CreateMap<T2, T1>();

            }).CreateMapper();

            return mapper;
        }
    }
}
