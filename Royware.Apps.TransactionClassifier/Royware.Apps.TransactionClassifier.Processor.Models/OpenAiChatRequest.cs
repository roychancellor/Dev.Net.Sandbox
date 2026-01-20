using System.Text.Json.Serialization;

namespace Royware.Apps.TransactionClassifier.Processor.Models
{
    public sealed class OpenAiResponseRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "";
        
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }
        
        [JsonPropertyName("max_output_tokens")]
        public int MaxOutputTokens { get; set; }
        
        [JsonPropertyName("input")]
        public List<InputItem> Input { get; set; } = [];
    }

    public sealed class InputItem
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = ""; // "system" | "user"
        
        [JsonPropertyName("content")]
        public List<InputContent> Content { get; set; } = [];
    }

    public sealed class InputContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "input_text";
        
        [JsonPropertyName("text")]
        public string Text { get; set; } = "";
    }

}
