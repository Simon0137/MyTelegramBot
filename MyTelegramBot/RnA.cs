using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTelegramBot
{
    public class RnA
    {
        public string Request { get; set; }
        public string? Answer { get; set; }

        public RnA(string request, string? answer)
        {
            this.Request = request;
            this.Answer = answer;
        }
    }
}
