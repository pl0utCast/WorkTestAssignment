using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WorkTestAssignment
{
    /// <summary>
    /// Класс, представляющий данные пользователя, такие как ФИО, телефон и паспортные данные.
    /// </summary>
    public class InputData
    {
        [JsonPropertyName("FIO")]
        public string FIO { get; set; }

        [JsonPropertyName("Phone")]
        public string Phone { get; set; }

        [JsonPropertyName("Passport")]
        public string Passport { get; set; }

        public InputData(string fIO, string phone, string passport)
        {
            FIO = fIO;
            Phone = phone;
            Passport = passport;
        }
    }
}
