using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordPractice_MAUI.Models
{
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
