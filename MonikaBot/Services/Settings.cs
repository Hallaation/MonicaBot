using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MonikaBot.Services
{
    [Serializable]
    class Settings
    {
        private string BotToken = "";
        private string BotPrefix = "!";

        //Public read only prop for token
        public string Token { get { return BotToken; } set { BotToken = value; } }
        public string Prefix { get { return BotPrefix; } set { BotPrefix = value; } }


        //overloaded constructor
        public Settings(string aBotToken = "", string aBotPrefix = "!")
        {
            BotToken = aBotToken;
            BotPrefix = aBotPrefix;
        }

        //File I/O
        public void SaveFile(string aFilePath = "./settings.json")
        {
            using (StreamWriter file = new StreamWriter(aFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                Console.WriteLine(BotToken);
                string jsonData = JsonConvert.SerializeObject(this);
                file.WriteLine(jsonData);
                serializer.Serialize(file, typeof(Settings));
            }
        }

        public static Settings LoadFile(string aFilePath = "./settings.json")
        {
            if (File.Exists(aFilePath))
            {
                using (StreamReader file = File.OpenText(aFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer(); //make serializer

                    //read the file until it ends
                    string json = "";

                    json += file.ReadLine(); //read the jsonfile
                    return JsonConvert.DeserializeObject<Settings>(json);
                }
            }
            else
            {
                return null;
            }
        }
    }


}
