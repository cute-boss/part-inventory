/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* RptInOutModels.cs                                                                                    */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      01/06/2022  Add parameter for rack and building                             Azmir               */
/*      09/03/2023  Add TransferType to RptInOutModels                              Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SPS.Models
{
    public class RptInOutModels
    {
        [Display(Name = "Record Type")]
        public bool InType { get; set; }

        public bool OutType { get; set; }

        public bool TransferType { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "From")]
        public DateTime FromDT { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "To")]
        public DateTime ToDT { get; set; }

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

        [Display(Name = "All Part")]
        public bool AllPart { get; set; }

        public SelectList BuildingList { get; set; }
        public SelectList RackList { get; set; }
        public SelectList PartList { get; set; }
        public SelectList PartSearchList { get; set; }
        public RecordInOutModels RecordInOutModel { get; set; }
    }
}