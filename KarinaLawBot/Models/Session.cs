using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarinaLawBot.Models
{
    public class Session
    {
        public string LanguageCode { get; set; } = "ru";
        public string CurrentMenu { get; set; } = "main";
        public Stack<string> MenuHistory { get; } = new Stack<string>();
    }
}
