using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WorkTestAssignment
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        // Стек для хранения данных типа InputData, передаваемый из главного окна
        private Stack<InputData> stack = new Stack<InputData>();
        public Window1(Stack<InputData> Stack)
        {
            stack = Stack;
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события инициализации текстового поля для отображения данных в обработанном виде.
        /// Этот метод форматирует данные из стека и выводит их в TextBox.
        /// </summary>
        private void TB_Processed_Initialized(object sender, EventArgs e)
        {
                string text = "";

                foreach(InputData inputData in stack)
                {
                    text += $"FIO: {inputData.FIO}\nPhone: {inputData.Phone}\nPassport: {inputData.Passport}\n\n";
                }
                TB_Processed.Text = text;
        }

        /// <summary>
        /// Обработчик события инициализации текстового поля для отображения сырых данных (в JSON-формате).
        /// Этот метод читает файл и выводит его содержимое в TextBox.
        /// </summary>
        private void TB_Raw_Initialized(object sender, EventArgs e)
        {
            using (FileStream fs = new FileStream("inputData.json", FileMode.OpenOrCreate))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                string textFromFile = Encoding.Default.GetString(buffer);
                TB_Raw.Text = textFromFile;
            }
        }
    }
}
