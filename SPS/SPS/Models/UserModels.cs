/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* UserModels.cs                                                                                        */
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
    public class UserModels
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(20, ErrorMessage = "{0} cannot be more than {1} characters")]
        [RegularExpression(@"^([a-zA-Z0-9]+)$", ErrorMessage = "{0} is invalid")]   //only accept character and number
        [Display(Name = "Username")]
        public string UserName { get; set; }

        public int RoleId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Role")]
        public string RoleName { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        //public int StaffId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(12, ErrorMessage = "{0} must be between {2}-{1} characters long", MinimumLength = 6)]
        [RegularExpression(@"^([a-zA-Z0-9]+)$", ErrorMessage = "{0} is invalid")]   //only accept character and number
        [Display(Name = "Staff No.")]
        public string StaffNo { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(80, ErrorMessage = "{0} cannot be more than {1} characters")]
        [Display(Name = "Name")]
        public string StaffName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "{0} is invalid")]
        [StringLength(100, ErrorMessage = "{0} cannot be more than {1} characters")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string OldRoleName { get; set; }
        public string OldStaffNo { get; set; }
        public string OldEmail { get; set; }
    }

    public class UserDetailModels
    {
        public UserModels UserModel { get; set; }
    }

    public class UserEditModels
    {
        public UserModels UserModel { get; set; }

        public SelectList RoleList { get; set; }
    }

    public class RoleModels
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}