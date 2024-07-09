/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* CommonMsg.cs                                                                                         */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      28/03/2022  Add UI msg                                                      Azmir               */
/*      09/03/2023  Add msg for PartMinQty and transfer                             Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

using System.Windows.Forms;

namespace SPSLib
{
    public class CommonMsg
    {
        /*---------- Web Message ----------*/
        public const string SQL_ERR = "SQL error.";
        public const string INVALID_ID = "ID is invalid.";

        #region AccountController
        public const string INVALID_USRNAME_PWD = "Username or password is incorrect.";
        public const string INVALID_USRNAME_EMAIL = "Username or email is incorrect.";
        public const string FAIL_SEND_EMAIL = "Failed to send email.";
        public const string SENT_TOKEN = "The password token has been sent to your email.";
        public const string INVALID_TOKEN = "The password token is invalid.";
        public const string TOKEN_EXPIRY = "The password token has expired. Please request again.";
        public const string PWD_RESET = "The password has been reset. Please login using new password.";
        public const string SESSION_EXPIRED = "Your login session has expired.";
        public const string DUPLICATE_LOGIN = "Duplicate login has been detected. Your login session has been ended.";
        public const string PWD_CHANGE = "The password has been changed.";
        public const string INVALID_OLD_PWD = "Current password is incorrect.";
        #endregion

        #region UserController
        public const string DUP_USRNAME = "Username has already been used.";
        public const string DUP_STAFFNO = "Staff no. has already been used.";
        public const string DUP_EMAIL = "Email has already been used.";
        public const string SUCC_REG_USER = "User has been created successfully.";
        public const string FAIL_REG_USER = "Failed to create user.";
        public const string SUCC_UPD_USER = "User has been updated successfully.";
        public const string FAIL_UPD_USER = "Failed to update user.";
        public const string INVALID_MAX_USER = "No. of login user has exceed maximum limit. Please try again later.";
        #endregion

        #region MiscController
        public const string SUCC_UPD_MISC = "Miscellaneous has been updated successfully.";
        public const string FAIL_UPD_MISC = "Failed to update miscellaneous.";
        #endregion

        #region LogController
        public const string INVALID_DATETIME = "Invalid date.";
        #endregion

        #region BuildingController
        public const string DUP_BUILDING = "Building has already been used.";
        public const string SUCC_REG_BUILDING = "Building has been created successfully.";
        public const string FAIL_REG_BUILDING = "Failed to create building.";
        public const string SUCC_UPD_BUILDING = "Building has been updated successfully.";
        public const string FAIL_UPD_BUILDING = "Failed to update building.";
        public const string INV_BUILDING = "Invalid building. Please try again.";

        public const string EMPTY_BUILDING_NAME = "Building name is empty.";
        #endregion

        #region RackController
        public const string DUP_RACK = "Rack has already been used.";
        public const string SUCC_REG_RACK = "Rack has been created successfully.";
        public const string FAIL_REG_RACK = "Failed to create rack.";
        public const string SUCC_UPD_RACK = "Rack has been updated successfully.";
        public const string FAIL_UPD_RACK = "Failed to update rack.";
        public const string INV_RACK = "Invalid rack. Please try again.";

        public const string EMPTY_RACK_NAME = "Rack name is empty.";
        public const string EMPTY_RACK_CODE = "Rack code is empty.";
        #endregion

        #region PartController
        public const string DUP_PART = "Part has already been used.";
        public const string SUCC_REG_PART = "Part has been created successfully.";
        public const string FAIL_REG_PART = "Failed to create part.";
        public const string SUCC_UPD_PART = "Part has been updated successfully.";
        public const string FAIL_UPD_PART = "Failed to update part.";
        public const string INV_PART_NO = "Invalid part code. Please try again.";
        public const string INV_PART_MIN_QTY = "Invalid minimum quantity. Please try again.";

        public const string MAX_PART_FILE_SIZE = "File is too large. Please upload file below than ";
        public const string INV_PART_FILE_FORMAT = "Invalid file format. Please select .jpg, .png, .jpeg format only.";

        public const string EMPTY_PART_NAME = "Part name is empty.";
        public const string EMPTY_PART_CODE = "Part code is empty.";
        #endregion

        #region RecordController
        public const string PROCEED_RECORD_TYPE = "Part type has been recorded successfully.";
        public const string INV_RECORD_QTY = "Invalid record quantity. The value must be more than 0.";
        public const string SUCC_REG_RECORD = "Part has been recorded successfully.";
        public const string FAIL_REG_RECORD = "Failed to record part.";
        public const string NEGATIVE_UPD_RECORD = "Record quantity has exceed balance quantity. Please try again.";

        public const string NEGATIVE_UPD_TRANSFER = "Transfer quantity has exceed balance quantity. Please try again.";
        public const string INV_RACK_TRANSFER = "Unable to tansfer part at the same rack. Please try again.";
        public const string SUCC_TRANSFER = "Part has been transferred successfully.";
        public const string FAIL_TRANSFER = "Failed to transfer part.";

        public const string INV_COL_RECORD_FILE = "Invalid column. CSV file must have the same column as the database data.";
        public const string NOT_FOUND_RECORD_FILE = "File input is empty.";
        public const string INV_PART_QTY = "Invalid quantity. The value must be in integer format and is more than 0.";
        public const string SUCC_IMPORT_RECORD = "File has been imported successfully.";
        public const string FAIL_IMPORT_RECORD = "Failed to import file.";
        #endregion

        /*---------- Server Message ----------*/
        #region Server
        /// <summary>
        /// SQL error msg
        /// </summary>
        public static void SQLErrorMsg()
        {
            MessageBox.Show("SQL Server error.", "Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Duplicate server instance msg
        /// </summary>
        public static void DupServerMsg()
        {
            MessageBox.Show("SPS Server has already started.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Exit server confirmation msg
        /// </summary>
        /// <returns></returns>
        public static DialogResult ExitConfirmationMsg()
        {
            DialogResult result = MessageBox.Show("Are you sure you want to stop SPS Server?", "Confirmation",
               MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            return result;
        }
        #endregion
    }
}
