using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MonikaBot.Extensions
{
    [Serializable]
    class Settings
    {
        private string BotToken = "";
        private string prefix = "!";

        //Public read only prop for token
        public string Token { get { return BotToken; } set { BotToken = value; } }
        public string Prefix { get { return prefix; } set { prefix = value; } }

    }


}
