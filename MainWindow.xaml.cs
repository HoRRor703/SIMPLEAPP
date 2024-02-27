using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using Button = System.Windows.Controls.Button;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel.Design;





namespace SIMPLEAPP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            
            InitializeComponent();
            string connectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                @"Data Source = C:\Users\gg\Downloads\Documents\WPF\Отдел кадров Колледжа.accdb;" +
                "User Id=Admin;Password=;";
            OleDbConnection connection = new OleDbConnection(connectionString);


            //PHOTO1

            




            connection.Open();
            OleDbCommand command = new OleDbCommand("Select * From Сотрудники", connection);
            DataGrid1.ItemsSource = command.ExecuteReader();
            connection.Close();
            //2
            scrollViewer.PreviewMouseWheel += new MouseWheelEventHandler(ScrollViewer_PreviewMouseWheel);
            //3
            string tableName = "Сотрудники";
            int rowCount = 0;
            using (OleDbConnection connection1 = new OleDbConnection(connectionString))
            {
                connection1.Open();

                //SQL-запрос для подсчёта количества строк
                string sql = "SELECT COUNT(*) FROM " + tableName;

                using (OleDbCommand command1 = new OleDbCommand(sql, connection1))
                {
                    rowCount = (int)command1.ExecuteScalar(); // Получение количества строк

                }
            }

            for (int number = 1; number <= rowCount; number++)
            {
                int IdSotrudnika = 0;
                string query = $"SELECT TOP 1 ID_Сотрудника FROM (SELECT TOP {number} ID_Сотрудника FROM Сотрудники ORDER BY ID_Сотрудника) ORDER BY ID_Сотрудника DESC;";

                connection.Open();
                OleDbCommand kolsotrudnokov = new OleDbCommand(query, connection);

                var numberOfSotrudnika = kolsotrudnokov.ExecuteScalar();
                IdSotrudnika = Convert.ToInt32(numberOfSotrudnika);
                string queryNameButton = $"SELECT Фамилия, Имя, Отчество FROM Сотрудники WHERE ID_Сотрудника = {IdSotrudnika}";
                OleDbCommand commandNameButton = new OleDbCommand(queryNameButton, connection);
                OleDbDataReader reader = commandNameButton.ExecuteReader();
                string fullName = "";
                if (reader.Read()) { 
                    string Surname = reader["Фамилия"].ToString();
                    string Name = reader["Имя"].ToString();
                    string Otchestvo = reader["Отчество"].ToString();

                    fullName = $" {Surname} {Name} {Otchestvo}";
                }

                Console.WriteLine(number);
                Button newButton = new Button();

                newButton.Content = $"  {number} {fullName}";
                newButton.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                newButton.Name = "ButtunOfSotrudnik_" + Convert.ToString(IdSotrudnika); // Присвоение имени кнопке
                string NName = newButton.Name;

                newButton.Click += NewButton_Click1; // Присвоение обработчика события нажатия кнопки
                
                Thickness buttonMargin = new Thickness(0, 0, 0, 10);
                newButton.Margin = buttonMargin;
                newButton.Height = 50;
                SCROLL.Children.Add(newButton);
                var hg = SCROLL.Height;
                SCROLL.Height = hg + 60;
                connection.Close();
            }
            

        }




        string connectionString1 = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                @"Data Source = C:\Users\gg\Downloads\Documents\WPF\Отдел кадров Колледжа.accdb;" +
                "User Id=Admin;Password=;";
        private void UploadImage(string path, string connectionString)
        {
            byte[] imageBytes = File.ReadAllBytes(path);

            string query = "UPDATE Сотрудники SET Фото = ? WHERE ID_Сотрудника = 3";

            using (OleDbConnection connection2 = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command2 = new OleDbCommand(query, connection2))
                {
                    command2.Parameters.Add("Фото", OleDbType.Binary).Value = imageBytes;
                    connection2.Open();
                    command2.ExecuteNonQuery();
                }
            }
        }

        private BitmapImage GetImage(string connectionString, string id_sotrudnika)
        {
            string query = $"SELECT Фото FROM Сотрудники WHERE ID_Сотрудника = {id_sotrudnika}"; // Замените на ваш запрос

            using (OleDbConnection connection1 = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command1 = new OleDbCommand(query, connection1))
                {
                    connection1.Open();
                    byte[] imageBytes = command1.ExecuteScalar() as byte[];

                    if (imageBytes != null)
                    {
                        return ConvertBytesToBitmapImage(imageBytes);
                    }
                }
            }

            return null;
        }

        private BitmapImage ConvertBytesToBitmapImage(byte[] imageBytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
        
        
        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|Все файлы (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            bool? result = openFileDialog.ShowDialog();
            
            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                
                UploadImage(selectedFilePath, connectionString1);
                //BitmapImage bitmapImage1 = GetImage(connectionString1);
                
            }
        }
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.Delta < 0)
            {
                scrollViewer.LineDown();
            }
            else
            {
                scrollViewer.LineUp();
            }
            e.Handled = true;
        }
        private void NewButton_Click1(object sender, RoutedEventArgs e)
        {
            MainInfoOfSotrudnik.Visibility = Visibility.Visible;
            Button button = (Button)sender;
            string [] name = button.Name.Split('_');
            string ID_Sotrudnika = name[1];
            BitmapImage bitmapImage1 = GetImage(connectionString1, ID_Sotrudnika);
            PhotoOfSotrudnik.Source = bitmapImage1;
            Grid1.Visibility = Visibility.Collapsed;
            MainInfoOfSotrudnik.Visibility = Visibility.Visible;

            
        }
        public void Cube_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                To = 240,
                Duration = TimeSpan.FromSeconds(0.4),  // Настройка продолжительности анимации
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }  // Настройка замедления в конце анимации
            };
            DoubleAnimation animationBackManu = new DoubleAnimation
            {
                To = 240,
                Duration = TimeSpan.FromSeconds(0.4),  // Настройка продолжительности анимации
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }  // Настройка замедления в конце анимации
            };
            //Button1.Visibility = Visibility.Visible;
            Menu.BeginAnimation(WidthProperty, animation);
            MenuBack.BeginAnimation(WidthProperty, animationBackManu);
            DoubleAnimation animationButton = new DoubleAnimation
            {
                To = 120,
                Duration = TimeSpan.FromSeconds(0.4),  // Настройка продолжительности анимации
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }  // Настройка замедления в конце анимации
            };
            
            Button1.BeginAnimation(WidthProperty, animationButton);
            Button2.BeginAnimation(WidthProperty, animationButton);
            Button3.BeginAnimation(WidthProperty, animationButton);
            Button4.BeginAnimation(WidthProperty, animationButton);
            Button5.BeginAnimation(WidthProperty, animationButton);
            Button6.BeginAnimation(WidthProperty, animationButton);

            DoubleAnimation animationText_menu = new DoubleAnimation
            {
                To = 120,
                Duration = TimeSpan.FromSeconds(0.4),  // Настройка продолжительности анимации
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }  // Настройка замедления в конце анимации
            };

            Text_menu.BeginAnimation(WidthProperty, animationText_menu);



        }
        
        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Cube_MouseEnter(sender, e);
            Button button = (Button)sender;
            DoubleAnimation animationButton = new DoubleAnimation
            {
                To = 160,
                Duration = TimeSpan.FromSeconds(0.3),  // Настройка продолжительности анимации
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }  // Настройка замедления в конце анимации
            };
            button.BeginAnimation(WidthProperty, animationButton);
            
            Thickness margin = button.Margin;

            // Получаем значение X из Margin
            double xValue = margin.Left;
            double targetX = -20; // Предположим, что мы хотим переместить кубик в точку X=200

            if (1==1)
            {
                DoubleAnimation anim = new DoubleAnimation
                {
                    To = targetX,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }// Продолжительность анимации (1 секунда)
                };
                button.BeginAnimation(Canvas.LeftProperty, anim);
                
            }

        }
        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Button button = (Button)sender;

            //DoubleAnimation animationButton = new DoubleAnimation
            //{
                //To = 120,
                //Duration = TimeSpan.FromSeconds(0)  // Настройка продолжительности анимации
                 // Настройка замедления в конце анимации
            //};

            //button.BeginAnimation(WidthProperty, animationButton);
            //Thickness margin = button.Margin;

            // Получаем значение X из Margin
            //double xValue = margin.Left;
            //double targetX = 0; // Предположим, что мы хотим переместить кубик в точку X=200

            // Добавляем анимацию перемещения
            if (1==1)
            {
                DoubleAnimation anim = new DoubleAnimation
                {
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.42),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }

                };
                button.BeginAnimation(Canvas.LeftProperty, anim);
            }

        }
        private void KKKKK(object sender, RoutedEventArgs e)
        {
            MainInfoOfSotrudnik.Visibility = Visibility.Collapsed;
            
            Grid1.Visibility = Visibility.Visible;
            
                
        }
        private void Cube_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                To = 70,
                Duration = TimeSpan.FromSeconds(0.4),  // Настройка продолжительности анимации
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }  // Настройка замедления в конце анимации
            };
            DoubleAnimation animationBackManu = new DoubleAnimation
            {
                To = 70,
                Duration = TimeSpan.FromSeconds(0.4),  // Настройка продолжительности анимации
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }  // Настройка замедления в конце анимации
            };

            Menu.BeginAnimation(WidthProperty, animation);
            MenuBack.BeginAnimation(WidthProperty, animationBackManu);
            DoubleAnimation animationButton = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.2),  // Настройка продолжительности анимации
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }  // Настройка замедления в конце анимации
            };

            Button1.BeginAnimation(WidthProperty, animationButton);
            Button2.BeginAnimation(WidthProperty, animationButton);
            Button3.BeginAnimation(WidthProperty, animationButton);
            Button4.BeginAnimation(WidthProperty, animationButton);
            Button5.BeginAnimation(WidthProperty, animationButton);
            Button6.BeginAnimation(WidthProperty, animationButton);
            //Button1.Visibility = Visibility.Collapsed;
            DoubleAnimation animationText_menu = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.2),  // Настройка продолжительности анимации
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }  // Настройка замедления в конце анимации
            };

            Text_menu.BeginAnimation(WidthProperty, animationText_menu);


        }
        
        private void Exit_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();  // Закрываем текущее окно при клике на изображение
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Thickness margin = button.Margin;

            // Получаем значение X из Margin
            double xValue = margin.Top;
            double targetX = xValue - 5;

            // Добавляем анимацию перемещения
            DoubleAnimation anim = new DoubleAnimation
            {
                To = targetX,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }// Продолжительность анимации (1 секунда)
            };
            MenuBack.BeginAnimation(Canvas.TopProperty, anim);



        }
        













        private void Label_MouseEnter(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
            
        }

        private void Label_MouseLeave(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
