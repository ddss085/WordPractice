using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordPractice_MAUI.Models
{

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

}
