/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* RptInventoryModels.cs                                                                                */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      20/05/2022  Initial version                                                 Azmir               */
/*      01/06/2022  Add parameter for rack and building                             Azmir               */
/*      09/03/2023  Add PartMinQty to RptInventoryModels                            Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SPS.Models
{
    public class RptInventoryModels
    {
        [Display(Name = "Search By")]
        public bool SearchCategory { get; set; }

        public int RecordId { get; set; }
        public DateTime? RecordDateTime { get; set; }
        public string SRecordDateTime { get; set; }
        public int RecordQty { get; set; }
        public int RecordStatus { get; set; }
        public string RecordBy { get; set; }
        public string RecordRemark { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Building")]
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }

        [Display(Name = "All Building")]
        public bool AllBuilding { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Rack")]
        public int RackId { get; set; }
        public string RackCode { get; set; }
        public string RackName { get; set; }

        [Display(Name = "All Rack")]
        public bool AllRack { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Part")]
        public int PartId { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string PartDesc { get; set; }
        public int PartMinQty { get; set; }
        public int PartQty { get; set; }

        [Display(Name = "All Part")]
        public bool AllPart { get; set; }

        public SelectList BuildingList { get; set; }
        public SelectList RackList { get; set; }
        public SelectList PartList { get; set; }
        public SelectList PartSearchList { get; set; }
        public RecordInOutModels RecordInOutModel { get; set; }
    }
}