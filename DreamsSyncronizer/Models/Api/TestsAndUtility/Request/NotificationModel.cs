using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.TestsAndUtility.Request;

public class NotificationModel
{
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("data")]
        public string Body { get; set; }
}