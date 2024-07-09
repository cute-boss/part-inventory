/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* PartModels.cs                                                                                        */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Del PartImportModels                                            Azmir               */
/*      25/07/2022  Add BalanceQty                                                  Azmir               */
/*      13/02/2023  Add PartMinQty                                                  Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SPS.Models
{
    public class PartModels
    {
        public int PartId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(30, ErrorMessage = "{0} cannot be more than {1} characters")]
        [Display(Name = "Part Code")]
        public string PartCode { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, ErrorMessage = "{0} cannot be more than {1} characters")]
        [Display(Name = "Part Name")]
        public string PartName { get; set; }

        [StringLength(200, ErrorMessage = "{0} cannot be more than {1} characters")]
        [Display(Name = "Description")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PartDesc { get; set; }

        [Display(Name = "Upload File")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PartFileName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PartGUIDFileName { get; set; }

        public Guid Guid { get; set; }

        public PartModels()
        {
            Guid = Guid.NewGuid();
        }

        public string OldPartCode { get; set; }
        public string OldPartFileName { get; set; }
        public string OldPartGUIDFileName { get; set; }

        [Display(Name = "Balance Quantity")]
        public int BalanceQty { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Minimum Quantity")]
        public int PartMinQty { get; set; }
    }
}