using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using Image = System.Windows.Media.ImageSource;
using System.Collections.ObjectModel;
using System.IO.Enumeration;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Text;

namespace ImageSlideShow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string imagePaths = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        static string appfolder = Path.Combine(imagePaths, "imageMarket");

        private string[] _imagePaths;
        private int _currentImageIndex;

        public MainWindow()
        {
            InitializeComponent();


            if (!Directory.Exists(appfolder))
            {
                Directory.CreateDirectory(appfolder);
            }

            this.Loaded += Window_Loaded;

            


        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] imagePaths = await LoadAllImagesAsync(appfolder);
            _imagePaths = imagePaths;

            DataContext = this;

            _currentImageIndex = SetRandomIndex();

            if (_imagePaths.Length > 0)
            {
                try
                {
                    ChangeImage();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            else
            {
                MessageBox.Show("Нет изображений в папке", "Пустая папка!", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                this.Close();
            }
        }

        private async Task<string[]> LoadAllImagesAsync(string path)
        {
            return await Task.Run(() => Directory.GetFiles(path));
        }

        private int SetRandomIndex()
        {
            Random rand = new();
            return rand.Next(0, _imagePaths.Length - 1);
        }

        private int ShowNext() // пришлось написать этот тк ниже строка не работает ((
        {
            if (_currentImageIndex == _imagePaths.Length - 1)
            {
                _currentImageIndex = 0;
            }
            else
            {
                _currentImageIndex++;
            }

            return _currentImageIndex;
        }


        private void ChangeImage()
        {
            //var bitmap = new BitmapImage(new Uri(_imagePaths[_currentImageIndex], UriKind.Relative));
            ImageViewer.Source = GetGlowingImage(_imagePaths[ShowNext()]);

            //_currentImageIndex = (_currentImageIndex + 1) % _imagePaths.Length; 
            // Выше указзаный код почему то не работает, возвращает только один и тот же фото ((


            var fadeIn = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(1));
            var fadeOut = new DoubleAnimation(1.0, 0.0, TimeSpan.FromSeconds(2));
            fadeOut.BeginTime = TimeSpan.FromSeconds(3);
            fadeOut.Completed += (s, e) => ChangeImage();

            var storyboard = new Storyboard();
            storyboard.Children.Add(fadeIn);
            storyboard.Children.Add(fadeOut);

            Storyboard.SetTarget(fadeIn, ImageViewer);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(fadeOut, ImageViewer);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));

            storyboard.Begin();
        }


        public ImageSource GetGlowingImage(string name)
        {
            BitmapImage glowIcon = new BitmapImage();
            glowIcon.BeginInit();
            glowIcon.UriSource = new Uri(name, UriKind.RelativeOrAbsolute);
            glowIcon.CacheOption = BitmapCacheOption.OnLoad;
            glowIcon.EndInit();

            return glowIcon;
        }

        
    }
}