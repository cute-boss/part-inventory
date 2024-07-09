/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* FilterConfig.cs                                                                                      */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPS.Filters;
using System.Web.Mvc;

namespace SPS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new NoCache());
        }
    }
}