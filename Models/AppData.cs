﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaperWork.Models
{
    internal class AppData
    {
        private static string _UserUid;
        public static string UserUid { get { return _UserUid;  } set { _UserUid = value; } }
        private static string _UserPassword;
        public static string UserPassword { get { return _UserPassword; } set { _UserPassword = value; } }
        private static string _UserEmail;
        public static string UserEmail { get { return _UserEmail; } set { _UserEmail = value; } }
        private static string _UserAccoutType;
        public static string UserAccoutType { get { return _UserAccoutType; } set { _UserAccoutType = value; } }
        private static string _UserName;
        public static string UserName { get { return _UserName; } set { _UserName = value; } }
    }
}
