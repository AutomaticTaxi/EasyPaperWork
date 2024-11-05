namespace EasyPaperWork.Models
{
    public class Log
    {
        private DateTime _dateTime;
        public DateTime? dateTime { get; set; }

        private string _menssage;
        public string menssage { get { return _menssage; } set { _menssage = value; } }
        public Log(string filename, int option)
        {
            switch (option)
            {
                case '1':
                    _menssage = string.Concat("Documento adicionado. Nome = ", filename);
                    dateTime = DateTime.UtcNow;
                    break;
                case '2':
                    _menssage = string.Concat("Documento Removido. Nome = ", filename);
                    _dateTime = DateTime.UtcNow;
                    break;
                case '3':
                    _menssage = string.Concat("Documento Atualizado. Nome =  ", filename);
                    _dateTime = DateTime.UtcNow;
                    break;
                case '4':
                    _menssage = string.Concat("Documento Baixado. Nome =  ", filename);
                    _dateTime = DateTime.UtcNow;
                    break;

            }
        }
        public Log() { }
    }
}
