/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* BuildingModels.cs                                                                                    */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System.ComponentModel.DataAnnotations;

namespace SPS.Models
{
    public class BuildingModels
    {
        public int BuildingId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(30, ErrorMessage = "{0} cannot be more than {1} characters")]
        [Display(Name = "Building Name")]
        public string BuildingName { get; set; }

        public string OldBuildingName { get; set; }
    }
}