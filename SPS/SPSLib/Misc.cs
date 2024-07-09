/*------------------------------------------------------------------------------------------------------*/
/* Copyright (C) Panasonic System Networks Malaysia Sdn. Bhd.                                           */
/* Misc.cs                                                                                   */
/*                                                                                                      */
/* Modify History:							                                                            */
/* 		Date        Comment			                                                Name	            */
/*      ---------------------------------------------------------------------------------------------   */
/*      12/10/2021  Initial version                                                 Azmir               */
/*                                                                                                      */
/*------------------------------------------------------------------------------------------------------*/

namespace SPSLib
{
    public class Misc
    {
        public Misc() { }

        public bool UseEmail;
        public string EmailSmtp;
        public string EmailPort;
        public int EmailProtocol;
        public string EmailUsername;
        public string EmailPassword;
        public int RetentionPeriod;
        public int AttachmentSize;
        public int IdleTime;
        public int TokenResetTime;
        public string DefaultEmail;
    }
}
