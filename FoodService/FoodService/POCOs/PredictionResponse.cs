namespace FoodService.POCOs
{
    public class PredictionResponse
    {
        public Prediction[] Predictions { get; set; }
    }

    public class Prediction
    {
        public float Probability { get; set; }

        public string TagName { get; set; }
    }
}