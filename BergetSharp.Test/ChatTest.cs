using System.ClientModel;
using BergetSharp.Model;
using OpenAI;
using OpenAI.Chat;

namespace BergetSharp.Test;

public class ChatTest
{
    [Test]
    public async Task CompleteChat_UserPrompt_Test()
    {
        var credentials = new ApiKeyCredential(Environment.GetEnvironmentVariable("BERGET_API_KEY") ?? "");
        var options = new OpenAIClientOptions { Endpoint = new Uri("https://api.berget.ai/v1") };

        var client = new BergetClient(credentials, options);
        var chatClient = client.GetChatClient(Models.Chat.Qwen3_8_27B_Fp8);

        var completion = await chatClient.CompleteChatAsync("What is the capital of Sweden? Answer with a single word.");

        await Assert.That(completion).IsNotNull();
        var message = completion.Value.Content.FirstOrDefault();
        await Assert.That(message?.Text).IsNotNull();
        await Assert.That(string.IsNullOrWhiteSpace(message!.Text)).IsFalse();
        await Assert.That(completion.Value.FinishReason).IsEqualTo(ChatFinishReason.Stop);
    }

    [Test]
    public async Task CompleteChat_ImageInput_Test()
    {
        var credentials = new ApiKeyCredential(Environment.GetEnvironmentVariable("BERGET_API_KEY") ?? "");
        var options = new OpenAIClientOptions { Endpoint = new Uri("https://api.berget.ai/v1") };

        var client = new BergetClient(credentials, options);
        var chatClient = client.GetChatClient(Models.Chat.Gemma4_31B_It);

        var imageFilePath = Path.Combine(AppContext.BaseDirectory, "Files", "sample-bumblebee-400x300.png");
        var imageBytes = BinaryData.FromBytes(await File.ReadAllBytesAsync(imageFilePath));

        var userMessage = new UserChatMessage(
            ChatMessageContentPart.CreateTextPart("What is in this image? Answer with a single word."),
            ChatMessageContentPart.CreateImagePart(imageBytes, "image/png"));

        var completion = await chatClient.CompleteChatAsync(userMessage);

        await Assert.That(completion).IsNotNull();
        var message = completion.Value.Content.FirstOrDefault();
        await Assert.That(message?.Text).IsNotNull();
        await Assert.That(string.IsNullOrWhiteSpace(message!.Text)).IsFalse();
    }
}
