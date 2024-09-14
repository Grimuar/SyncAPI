using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.TestsAndUtility.Request;

public class FirebaseModel
{
        [JsonPropertyName ("message")]
        public string To { get; set; }

        [JsonPropertyName ("body")]
        public NotificationModel Data { get; set; }
}