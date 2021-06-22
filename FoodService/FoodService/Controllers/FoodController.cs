using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using FoodService.POCOs;
using FoodService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace FoodService.Controllers
{
    [ApiController]
    [Route("api/food")]
    public class FoodController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _clientFactory;

        private readonly ICacheService _redisService;

        private readonly JsonSerializerOptions _jsonSerializerOptions;
        
        public FoodController(IConfiguration config, IHttpClientFactory factory, ICacheService redisService)
        {
            _configuration = config;
            _clientFactory = factory;
            _redisService = redisService;
            _jsonSerializerOptions = new JsonSerializerOptions() {PropertyNameCaseInsensitive = true};
        }

        [HttpPost("predict"), Authorize(Policy = "Users")]
        public async Task<IActionResult> Predict([FromForm] IFormFile image)
        {
            byte[] imageBytes = new byte[4000000];
            var stream = image.OpenReadStream();
            stream.Read(imageBytes);
            
            var foodPredictionClient = _clientFactory.CreateClient("foodPrediction");
            var googlePlacesClient = _clientFactory.CreateClient("googlePlaces");
            
            var foodPredictionRequest = new HttpRequestMessage(HttpMethod.Post, "image");
            var form = new MultipartFormDataContent();
            form.Add(new ByteArrayContent(imageBytes, 0, imageBytes.Length), "imageData", image.FileName);

            foodPredictionRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
            foodPredictionRequest.Content = form;

            var foodPredictionResponse = await foodPredictionClient.SendAsync(foodPredictionRequest);
            var jsonContent = await foodPredictionResponse.Content.ReadAsStreamAsync();
            
            var predictionResult = await JsonSerializer.DeserializeAsync<PredictionResponse>(jsonContent, _jsonSerializerOptions);

            Prediction highestPrediction = predictionResult.Predictions[0];
            foreach (var prediction in predictionResult.Predictions)
            {
                if (prediction.Probability > highestPrediction.Probability)
                {
                    highestPrediction = prediction;
                }
            }
            
            var hashResponse =  await _redisService.GetHashAsync(highestPrediction.TagName);

            var googlePlacesRequest = new HttpRequestMessage(HttpMethod.Get,
                $"textsearch/json?key={_configuration["GoogleApiKey"]}&region=84&radius=1000&query=${hashResponse[1].Value.ToString()}");
            
            var placesResponse = await googlePlacesClient.SendAsync(googlePlacesRequest);
            
            jsonContent = await placesResponse.Content.ReadAsStreamAsync();
            var places = (await JsonSerializer.DeserializeAsync<NearbySearchResponse>(jsonContent, _jsonSerializerOptions)).Results;
            
            
            return Ok(new { vietnameseName = hashResponse[1].Value.ToString(), description = hashResponse[0].Value.ToString(), places });
        }
    }
}