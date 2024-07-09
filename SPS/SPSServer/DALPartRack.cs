/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* DALPartRack.cs                                                                                       */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      03/03/2023  Initial version                                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using SPSLib;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SPSServer
{
    class DALPartRack
    {
        /// <summary>
        /// Get part rack below min qty
        /// </summary>
        /// <param name="tblResult">Output result</param>
        /// <returns></returns>
        public static bool GetPartRackBelowMinQtyList(out DataTable tblResult)
        {
            tblResult = new DataTable();
            string sCon = Common.GetDBStrCon();

            if (sCon == null)
            {
                return false;
            }

            using (SqlConnection sqlcon = new SqlConnection(sCon))
            {
                try
                {
                    sqlcon.Open();

                    SqlCommand sqlcmd = new SqlCommand("usp_GetPartRackBelowMinQtyList", sqlcon);
                    sqlcmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter sqlda = new SqlDataAdapter(sqlcmd);
                    sqlda.Fill(tblResult);
                }

                catch (Exception ex)
                {
                    Common.WriteToLog(ex.ToString());
                    return false;
                }
            }

            return true;
        }
    }
}
