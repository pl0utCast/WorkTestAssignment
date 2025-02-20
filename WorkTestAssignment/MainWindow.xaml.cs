using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Xceed.Wpf.Toolkit;
using System.IO;
using System.Text.Json;
using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WorkTestAssignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Стек для хранения данных, который будет сериализован и десериализован в/из файла
        private Stack<InputData> stack;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события нажатия кнопки для открытия нового окна (Window1)
        /// В новом окне будут отображаться сохраненные ранее записи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1(stack);
            window1.ShowDialog();
        }

        /// <summary>
        /// Обработчик события нажатия кнопки для сохранения введенных данных в стек и файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            using (FileStream fs = new FileStream("inputData.json", FileMode.Open))
            {
                InputData inputData = new InputData(TB_FIO.GetLineText(0), MTB_Phone.GetLineText(0), MTB_Passport.GetLineText(0));
                stack.Push(inputData);
                JsonSerializer.Serialize<Stack<InputData>>(fs, stack);
                System.Windows.MessageBox.Show("Сохранение прошло успешно");
            }

        }

        /// <summary>
        /// Обработчик события для инициализации окна, при котором данные из файла десериализуются в стек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            stack = new Stack<InputData>();
            using (FileStream fs = new FileStream("inputData.json", FileMode.OpenOrCreate))
            {
                if (fs.Length> 0)
                {
                    stack = JsonSerializer.Deserialize<Stack<InputData>>(fs);
                }
            }
        }

        /// <summary>
        /// Обработчик события для кнопки, очищающей файл и стек данных.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {    
            File.WriteAllText("inputData.json", string.Empty);
            stack.Clear();
            System.Windows.MessageBox.Show("Файл был успешно очищен");
        }
    }


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