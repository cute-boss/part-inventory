/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* BundleConfig.cs                                                                                      */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      09/03/2023  Add ekko-lightbox                                               Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System.Web.Optimization;

namespace SPS
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            // When using Bundle, do not append the .min eg: adminlte.min.css
            bundles.Add(new StyleBundle("~/content/css").Include(
                        "~/Content/sb-admin-2/sb-admin-2.css",
                        "~/Content/fontawesome-free/css/all.css",
                        "~/Content/select2/select2.css",
                        "~/Content/select2/select2-bootstrap4.css",
                        "~/Content/sweetalert2/sweetalert2.css",
                        "~/Content/tempusdominus/tempusdominus-bootstrap-4.css",
                        "~/Content/toastr/toastr.css",
                        "~/Content/dropzone/dropzone.css",
                        "~/Content/custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/script").Include(
                        "~/Scripts/bootstrap/bootstrap.bundle.js",
                        "~/Scripts/jquery-easing/jquery.easing.js",
                        "~/Scripts/sb-admin-2/sb-admin-2.js",
                        "~/Scripts/fontawesome-free/all.js",
                        "~/Scripts/moment/moment.js",
                        "~/Scripts/select2/select2.js",
                        "~/Scripts/sweetalert2/sweetalert2.js",
                        "~/Scripts/tempusdominus/tempusdominus-bootstrap-4.js",
                        "~/Scripts/dropzone/dropzone.js",
                        "~/Scripts/toastr/toastr.js",
                        "~/Scripts/jszip.js",
                        "~/Scripts/custom.js"));

            #region datatable
            bundles.Add(new StyleBundle("~/content/datatable/css").Include(
                        "~/Content/DataTables/datatables-bs4/dataTables.bootstrap4.css",
                        "~/Content/DataTables/datatables-responsive/responsive.bootstrap4.css",
                        "~/Content/DataTables/datatables-buttons/buttons.bootstrap4.css"));

            bundles.Add(new ScriptBundle("~/bundles/datatable/script").Include(
                        "~/Scripts/DataTables/datatables/jquery.dataTables.js",
                        "~/Scripts/DataTables/datatables-bs4/dataTables.bootstrap4.js",
                        "~/Scripts/DataTables/datatables-responsive/dataTables.responsive.js",
                        "~/Scripts/DataTables/datatables-responsive/responsive.bootstrap4.js",
                        "~/Scripts/DataTables/datatables-buttons/dataTables.buttons.js",
                        "~/Scripts/DataTables/datatables-buttons/buttons.bootstrap4.js",
                        "~/Scripts/DataTables/datatables-buttons/buttons.colVis.js",
                        "~/Scripts/DataTables/datetime-moment.js",
                        "~/Scripts/DataTables/datatables-buttons/buttons.html5.js",
                        "~/Scripts/DataTables/datatables-buttons/buttons.print.js"));
            #endregion

            #region ekko-lightbox
            bundles.Add(new StyleBundle("~/bundles/lightbox/css").Include(
                "~/Content/ekko-lightbox/ekko-lightbox.css"));

            bundles.Add(new ScriptBundle("~/bundles/lightbox/script").Include(
               "~/Scripts/ekko-lightbox/ekko-lightbox.js"));
            #endregion

            #if DEBUG
            BundleTable.EnableOptimizations = false;
            #else
                BundleTable.EnableOptimizations = true;
            #endif
        }
    }
}