namespace FoodService.POCOs
{
    public class NearbySearchResponse
    {
        public Result[] Results { get; set; }
    }

    public class Result
    {
        public string Formatted_Address { get; set; }

        public string Name { get; set; }

        public string Place_Id { get; set; }

        public float Rating { get; set; }
    }
}