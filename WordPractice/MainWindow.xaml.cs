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

            _questionsStack = new Stack<QuestionSet>();
        }

        private QuestionSets _questionSets = null;
        private QuestionSet _pickedQuestion = null;
        private Stack<QuestionSet> _questionsStack = null;

        private void LoadXmlFile()
        {
            string filePath = "Data/Questions.xml";
            //string fullPath = Directory.GetCurrentDirectory() + '/' + filePath;


            XmlSerializer serializer = new XmlSerializer(typeof(QuestionSets));

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    // 파일 스트림의 위치 확인 및 조정
                    if (fileStream.Position != 0)
                    {
                        fileStream.Seek(0, SeekOrigin.Begin);
                    }

                    // 역직렬화 수행
                    _questionSets = (QuestionSets)serializer.Deserialize(fileStream);
                }
            }
            catch (Exception ex)
            {
                // 오류 발생 시 예외 내용 출력
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_questionSets == null)
            {
                MessageBox.Show("xml 데이터를 불러올 수 없습니다.");
                Application.Current.Shutdown();
            }
            else OnNext();
        }


        private void UpdateGUI()
        {
            txtQuestion.Text = _pickedQuestion.Question;
            txtHanzi.Text = _pickedQuestion.Hanzi;
            txtPinyin.Text = _pickedQuestion.Pinyin;
            txtHanzi.Visibility = Visibility.Collapsed;
            txtCount.Text = (_questionsStack.Count + 1).ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
            if (_pickedQuestion != null) _questionsStack.Push(_pickedQuestion);
            _pickedQuestion = RandomPicker.PickRandom(_questionSets.Sets);
            UpdateGUI();
        }

        private void OnShow()
        {
            txtHanzi.Visibility = Visibility.Visible;
        }

        private void OnPrevious()
        {
            if (_questionsStack.TryPop(out QuestionSet question))
            {
                _pickedQuestion = question;
                UpdateGUI();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
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
    }

    [XmlRoot("QuestionSets")]
    public class QuestionSets
    {
        [XmlElement("QuestionSet")]
        public QuestionSet[] Sets { get; set; }
    }

    // XML 구조에 대응하는 클래스 정의
    public class QuestionSet
    {
        public string Question { get; set; }
        public string Hanzi { get; set; }
        public string Pinyin { get; set; }
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