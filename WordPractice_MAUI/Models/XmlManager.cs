using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WordPractice_MAUI.Models
{
    public class XmlManager
    {
        private static XmlManager _instance;
        public static XmlManager Instance
        {
            get => _instance ??= new XmlManager();
        }

        private string _filePath = "Questions.xml";

        public event EventHandler UseChanged;

        public ObservableCollection<QuestionBunch> QuestionBunches { get; set; } = [];
        public Root XmlRoot { get; set; }
        public void OnUseChanged()
        {
            UseChanged?.Invoke(this, new EventArgs());
        }

        //private void LoadXmlFile()
        //{
        //    string? filePath = "WordPractice.Questions.xml";
        //    //string fullPath = Directory.GetCurrentDirectory() + '/' + filePath;


        //    XmlSerializer? serializer = new(typeof(Root));

        //    try
        //    {
        //        using (FileStream? fileStream = new(filePath, FileMode.Open))
        //        {
        //            // 파일 스트림의 위치 확인 및 조정
        //            if (fileStream.Position != 0)
        //            {
        //                fileStream.Seek(0, SeekOrigin.Begin);
        //            }

        //            // 역직렬화 수행
        //            _root = serializer.Deserialize(fileStream) as Root;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // 오류 발생 시 예외 내용 출력
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //    }
        //}
        public void Initialize()
        {
            _filePath = "Questions.xml";
            LoadXmlFile();
        }

        private async void LoadXmlFile()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(_filePath);
                using var reader = new StreamReader(stream);

                var contents = await reader.ReadToEndAsync();

                if (!string.IsNullOrEmpty(contents))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Root));
                    using var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
                    XmlRoot = serializer.Deserialize(contentStream) as Root;

                    if (XmlRoot != null)
                    {
                        QuestionBunches.Clear();
                        foreach (var questionBunch in XmlRoot.QuestionBunches)
                        {
                            QuestionBunches.Add(questionBunch);
                        }
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "File content is empty.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Unable to load XML file: {ex.Message}", "OK");
            }
        }
        public async void SaveXmlFile()
        {
            //string filePath = Path.Combine(FileSystem.AppDataDirectory, _filePath);
;
            //XmlSerializer serializer = new XmlSerializer(typeof(Root));
            //using var writer = new StreamWriter();
            //var xml = new XmlWriter();
            //xml.
            //serializer.Serialize(writer, XmlRoot);
        }

    }
}
