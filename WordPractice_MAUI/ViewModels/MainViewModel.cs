using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml.Serialization;
using WordPractice_MAUI.Models;

namespace WordPractice_MAUI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Field
        private string _textCount;
        private string _textQuestion;
        private string _textHanzi;
        private string _textPinyin;
        private bool _showHanzi;
        private QuestionSet? _pickedQuestion;
        private Stack<QuestionSet> _questionsStack;
        private List<QuestionSet> _activeQuestionSets;
        #endregion
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Property
        public string TextCount
        {
            get => _textCount;
            set
            {
                if (_textCount != value)
                {
                    _textCount = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TextQuestion
        {
            get => _textQuestion;
            set
            {
                if (_textQuestion != value)
                {
                    _textQuestion = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TextHanzi
        {
            get => _textHanzi;
            set
            {
                if (_textHanzi != value)
                {
                    _textHanzi = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TextPinyin
        {
            get => _textPinyin;
            set
            {
                if (_textPinyin != value)
                {
                    _textPinyin = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool ShowHanzi
        {
            get => _showHanzi;
            set
            {
                if (_showHanzi != value)
                {
                    _showHanzi = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<QuestionBunch> QuestionBunches { get; set; } = XmlManager.Instance.QuestionBunches;

        #endregion

        public MainViewModel()
        {
            _textQuestion = "Hello, MAUI!";
            CommandButton = new Command<string>(OnCommandButton);
            _questionsStack = [];
            _activeQuestionSets = [];

            XmlManager.Instance.Initialize();
            XmlManager.Instance.UseChanged += OnUseChanged;
            OnNext();
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
                Application.Current.MainPage.DisplayAlert("Alert", "There is no question to show!", "OK");
                //MessageBox.Show("There is no question to show!");
            }
            UpdateGUI();
        }

        private void OnShow()
        {
            ShowHanzi = true;
            //txtHanzi.Visibility = Visibility.Visible;
        }

        private void OnPrevious()
        {
            _questionsStack.TryPop(out QuestionSet? question);
            _pickedQuestion = question;
            UpdateGUI();
        }

        private void OnCommandButton(string text)
        {
            switch (text)
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

        private void UpdateGUI()
        {
            if (_pickedQuestion == null)
            {
                TextCount = "0";
                TextQuestion = "Question";
                TextHanzi = "Hanzi";
                TextPinyin = "Pinyin";
                ShowHanzi = true;
                //txtQuestion.Text = "Question";
                //txtHanzi.Text = "Hanzi";
                //txtPinyin.Text = "Pinyin";
                //txtHanzi.Visibility = Visibility.Visible;
                //txtCount.Text = "";
            }
            else
            {
                TextCount = (_questionsStack.Count + 1).ToString();
                TextQuestion = _pickedQuestion.Question;
                TextHanzi = _pickedQuestion.Hanzi;
                TextPinyin = _pickedQuestion.Pinyin;
                ShowHanzi = false;
                //txtQuestion.Text = _pickedQuestion.Question;
                //txtHanzi.Text = _pickedQuestion.Hanzi;
                //txtPinyin.Text = _pickedQuestion.Pinyin;
                //txtHanzi.Visibility = Visibility.Collapsed;
                //txtCount.Text = (_questionsStack.Count + 1).ToString();
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

        #region Command
        public ICommand CommandButton { get; set; }
        #endregion

        #region HandlerMethod
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void OnUseChanged(object? sender, EventArgs e)
        {
            UpdateActiveQuestionSets();
        }
        #endregion
    }
}