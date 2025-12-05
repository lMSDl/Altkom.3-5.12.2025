
namespace TDD
{
    public class FizzBuzz
    {
        public static string GetFizzBuzz(int number)
        {
            string result = string.Empty;

            if (IsFizz(number))
                result += "Fizz";
            if (IsBuzz(number))
                result += "Buzz";

            return string.IsNullOrEmpty(result) ? number.ToString() : result;
        }

        public static string GetSequence(int count)
        {
            return string.Join(" ",
                Enumerable.Range(1, count)
                .Select(GetFizzBuzz));
        }

        private static bool IsBuzz(int number)
        {
            return number % 5 == 0;
        }

        private static bool IsFizz(int number)
        {
            return number % 3 == 0;
        }
    }
}

