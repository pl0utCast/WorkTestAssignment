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
        //Стек для хранения данных, который будет сериализован и десериализован в/из файла
        private Stack<InputData> stack;

        //Предыдущее корректное значение для ФИО
        private string previousText = "";

        //Предыдущее корректное значение для кода страны
        private string previousNum = "";

        //Шаблон для недопустимых символов
        private string specialChars = @"[!""#$%&'()*+,\-./:;<=>?@[\\\]^_`{|}~]";

        //Регулярное выражение проверки корректности ФИО
        private static readonly Regex rgx = new Regex(@"^[\p{L}]+$");

        //Проверка на соответствие регулярному выражению
        private static bool IsTextAllowed(string text)
        {
            return rgx.IsMatch(text);
        }

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
            if (MTB_Passport.Text == "____ ______" && MTB_Phone.Text == "(___)___-__-__" && TB_FIO.Text == "")
            {
                System.Windows.MessageBox.Show("Для сохранения записи заполните поля выше");
                return;
            }

            bool error = false;
            string errorText = "";


            if (TB_FIO.Text == "")
            {
                errorText = "Поле 'ФИО' не должно быть пустым";
                error = true;
            }

            if (TB_PhoneCode.Text.Equals(""))
            {
                errorText += "\nТелефонный код страны не должен быть пустым";
                error = true;
            }

            if (MTB_Phone.Text.Contains("_"))
            {
                errorText += "\nПоле 'Телефон' должно быть заполнено целиком";
                error = true;
            }

            if (MTB_Passport.Text.Contains("_"))
            {
                errorText += "\nПоле 'Паспорт (Серия и номер)' должно быть заполнено целиком";
                error = true;
            }

            if (error)
            {
                System.Windows.MessageBox.Show(errorText);
                return;
            }

            int phoneCode = Int32.Parse(TB_PhoneCode.Text);

            string phoneCodeStr = phoneCode.ToString();

            string phoneNumber = "+" + phoneCodeStr + MTB_Phone.GetLineText(0);

            using (FileStream fs = new FileStream("inputData.json", FileMode.Open))
            {
                InputData inputData = new InputData(TB_FIO.GetLineText(0), phoneNumber, MTB_Passport.GetLineText(0));
                //Проверка на дублирование
                error = false;
                errorText = "";
                foreach(InputData data in stack)
                {
                    if (data.Phone == phoneNumber)
                    {
                        error = true;
                        errorText = "Запись с таким номером телефона уже существует";
                    }
                    if (data.Passport == inputData.Passport)
                    {
                        error = true;
                        errorText += "\nЗапись с такими паспортными данными уже существует";
                    }
                }

                if (error)
                {
                    System.Windows.MessageBox.Show(errorText);
                    return;
                }

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

        /// <summary>
        /// Обработчик события ввода текста в текстовое поле для имени и фамилии,
        /// Запрещает ввод символов, если они не являются буквенными.
        /// </summary>
        private void TB_FIO_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        /// <summary>
        /// Обработчик события изменения текста в текстовом поле для имени и фамилии.
        /// Проверяет, содержит ли текст цифры или специальные символы. 
        /// Если содержатся, возвращает текст к предыдущему значению.
        /// </summary>
        private void TB_FIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            //Проверяем, содержит ли текст цифры
            if (Regex.IsMatch(textBox.Text, @"\d+") || Regex.IsMatch(textBox.Text, specialChars))
            {
                //Если содержится, возвращаем текст к предыдущему значению
                textBox.Text = previousText;
                //Устанавливаем курсор в конец текста
                textBox.SelectionStart = textBox.Text.Length;
            }
            else
            {
                //Если нет, сохраняем текущее значение как предыдущее
                previousText = textBox.Text;
            }

        }

        /// <summary>
        /// Обработчик события ввода текста в текстовое поле для кода телефона.
        /// Запрещает ввод символов, если они не являются цифрами.
        /// </summary>
        private void TB_PhoneCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }

        /// <summary>
        /// Обработчик события изменения текста в текстовом поле для кода телефона.
        /// Проверяет, содержит ли текст буквы, пробелы или специальные символы. 
        /// Если содержатся, возвращает текст к предыдущему значению.
        /// </summary>
        private void TB_PhoneCode_TextChanged(object sender, TextChangedEventArgs e)
        {
           var textBox = sender as TextBox;

            // Проверяем, содержит ли текст буквы или пробелы или спецсимволы
            if (Regex.IsMatch(textBox.Text, @"\p{L}") || Regex.IsMatch(textBox.Text, @"\s") || Regex.IsMatch(textBox.Text, specialChars))
            {
                // Если содержится, возвращаем текст к предыдущему значению
                textBox.Text = previousNum;
                // Устанавливаем курсор в конец текста
                textBox.SelectionStart = textBox.Text.Length;
            }
            else
            {
                // Если нет, сохраняем текущее значение как предыдущее
                previousNum = textBox.Text;
            }
        }

        /// <summary>
        /// Обработчик события нажатия клавиш в текстовом поле для кода телефона,
        /// Запрещает ввод пробела.
        /// </summary>
        private void TB_PhoneCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Обработчик события потери фокуса в текстовом поле для имени и фамилии,
        /// Проверяет, пустое ли поле. Если пустое, отображает сообщение об ошибке.
        /// </summary>
        private void TB_FIO_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TB_FIO.Text.Equals(""))
            {
                Label_ErrorFIO.Visibility = Visibility.Visible;
                Label_ErrorFIO.Content = "Поле не должно быть пустым";
            }
            else
            {
                Label_ErrorFIO.Visibility = Visibility.Hidden;
                Label_ErrorFIO.Content = string.Empty;
            }
        }

        /// <summary>
        /// Обработчик события потери фокуса в текстовом поле для кода телефона,
        /// Проверяет, пустое ли поле. Если пустое, отображает сообщение об ошибке.
        /// </summary>
        private void TB_PhoneCode_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TB_PhoneCode.Text.Equals(""))
            {
                Label_ErrorPhone.Visibility = Visibility.Visible;
                Label_ErrorPhone.Content = "Код страны не должен быть пустым";
            }
            else
            {
                Label_ErrorPhone.Visibility = Visibility.Hidden;
                Label_ErrorPhone.Content = string.Empty;
            }
        }

        /// <summary>
        /// Обработчик события потери фокуса в маске ввода телефона,
        /// Проверяет, заполнено ли поле полностью. Если нет, отображает сообщение об ошибке.
        /// </summary>
        private void MTB_Phone_LostFocus(object sender, RoutedEventArgs e)
        {
            if (MTB_Phone.Text.Contains("_"))
            {
                Label_ErrorPhone.Visibility = Visibility.Visible;
                Label_ErrorPhone.Content = "Поле должно быть заполнено целиком";
            }
            else
            {
                Label_ErrorPhone.Visibility = Visibility.Hidden;
                Label_ErrorPhone.Content = string.Empty;
            }
        }

        /// <summary>
        /// Обработчик события потери фокуса в маске ввода паспорта,
        /// Проверяет, заполнено ли поле полностью. Если нет, отображает сообщение об ошибке.
        /// </summary>
        private void MTB_Passport_LostFocus(object sender, RoutedEventArgs e)
        {
            if (MTB_Passport.Text.Contains("_"))
            {
                Label_ErrorPassport.Visibility = Visibility.Visible;
                Label_ErrorPassport.Content = "Поле должно быть заполнено целиком";
            }
            else
            {
                Label_ErrorPassport.Visibility = Visibility.Hidden;
                Label_ErrorPassport.Content = string.Empty;
            }
        }
    }
}