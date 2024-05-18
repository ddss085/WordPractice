using System.Collections.ObjectModel;
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
                    default:
                        break;
                }
            }
        }

        private void OnAdd()
        {
            QuestionSets.Add(new QuestionSet { Question = "", Hanzi = "", Pinyin = "" });
            _root.QuestionBunches[lstView.SelectedIndex].QuestionSets = QuestionSets.ToArray();
            SaveXmlData();
        }
        private void OnRemove()
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
                        case Key.N:
                            OnAdd();
                            break;
                        case Key.D:
                            OnRemove();
                            break;
                    }
                }
            }
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
                item.Use = checkBox.IsChecked.Value;
                SaveXmlData();
                UpdateActiveQuestionSets();
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

        private void dtGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _root.QuestionBunches[lstView.SelectedIndex].QuestionSets = QuestionSets.ToArray();
            SaveXmlData();
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