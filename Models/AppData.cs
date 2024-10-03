using System;
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
        public static string CurrentFolder { get { return _UserName; } set { _UserName = value; } }
        private static byte[] _Salt;
        public static byte [] Salt { get { return _Salt; } set { _Salt = value; } }
      
        private static byte[] _Key;
        public static byte[] Key { get { return _Key; } set { _Key = value; } }
        private static string _ServerResult;
        public static string ServerResult { get { return _ServerResult; } set { _ServerResult = value; } }
    }
   
}
