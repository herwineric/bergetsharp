using System.ClientModel;
using BergetSharp.Model;
using OpenAI;
using OpenAI.Audio;

namespace BergetSharp.Test;

public class AudioTranscribeTest
{
    [Test]
    public async Task TranscribeAudio_SampleSpeech_Test()
    {
        var credentials = new ApiKeyCredential(Environment.GetEnvironmentVariable("BERGET_API_KEY") ?? "");
        var options = new OpenAIClientOptions { Endpoint = new Uri("https://api.berget.ai/v1") };

        var client = new BergetClient(credentials, options);
        var audioClient = client.GetAudioClient(Models.SpeechToText.FasterWhisperLargeV3);

        var audioFilePath = Path.Combine(AppContext.BaseDirectory, "Files", "sample-speech-1m.mp3");

        var result = await audioClient.TranscribeAudioAsync(audioFilePath);

        await Assert.That(result).IsNotNull();
        await Assert.That(string.IsNullOrWhiteSpace(result.Value.Text)).IsFalse();
    }
}
