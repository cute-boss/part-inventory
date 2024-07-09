/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* RptLogModels.cs                                                                                      */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SPS.Models
{
    public class RptLogModels
    {
        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Start Date/Time")]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "End Date/Time")]
        public DateTime EndDateTime { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Type")]
        public int LogTypeId { get; set; }

        public SelectList LogTypeList { get; set; }
    }

    public class LogDetailModel
    {
        public int LogId { get; set; }

        public int LogTypeId { get; set; }

        public string LogTypeName { get; set; }

        public string LogDesc { get; set; }

        public string UserName { get; set; }

        public string SLogTime { get; set; }

        public DateTime LogTime { get; set; }
    }
}