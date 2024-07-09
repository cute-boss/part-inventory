/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* RecordModels.cs                                                                                      */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Add RecordImportModels                                          Azmir               */
/*      01/06/2022  Add RackList                                                    Azmir               */
/*      09/03/2023  Add RecordTransferModel for transfer func                       Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SPS.Models
{
    public class RecordTypeModels
    {
        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Record Type")]
        public int RecordStatus { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Rack Code")]
        public string RackCode { get; set; }

        public SelectList RackList { get; set; }
    }

    public class RecordInOutModels
    {
        public RecordTypeModels RecordTypeModel { get; set; }
        public int RecordId { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date/Time")]
        public DateTime? RecordDateTime { get; set; }

        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "{0} only accept digits")]     // only accept number
        [Display(Name = "Record Quantity")]
        public int RecordQty { get; set; }

        [Display(Name = "Record Status")]
        public int RecordStatus { get; set; }

        [Display(Name = "Recorded By")]
        public string RecordBy { get; set; }

        [StringLength(200, ErrorMessage = "{0} cannot be more than {1} characters")]
        [Display(Name = "Record Remark")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string RecordRemark { get; set; }

        public int RackId { get; set; }

        [Display(Name = "Rack Code")]
        public string RackCode { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int PartId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Part")]
        public string PartCode { get; set; }

        [Display(Name = "Balance Quantity")]
        public int PartQty { get; set; }

        [Display(Name = "Part Image")]
        public string PartImage { get; set; }

        public string PartGUIDFileName { get; set; }

        public bool IsPartRackExist { get; set; }

        public SelectList PartRackList { get; set; }

        // Initialize model that is not in the view
        public RecordInOutModels()
        {
            RecordTypeModel = new RecordTypeModels();
        }
    }

    public class RecordImportModels
    {
        [Display(Name = "File Template")]
        public string TemplateFile { get; set; }
        [Display(Name = "File Import")]
        public string ImportFile { get; set; }
        public BuildingModels BuildingModel { get; set; }
        public RackModels RackModel { get; set; }
        public PartModels PartModel { get; set; }
        public RecordInOutModels RecordInOutModel { get; set; }
        public ImportErrorModel ImportErrorModel { get; set; }
        public IList<ImportErrorModel> ImportErrorListModel { get; set; }
        public int InputRow { get; set; }
    }

    public class ImportErrorModel
    {
        [Display(Name = "Row")]
        public int ErrorRow { get; set; }

        [Display(Name = "Description")]
        public string Error { get; set; }
    }

    public class RecordTransferModels
    {
        public RecordTypeModels RecordTypeModel { get; set; }
        public int RecordId { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date/Time")]
        public DateTime? RecordDateTime { get; set; }

        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "{0} only accept digits")]     // only accept number
        [Display(Name = "Transfer Quantity")]
        public int RecordQty { get; set; }

        [Display(Name = "Record Status")]
        public int RecordStatus { get; set; }

        [Display(Name = "Transferred By")]
        public string RecordBy { get; set; }

        [StringLength(200, ErrorMessage = "{0} cannot be more than {1} characters")]
        [Display(Name = "Transfer Remark")]
        [Required(ErrorMessage = "{0} is required")]
        public string RecordRemark { get; set; }

        public int RackId { get; set; }

        [Display(Name = "Rack Code (Current)")]
        public string RackCode { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int PartId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Part")]
        public string PartCode { get; set; }

        [Display(Name = "Balance Quantity")]
        public int PartQty { get; set; }

        public int OldPartQty { get; set; }

        public int TtlPartQty { get; set; }

        [Display(Name = "Part Image")]
        public string PartImage { get; set; }

        public string PartGUIDFileName { get; set; }

        public bool IsPartRackExist { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int NewRackId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Rack Code (New Location)")]
        public string NewRackCode { get; set; }

        public int LastSavedQty { get; set; }

        public bool RecordTransfer { get; set; }

        public bool RecordTransferPartiallyExists { get; set; }

        public bool RecordTransferFullExists { get; set; }

        public SelectList PartRackList { get; set; }

        public SelectList RackList { get; set; }

        // Initialize model that is not in the view
        public RecordTransferModels()
        {
            RecordTypeModel = new RecordTypeModels();
        }
    }
}