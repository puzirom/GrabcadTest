using System;

namespace MathCompetition.Models
{
    public class Mathematics
    {
        #region Public Properties

        public int Result { get; private set; }
        public string Id { get; private set; }
        public string Text { get; private set; }
        public string[] Answers { get; private set; }
        public bool IsEmpty => string.IsNullOrEmpty(Id);

        #endregion

        #region Private Variables and Costants

        private const int MaxVal = 10;
        private const int MinVal = 1;
        private static readonly Random Random = new Random();
        private static Mathematics _instance;

        #endregion
        
        public static Mathematics Instance => _instance ?? (_instance = new Mathematics());

        public void GenerateNew()
        {
            GenerateQuestion();
            GenerateAnswers();
        }

        public AnswerResult CompareResult(string questionId, string result)
        {
            if (Id == string.Empty)
            {
                return AnswerResult.Late;
            }

            var isIdsEqual = string.Equals(questionId, Id, StringComparison.OrdinalIgnoreCase);
            var isResultsEqual = string.Equals(result, Result.ToString(), StringComparison.OrdinalIgnoreCase);          
            
            if (isIdsEqual && isResultsEqual)
            {
                Id = string.Empty;
                return AnswerResult.Correct;
            }

            if (isIdsEqual)
            {
                return AnswerResult.Incorrect;
            }

            return AnswerResult.Late;
        }

        #region Private Accessory Methods

        private void GenerateQuestion()
        {
            Id = Guid.NewGuid().ToString("N");
            const string format = "{0} {2} {1}?";

            var a = GetRandomValue(MinVal, MaxVal);
            var b = GetRandomValue(MinVal, MaxVal);

            var variant = GetRandomValue(0, 3);
            switch (variant) {
                case 0:
                    Result = a + b;
                    Text = string.Format(format, a, b, "+");
                    break;
                case 1:
                    Result = a - b;
                    Text = string.Format(format, a, b, "-");
                    break;
                case 2:
                    Result = a * b;
                    Text = string.Format(format, a, b, "*");
                    break;
                case 3:
                    Result = a / b;
                    Text = string.Format(format, a, b, "/");
                    break;
            }
        }

        private void GenerateAnswers()
        {
            var wrong0 = Result + GetRandomValue(MinVal, MaxVal);
            var wrong1 = Result - GetRandomValue(MinVal, MaxVal);

            var variant = GetRandomValue(0, 1);
            if (variant == 1)
            {
                wrong0 = Result - GetRandomValue(MinVal, MaxVal);
                wrong1 = Result + GetRandomValue(MinVal, MaxVal);
            }

            variant = GetRandomValue(0, 2);
            switch (variant)
            {
                case 0:
                    SetupAnswers(Result, wrong0, wrong1);
                    break;
                case 1:
                    SetupAnswers(wrong0, Result, wrong1);
                    break;
                case 2:
                    SetupAnswers(wrong0, wrong1, Result);
                    break;
            }
        }

        private void SetupAnswers(int answer0, int answer1, int answer2)
        {
            if (Answers == null)
            {
                Answers = new string[3];
            }

            Answers[0] = answer0.ToString();
            Answers[1] = answer1.ToString();
            Answers[2] = answer2.ToString();
        }

        private static int GetRandomValue(int minVal, int maxVal)
        {
            return Random.Next(minVal, maxVal + 1);
        }

        #endregion
    }

    public enum AnswerResult
    {
        Correct,
        Incorrect,
        Late
    }
}