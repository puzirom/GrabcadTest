namespace MathCompetition.Models
{
    public class Competitor
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int Points { get; private set; }

        public static Competitor NewCompetitor(string id, string name)
        {
            return new Competitor
            {
                Id = id, 
                Name = name, 
                Points = 0
            };
        }

        public void PointsUp()
        {
            Points++;
        }

        public void PointsDown()
        {
            if (Points > 0)
            {
                Points--;
            }
        }
    }
}