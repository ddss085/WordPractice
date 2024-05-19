using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Xml.Serialization;

namespace WordPractice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadXmlFile();

            _questionsStack = [];
            _activeQuestionSets = [];
        }

        private Root? _root;
        private Stack<QuestionSet> _questionsStack;
        private List<QuestionSet> _activeQuestionSets;

        private void LoadXmlFile()
        {
            string? filePath = "Data/Questions.xml";
            //string fullPath = Directory.GetCurrentDirectory() + '/' + filePath;


            XmlSerializer? serializer = new(typeof(Root));

            try
            {
                using (FileStream? fileStream = new(filePath, FileMode.Open))
                {
                    // 파일 스트림의 위치 확인 및 조정
                    if (fileStream.Position != 0)
                    {
                        fileStream.Seek(0, SeekOrigin.Begin);
                    }

                    // 역직렬화 수행
                    _root = serializer.Deserialize(fileStream) as Root;
                }
            }
            catch (Exception ex)
            {
                // 오류 발생 시 예외 내용 출력
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        private void SaveXmlData()
        {
            string filePath = "Data/Questions.xml";
            XmlSerializer serializer = new(typeof(Root));

            try
            {
                using (FileStream fileStream = new(filePath, FileMode.Create))
                {
                    // 직렬화 수행
                    serializer.Serialize(fileStream, _root);
                }
                Console.WriteLine("Data saved successfully.");
            }
            catch (Exception ex)
            {
                // 오류 발생 시 예외 내용 출력
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            UpdateActiveQuestionSets();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_root == null)
            {
                MessageBox.Show("xml 데이터를 불러올 수 없습니다.");
                Application.Current.Shutdown();
            }
            else
            {
                QuestionBunches = new ObservableCollection<QuestionBunch>(_root.QuestionBunches);
                QuestionSets = new ObservableCollection<QuestionSet>();
                lstView.ItemsSource = QuestionBunches;
                dtGrid.ItemsSource = QuestionSets;
                UpdateActiveQuestionSets();
                OnNext();
            }
        }

        #region Run GUI
        private QuestionSet? _pickedQuestion;

        private void UpdateGUI()
        {
            if (_pickedQuestion == null)
            {
                txtQuestion.Text = "Question";
                txtHanzi.Text = "Hanzi";
                txtPinyin.Text = "Pinyin";
                txtHanzi.Visibility = Visibility.Visible;
                txtCount.Text = "";
            }
            else
            {
                txtQuestion.Text = _pickedQuestion.Question;
                txtHanzi.Text = _pickedQuestion.Hanzi;
                txtPinyin.Text = _pickedQuestion.Pinyin;
                txtHanzi.Visibility = Visibility.Collapsed;
                txtCount.Text = (_questionsStack.Count + 1).ToString();
            }
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                switch (button.Tag)
                {
                    case "Previous":
                        OnPrevious();
                        break;
                    case "Show":
                        OnShow();
                        break;
                    case "Next":
                        OnNext();
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnNext()
        {
            if (_pickedQuestion != null) _questionsStack?.Push(_pickedQuestion);
            if (_activeQuestionSets.Count > 0)
            {
                _pickedQuestion = RandomPicker.PickRandom(_activeQuestionSets.ToArray());
            }
            else
            {
                _pickedQuestion = null;
                MessageBox.Show("There is no question to show!");
            }
            UpdateGUI();
        }

        private void OnShow()
        {
            txtHanzi.Visibility = Visibility.Visible;
        }

        private void OnPrevious()
        {
            _questionsStack.TryPop(out QuestionSet? question);
            _pickedQuestion = question;
            UpdateGUI();
        }


        #endregion

        #region Edit GUI
        public ObservableCollection<QuestionBunch> QuestionBunches { get; set; }
        public ObservableCollection<QuestionSet> QuestionSets { get; set; }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                switch (button.Tag)
                {
                    case "Add":
                        OnAdd();
                        break;
                    case "Remove":
                        OnRemove();
                        break;
                    case "New":
                        OnNew();
                        break;
                    case "Delete":
                        OnDelete();
                        break;
                    case "Save":
                        OnSave();
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnAdd()
        {
            QuestionBunches.Add(new QuestionBunch { Name = "New Bunch", Use = false, QuestionSets = [] });
            _root.QuestionBunches = QuestionBunches.ToArray();
            SaveXmlData();
        }
        private void OnRemove()
        {
            if (lstView.SelectedItem is QuestionBunch selectedBunch)
            {
                if (MessageBox.Show("Are you really gonna remove this bunch?", "", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                {
                    if (MessageBox.Show("REALLY?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        QuestionBunches.Remove(selectedBunch);
                        _root.QuestionBunches = QuestionBunches.ToArray();
                        SaveXmlData();
                    }
                }
            }
        }
        private void OnNew()
        {
            QuestionSets.Add(new QuestionSet { Question = "", Hanzi = "", Pinyin = "" });
            _root.QuestionBunches[lstView.SelectedIndex].QuestionSets = QuestionSets.ToArray();
            SaveXmlData();
        }
        private void OnDelete()
        {
            if (dtGrid.SelectedItem is QuestionSet selectedQuestion)
            {
                if (MessageBox.Show("Are you really gonna delete this question?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QuestionSets.Remove(selectedQuestion);
                    _root.QuestionBunches[lstView.SelectedIndex].QuestionSets = QuestionSets.ToArray();
                    SaveXmlData();
                }
            }
            else
            {
                MessageBox.Show("Please select a question to delete.");
            }
        }
        private void OnSave()
        {
            SaveXmlData();
            MessageBox.Show("A xml file has been saved.");
        }


        private void ListView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (lstView.SelectedItem is QuestionBunch questionBunch)
            {
                QuestionSets.Clear();
                foreach (var question in questionBunch.QuestionSets)
                {
                    QuestionSets.Add(question);
                }
            }
        }

        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is QuestionBunch item)
            {
                if (checkBox.IsChecked.HasValue)
                    item.Use = checkBox.IsChecked.Value;
                SaveXmlData();
            }
        }

        private DateTime _lastClickTime;
        private const int DoubleClickSpeed = 300; // milliseconds

        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DateTime now = DateTime.Now;
            if ((now - _lastClickTime).TotalMilliseconds <= DoubleClickSpeed)
            {
                if (sender is TextBlock textBlock)
                {
                    ChangeBunchName(textBlock);
                }
            }
            _lastClickTime = now;
        }

        private void ChangeBunchName(TextBlock textBlock)
        {
            var textBox = FindSibling<TextBox>(textBlock);
            if (textBox != null)
            {
                textBox.Text = textBlock.Text;
                textBlock.Visibility = Visibility.Collapsed;
                textBox.Visibility = Visibility.Visible;
                // Dispatcher를 사용하여 포커스를 지연 설정
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    textBox.Focus();
                    textBox.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var textBlock = FindSibling<TextBlock>(textBox);
                if (textBlock != null)
                {
                    textBlock.Text = textBox.Text;
                    textBox.Visibility = Visibility.Collapsed;
                    textBlock.Visibility = Visibility.Visible;
                    SaveXmlData();
                }
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var textBlock = FindSibling<TextBlock>(textBox);
                if (textBlock != null)
                {
                    if (e.Key == Key.Enter)
                    {
                        textBlock.Text = textBox.Text;
                        textBox.Visibility = Visibility.Collapsed;
                        textBlock.Visibility = Visibility.Visible;
                        SaveXmlData();
                    }
                    else if (e.Key == Key.Escape)
                    {
                        textBox.Visibility = Visibility.Collapsed;
                        textBlock.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _root.QuestionBunches[lstView.SelectedIndex].QuestionSets = QuestionSets.ToArray();
            SaveXmlData();
        }
        private void DataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Clear sorting
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(dtGrid.ItemsSource);
            if (collectionView != null)
            {
                collectionView.SortDescriptions.Clear();
                // Refresh the view
                collectionView.Refresh();
            }

            // Clear sort direction indicators
            foreach (var column in dtGrid.Columns)
            {
                column.SortDirection = null;
            }
        }

        private T FindSibling<T>(FrameworkElement element) where T : FrameworkElement
        {
            var parent = (FrameworkElement)element.Parent;
            foreach (var child in parent.FindVisualChildren<T>())
            {
                return child;
            }
            return null;
        }
        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T t)
                {
                    return t;
                }

                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        #endregion

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (tbControl?.SelectedIndex == 0)
            {
                switch (e.Key)
                {
                    case Key.P:
                        OnPrevious();
                        break;
                    case Key.S:
                        OnShow();
                        break;
                    case Key.N:
                        OnNext();
                        break;
                    default:
                        break;
                }
            }
            if (tbControl?.SelectedIndex == 1)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    switch (e.Key)
                    {
                        case Key.A:
                            OnAdd();
                            break;
                        case Key.R:
                            OnRemove();
                            break;
                        case Key.N:
                            OnNew();
                            break;
                        case Key.D:
                            OnDelete();
                            break;
                        case Key.S:
                            OnSave();
                            break;
                    }
                }
                if (e.Key == Key.F2)
                {
                    if (lstView.ItemContainerGenerator.ContainerFromItem(lstView.SelectedItem) is ListViewItem container && container.IsFocused)
                    {
                        var textBlock = FindVisualChild<TextBlock>(container);
                        ChangeBunchName(textBlock);
                    }
                }
            }
        }

        private void UpdateActiveQuestionSets()
        {
            _activeQuestionSets.Clear();
            foreach (var questionBunch in QuestionBunches)
            {
                if (questionBunch.Use)
                    _activeQuestionSets.AddRange(questionBunch.QuestionSets);
            }
        }

    }

    [XmlRoot("Root")]
    public class Root
    {
        [XmlElement("QuestionBunch")]
        public required QuestionBunch[] QuestionBunches { get; set; }

    }
    public class QuestionBunch
    {
        [XmlElement("Name")]
        public required string Name { get; set; }
        [XmlElement("Use")]
        public required bool Use { get; set; }
        [XmlElement("QuestionSet")]
        public required QuestionSet[] QuestionSets { get; set; }
    }

    // XML 구조에 대응하는 클래스 정의
    public class QuestionSet
    {
        [XmlElement("Question")]
        public required string Question { get; set; }
        [XmlElement("Hanzi")]
        public required string Hanzi { get; set; }
        [XmlElement("Pinyin")]
        public required string Pinyin { get; set; }
    }

    public class RandomPicker
    {
        // Random 인스턴스를 정적 필드로 선언하여 재사용
        private static Random random = new Random();

        public static T PickRandom<T>(T[] array)
        {
            int index = random.Next(0, array.Length);
            return array[index];
        }
    }
}