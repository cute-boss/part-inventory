/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* RackModels.cs                                                                                        */
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
    public class RackModels
    {
        public int RackId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(30, ErrorMessage = "{0} cannot be more than {1} characters")]
        [Display(Name = "Rack Name")]
        public string RackName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(20, ErrorMessage = "{0} cannot be more than {1} characters")]
        [Display(Name = "Rack Code")]
        public string RackCode { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Building")]
        public int BuildingId { get; set; }

        public string BuildingName { get; set; }

        public string OldRackName { get; set; }

        public string OldRackCode { get; set; }

        public SelectList BuildingList { get; set; }
    }
}