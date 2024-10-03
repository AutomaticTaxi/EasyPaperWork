using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaperWork.Models
{
    public class Log
    {
        private DateTime _dateTime;
        public DateTime dateTime { get { return _dateTime; } set { _dateTime = value; } }
        
        private string _menssage;
        public string menssage { get { return _menssage; } set { _menssage = value; } }
        public Log CreateLogAddFile(string filename)
        {
            return new Log { menssage = string.Concat("Documento adicionado. Nome = ",filename), dateTime = DateTime.UtcNow };

        }
        public Log CreateLogDeleteFile(string filename)
        {
            return new Log { menssage = string.Concat("Documento Removido. Nome = ", filename), dateTime = DateTime.UtcNow };

        }
        public Log CreateLogUpdateFile(string filename)
        {
            return new Log { menssage = string.Concat("Documento Atualizado. Nome = ", filename), dateTime = DateTime.UtcNow };

        }
        public Log CreateLogDownloadFile(string filename)
        {
            return new Log { menssage = string.Concat("Documento Baixado. Nome = ", filename), dateTime = DateTime.UtcNow };

        }
    }
}
