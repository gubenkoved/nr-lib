using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace NRLib
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEnumerable<string> m_allowedExtensions;
        private Regex m_fileNameTemplateRegex = new Regex(@"^((?<autors>[^-]+)-(?<name>.+)\.(?<extension>[^.]+))|((?<name>.+)\.(?<extension>[^.]+))$", RegexOptions.RightToLeft | RegexOptions.Compiled);
        private bool m_searching;
        private string m_libDir
        {
            get
            {
                return Properties.Settings.Default.RootDir;
            }
            set
            {
                Properties.Settings.Default.RootDir = value;
                Properties.Settings.Default.Save();
            }
        }
        private ViewMode m_viewMode;
        
        public ObservableCollection<Book> Books { get; private set; }
        public ObservableCollection<Book> Favorites { get; private set; }
        public ObservableCollection<string> LastQueries { get; private set; }

        public MainWindow()
        {
            Init();

            InitializeComponent();

            // I don't know how to bind from resource section (Window.Resources) to root element (Window)
            ((FavoriteConverter)Resources["FavoriteConverter"]).FavoriteBooksCollection = Favorites;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            QueryBox.Focus();

            Activate();            
        }
        void Init()
        {
            m_viewMode = ViewMode.SearchResults;

            Books = new ObservableCollection<Book>();

            #region LastQeuries
            if (Properties.Settings.Default.LastQueries == null)
                Properties.Settings.Default.LastQueries = new System.Collections.Specialized.StringCollection();

            LastQueries = new ObservableCollection<string>(Properties.Settings.Default.LastQueries.OfType<string>());

            LastQueries.CollectionChanged += (s, a) =>
                {
                    Properties.Settings.Default.LastQueries.Clear();
                    Properties.Settings.Default.LastQueries.AddRange(LastQueries.ToArray());

                    Properties.Settings.Default.Save();
                };
            #endregion

            #region Favorites
            if (Properties.Settings.Default.Favorites == null)
                Properties.Settings.Default.Favorites = new System.Collections.Specialized.StringCollection();

            Favorites = new ObservableCollection<Book>(
                Properties.Settings.Default.Favorites
                .OfType<string>()
                .Select(path => CreateBookFromFileInfo(new FileInfo(path))));

            Favorites.CollectionChanged += (s, a) => SaveFavorites();
            #endregion

            m_allowedExtensions = Properties.Settings.Default.Extensions.Split('|').Select(e => e.ToLower());
        }
        void Invoke(Action action)
        {
            Dispatcher.Invoke((Delegate)action);
        }
        void SaveFavorites()
        {
            if (Properties.Settings.Default.Favorites == null)
                Properties.Settings.Default.Favorites = new System.Collections.Specialized.StringCollection();

            Properties.Settings.Default.Favorites.Clear();
            Properties.Settings.Default.Favorites.AddRange(Favorites.Select(b => b.FileInfo.FullName).ToArray());

            Properties.Settings.Default.Save();
        }
        void RecourseLookIn(DirectoryInfo dir, string query)
        {
            try
            {
                foreach (var file in dir.GetFiles())
                {
                    TryAddBook(file, query);
                }

                foreach (var subdirectory in dir.GetDirectories())
                {
                    RecourseLookIn(subdirectory, query);
                }
            }
            catch (Exception)
            {
            }
        }
        void UpdateLastQueries(string query)
        {
            if (LastQueries.Contains(query))
                LastQueries.Remove(query);

            LastQueries.Insert(0, query);

            while (LastQueries.Count > Properties.Settings.Default.MaxItemsInHistoryList)
                LastQueries.RemoveAt(Properties.Settings.Default.MaxItemsInHistoryList);
        }
        
        void StartFind(DirectoryInfo directory)
        {
            string query = QueryBox.Text;

            Books.Clear();

            UpdateLastQueries(query);

            SearchProgress.IsIndeterminate = true;
            m_searching = true;

            Task searchTask = new Task(
                () =>
                {
                    RecourseLookIn(directory, query);
                    Dispatcher.Invoke((Action)(() => SearchCompleted()));
                });

            searchTask.Start();
        }        
        void SearchCompleted()
        {
            SearchProgress.IsIndeterminate = false;

            m_searching = false;
        }
        void AddBook(Book book)
        {
            Books.Add(book);
        }
        void TryAddBook(FileInfo file, string query)
        {
            if (m_allowedExtensions.Contains(file.Extension.ToLower()))
            {
                if ( query.Split(' ').All(s => file.FullName.ToLower().IndexOf(s.ToLower()) != -1))
                {
                    Book book = CreateBookFromFileInfo(file);

                    if (book != null)
                        Invoke(() => AddBook(book));
                }
            }
        }
        Book CreateBookFromFileInfo(FileInfo file)
        {
            Match templated = m_fileNameTemplateRegex.Match(file.Name);

            if (templated.Success)
            {
                List<string> authors = new List<string>();

                if (templated.Groups["autors"].Success)
                {
                    foreach (var autor in templated.Groups["autors"].Value.Split(','))
                    {
                        authors.Add(autor.Trim());
                    }
                }

                return new Book() {
                    FileInfo = file,
                    Name = templated.Groups["name"].Value.Trim(),
                    Extension = templated.Groups["extension"].Value.Trim(),
                    Authors = authors
                };
            }

            return null;
        }

        private void Search_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DirectoryInfo libDirectory = null;

            while (libDirectory == null)
            {
                try
                {
                    var dir = new DirectoryInfo(m_libDir);

                    if (!dir.Exists)
                        throw new Exception("Directory doesn't not exists");

                    libDirectory = dir;
                }
                catch (Exception exc)
                {
                    Commands.ChangeLibDirCommand.Execute(null, this);
                }
            }

            ChangeViewMode(ViewMode.SearchResults);
            StartFind(libDirectory);
            
        }
        private void AutorList_SelectAutor(object sender, MouseButtonEventArgs e)
        {
            var autorsListBox = sender as ListBox;
            var autors = autorsListBox.SelectedItem as string;

            if (autors != null)
            {
                QueryBox.Text = autors.Split(new[] { ' ', '.' }).OrderByDescending(s => s.Length).First();

                if (!m_searching)
                    Commands.SearchCommand.Execute(null, this);
            }
        }
        private void QueryTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!m_searching)
                    Commands.SearchCommand.Execute(null, this);
            }
        }        
        private void OpenContainingFolder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Book book = (Book)e.Parameter;

            Process.Start("explorer.exe", string.Format("{0}\"{1}\"", "/select,", book.FileInfo.FullName));
        }
        private void OpenFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Book book = (Book)e.Parameter;

            OpenFile(book);
        }
        private void OpenFile(Book book)
        {
            if (File.Exists(book.FileInfo.FullName))
            {
                Process.Start(book.FileInfo.FullName);
                
            }
            else if (MessageBox.Show("File not finded.\nDo you want to specify new path for it?",
                    "Error", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OpenFileDialog ofd = new OpenFileDialog();

                if (ofd.ShowDialog() == true)
                {
                    book.FileInfo = new FileInfo(ofd.FileName);

                    SaveFavorites();

                    Process.Start(book.FileInfo.FullName);
                }
            }
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                OpenFile((sender as FrameworkElement).DataContext as Book);
            }
        }
        private void ChangeLibDir_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var folderSelector = new System.Windows.Forms.FolderBrowserDialog();
            folderSelector.Description = "Select root library dir:";

            if (folderSelector.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_libDir = folderSelector.SelectedPath;
            }
        }
        private void ToogleFavorite_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Book book = (Book)e.Parameter;

            if (Favorites.Contains(book))
            {
                // delete
                Favorites.Remove(book);
            }
            else
            {
                // add
                Favorites.Add(book);
            }

            // manual force to re-evaluate converter result (image update)
            var imageButton = (ImageButton)e.OriginalSource;
            var bindingExpression = imageButton.GetBindingExpression(ImageButton.ImageProperty);

            imageButton.SetBinding(ImageButton.ImageProperty, bindingExpression.ParentBinding);            
        }

        private void ShowFavorites_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (m_viewMode == ViewMode.SearchResults)
            {
                ChangeViewMode(ViewMode.Favorites);
            }
            else
            {
                ChangeViewMode(ViewMode.SearchResults);
            }
        }

        void ChangeViewMode(ViewMode mode)
        {
            if (mode != m_viewMode)
            {
                m_viewMode = mode;

                if (m_viewMode == ViewMode.SearchResults)
                {
                    ShowFavoritesButton.IsToogled = false;
                    fileList.ItemsSource = Books;
                }
                else
                {
                    ShowFavoritesButton.IsToogled = true;
                    fileList.ItemsSource = Favorites;
                }
            }
        }
        enum ViewMode
        {
            SearchResults,
            Favorites
        }
    }

    [Serializable]
    public class Book
    {
        private FileInfo m_fileInfo;
        public FileInfo FileInfo
        {
            get { return m_fileInfo; }
            set { m_fileInfo = value; }
        }
        [NonSerialized]
        private IEnumerable<string> m_authors;
        public IEnumerable<string> Authors
        {
            get { return m_authors; }
            set { m_authors = value; }
        }
        
        [NonSerialized]
        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        
        [NonSerialized]
        private string m_extension;
        public string Extension
        {
            get { return m_extension; }
            set { m_extension = value; }
        }        

        public Book()
        {
            Authors = new List<string>();
        }

        public override bool Equals(object obj)
        {
            if (obj is Book)
            {
                return this.FileInfo.FullName == ((Book)obj).FileInfo.FullName;
            }

            return false;
        }
    }

    public class FavoriteConverter : DependencyObject, IValueConverter
    {
        public IEnumerable<Book> FavoriteBooksCollection
        {
            get { return (IEnumerable<Book>)GetValue(FavoriteBooksCollectionProperty); }
            set { SetValue(FavoriteBooksCollectionProperty, value); }
        }
        public static readonly DependencyProperty FavoriteBooksCollectionProperty = DependencyProperty.Register("FavoriteBooksCollection", typeof(IEnumerable<Book>), typeof(FavoriteConverter), new UIPropertyMetadata(null));

        public ImageSource FavoriteBookImage
        {
            get { return (ImageSource)GetValue(FavoriteBookImageProperty); }
            set { SetValue(FavoriteBookImageProperty, value); }
        }
        public static readonly DependencyProperty FavoriteBookImageProperty = DependencyProperty.Register("FavoriteBookImage", typeof(ImageSource), typeof(FavoriteConverter), new UIPropertyMetadata(null));

        public ImageSource NotFavoriteBookImage
        {
            get { return (ImageSource)GetValue(NotFavoriteBookImageProperty); }
            set { SetValue(NotFavoriteBookImageProperty, value); }
        }
        public static readonly DependencyProperty NotFavoriteBookImageProperty = DependencyProperty.Register("NotFavoriteBookImage", typeof(ImageSource), typeof(FavoriteConverter), new UIPropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Book book = (Book)value;

            if (FavoriteBooksCollection != null && FavoriteBooksCollection.Contains(book))
            {
                return FavoriteBookImage;
            }

            return NotFavoriteBookImage;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
