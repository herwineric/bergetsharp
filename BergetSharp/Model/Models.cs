namespace BergetSharp.Model;

/// <summary>
/// Model IDs available on the Berget.AI inference API, grouped by model type.
/// Source: <c>GET https://api.berget.ai/v1/models</c>.
/// </summary>
public static class Models
{
    /// <summary>
    /// Chat completion (text) models.
    /// </summary>
    public static class Chat
    {
        public const string Qwen3_8_27B_Fp8 = "Qwen/Qwen3.8-27B-FP8";
        public const string MistralSmall3_2_24B_Instruct_2506 = "mistralai/Mistral-Small-3.2-24B-Instruct-2506";
        public const string Glm5_3_Flash = "zai-org/GLM-5.3-Flash";
        public const string KimiK3 = "moonshotai/Kimi-K3";
        public const string Gemma4_31B_It = "google/gemma-4-31B-it";
    }

    /// <summary>
    /// Embedding models.
    /// </summary>
    public static class Embedding
    {
        public const string MultilingualE5LargeInstruct = "intfloat/multilingual-e5-large-instruct";
        public const string MultilingualE5Large = "intfloat/multilingual-e5-large";
    }

    /// <summary>
    /// Speech-to-text (transcription) models.
    /// </summary>
    public static class SpeechToText
    {
        public const string KbWhisperLarge = "KBLab/kb-whisper-large";
        public const string NbWhisperLarge = "NbAiLab/nb-whisper-large";
        public const string FasterWhisperLargeV3 = "Systran/faster-whisper-large-v3";
    }

    /// <summary>
    /// Rerank models.
    /// </summary>
    public static class Rerank
    {
        public const string BgeRerankerV2M3 = "BAAI/bge-reranker-v2-m3";
    }
}
