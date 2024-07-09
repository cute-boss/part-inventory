/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* EnumEx.cs                                                                                            */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*      14/02/2023  Add STATUS_TRANSFER to RecordStatus                             Azmir               */
/*      09/03/2023  Add UserRoles                                                   Azmir               */
/*      07/08/2023  Add EVTLOG_DB_BACKUP to LogType                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

namespace SPSLib
{
    public class EnumEx
    {
        public enum EmailProtocol
        {
            NONE = 0,               // None
            TLSSSL,                 // TLS/SSL
        };

        public enum LogType
        {
            LOGIN = 1,              // Login
            LOGOUT,                 // Logout
            RESET_PWD,              // Reset Password
            CHG_PWD,                // Change Password
            MAIN_RECORD,            // Record
            SYS_USER,               // User
            SYS_MISC,               // Miscellaneous
            SYS_BUILDING,           // Building
            SYS_RACK,               // Rack
            SYS_PART,               // Part
            SYS_PART_RACK,          // Part Rack
            BATCH_UPLOAD,           // Add Batch Upload

            EVTLOG_SVR = 200,       // Event log
            EVTLOG_REMOVE_HISTORY,  // Remove history
            EVTLOG_NOTIFICATION,    // Send notification
            EVTLOG_DB_BACKUP,       // Backup Database
        }

        public enum RecordStatus
        {
            STATUS_IN = 1,          // In record 
            STATUS_OUT,             // Out record
            STATUS_TRANSFER,        // Transfer record
        };

        public enum UserRoles
        {
            ROLES_ADMIN = 1,        // Admin role 
            ROLES_USER,             // User role
        };
    }
}
