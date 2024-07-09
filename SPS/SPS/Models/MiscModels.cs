/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* MiscModels.cs                                                                                        */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SPS.Models
{
    public class MiscModels
    {
        [Display(Name = "Enable Email")]
        public bool UseEmail { get; set; }

        [StringLength(50, ErrorMessage = "{0} cannot be more than {1} characters")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "SMTP Server")]
        public string EmailSmtp { get; set; }

        [StringLength(5, ErrorMessage = "{0} cannot be more than {1} characters")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Port")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "{0} only accept digits")]
        public string EmailPort { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Protocol")]
        public int EmailProtocol { get; set; }

        [StringLength(50, ErrorMessage = "{0} cannot be more than {1} characters")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Username")]
        public string EmailUsername { get; set; }

        [StringLength(100, ErrorMessage = "{0} cannot be more than {1} characters")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Password")]
        public string EmailPassword { get; set; }

        [Display(Name = "Data Retention Period (Year)")]
        public int RetentionPeriod { get; set; }

        [Display(Name = "Max. Attachment Size (MB)")]
        public int AttachmentSize { get; set; }

        [Display(Name = "Auto Logout Time (Min)")]
        public int IdleTime { get; set; }

        [Display(Name = "Password Reset Validation Period (Min)")]
        public int TokenResetTime { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "{0} is invalid")]
        [StringLength(50, ErrorMessage = "{0} cannot be more than {1} characters")]
        [Display(Name = "SPS Default Email")]
        public string DefaultEmail { get; set; }

        public SelectList EmailProtocolList { get; set; }
    }
}