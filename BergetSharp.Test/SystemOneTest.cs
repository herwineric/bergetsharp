using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using BergetSharp.Model;
using BergetSharp.Model.SystemOne;
using BergetSharp.SystemOne;
using OpenAI;

namespace BergetSharp.Test;

public class SystemOneTest
{
    private static SystemOneClient CreateClient()
    {
        // The validation tests never reach the transport, so a placeholder key is fine when
        // BERGET_API_KEY is not set; the integration test still requires a real key.
        var credentials = new ApiKeyCredential(Environment.GetEnvironmentVariable("BERGET_API_KEY") ?? "test-key");
        var options = new OpenAIClientOptions { Endpoint = new Uri("https://api.berget.ai/v1") };

        return new BergetClient(credentials, options).GetSystemOneClient(Models.SystemOne.Qwen3_5_2B);
    }

    [Test]
    public async Task SystemOne_NoulChoiceScore_Test()
    {
        var client = CreateClient();

        var questions = new Dictionary<string, SystemOneQuestion>
        {
            ["route"] = SystemOneQuestion.Choice("Which team should handle this?", new Dictionary<string, string?>
            {
                ["billing"] = "Payments",
                ["technical"] = "Bugs",
            }),
            ["refund"] = SystemOneQuestion.Noul("Is the customer asking for money back?"),
            ["urgency"] = SystemOneQuestion.Score("How urgent is this?", new[] { "Low", "Rising", "Critical" }),
        };

        var response = await client.SystemOneAsync("I was charged twice for my subscription.", questions);

        await Assert.That(response.Value.Answers.Count).IsEqualTo(questions.Count);
        await Assert.That(string.IsNullOrWhiteSpace(response.Value.Model)).IsFalse();
        await Assert.That(response.Value.Usage).IsNotNull();

        var route = response.Value.Answers["route"];
        await Assert.That(route.Type).IsEqualTo(SystemOneQuestionType.Choice);
        await Assert.That(string.IsNullOrWhiteSpace(route.Choice)).IsFalse();

        var refund = response.Value.Answers["refund"];
        await Assert.That(refund.Type).IsEqualTo(SystemOneQuestionType.Noul);
        await Assert.That(refund.Noul).IsNotNull();
        await Assert.That(refund.Noul!.Value).IsGreaterThanOrEqualTo(0.0);
        await Assert.That(refund.Noul!.Value).IsLessThanOrEqualTo(1.0);

        var urgency = response.Value.Answers["urgency"];
        await Assert.That(urgency.Type).IsEqualTo(SystemOneQuestionType.Score);
        await Assert.That(urgency.Score).IsNotNull();
    }
    
    [Test]
    public async Task SystemOne_NoulToolRouting_Test()
    {
        var client = CreateClient();

        var questions = new Dictionary<string, SystemOneQuestion>
        {
            ["tool1"] = SystemOneQuestion.Noul("Get the best waffle recipe"),
            ["tool2"] = SystemOneQuestion.Noul("Get the best european pancake recipe"),
            ["tool3"] = SystemOneQuestion.Noul("Get the best american pancake recipe"),
        };

        var response = await client.SystemOneAsync("What is the best pancake recipe?", questions);

        await Assert.That(response.Value.Answers.Count).IsEqualTo(questions.Count);
        await Assert.That(string.IsNullOrWhiteSpace(response.Value.Model)).IsFalse();
        await Assert.That(response.Value.Usage).IsNotNull();

        var results = response.Value.Answers
            .OrderByDescending(a => a.Value.Noul ?? 0.0).ToList();

        var candidates = results.Take(2)
            .Select(r => r.Key).ToList();

        await Assert.That(candidates).Contains("tool2");
        await Assert.That(candidates).Contains("tool3");
    }

    [Test]
    public async Task SystemOne_MissingState_ThrowsArgumentException_Test()
    {
        var client = CreateClient();
        var request = new SystemOneRequest
        {
            Questions = { ["q"] = SystemOneQuestion.Noul("Is this a test?") },
        };

        var exception = Assert.ThrowsExactly<ArgumentException>(() => client.SystemOne(request));
        await Assert.That(exception.Message).Contains("A state is required");
        await Assert.That(exception.ParamName).IsEqualTo(nameof(SystemOneRequest.State));
    }

    [Test]
    public async Task SystemOne_EmptyQuestions_ThrowsArgumentException_Test()
    {
        var client = CreateClient();
        var request = new SystemOneRequest { State = "some state" };

        var exception = Assert.ThrowsExactly<ArgumentException>(() => client.SystemOne(request));
        await Assert.That(exception.Message).Contains("At least one question is required");
        await Assert.That(exception.ParamName).IsEqualTo(nameof(SystemOneRequest.Questions));
    }

    [Test]
    public async Task SystemOne_TooManyQuestions_ThrowsArgumentException_Test()
    {
        var client = CreateClient();
        var request = new SystemOneRequest { State = "some state" };
        for (int i = 0; i < 65; i++)
        {
            request.Questions[$"q{i}"] = SystemOneQuestion.Noul("Is this a test?");
        }

        var exception = Assert.ThrowsExactly<ArgumentException>(() => client.SystemOne(request));
        await Assert.That(exception.Message).Contains("At most 64 questions are allowed per request, but got 65");
        await Assert.That(exception.ParamName).IsEqualTo(nameof(SystemOneRequest.Questions));
    }

    [Test]
    public async Task SystemOne_UnknownQuestionType_ThrowsArgumentException_Test()
    {
        var client = CreateClient();
        var request = new SystemOneRequest
        {
            State = "some state",
            Questions = { ["q"] = new SystemOneQuestion { Type = "boolean", Instructions = "Is this a test?" } },
        };

        var exception = Assert.ThrowsExactly<ArgumentException>(() => client.SystemOne(request));
        await Assert.That(exception.Message).Contains("Question 'q' is invalid: Unknown question type 'boolean'");
        await Assert.That(exception.ParamName).IsEqualTo(nameof(SystemOneRequest.Questions));
    }

    [Test]
    public async Task SystemOne_ChoiceWithSingleOption_ThrowsArgumentException_Test()
    {
        var client = CreateClient();
        var request = new SystemOneRequest
        {
            State = "some state",
            Questions = { ["q"] = SystemOneQuestion.Choice("Pick one", new Dictionary<string, string?> { ["only"] = null }) },
        };

        var exception = Assert.ThrowsExactly<ArgumentException>(() => client.SystemOne(request));
        await Assert.That(exception.Message).Contains("Question 'q' is invalid: Choice questions require 2-255 options, but criteria contains 1");
        await Assert.That(exception.ParamName).IsEqualTo(nameof(SystemOneRequest.Questions));
    }

    [Test]
    public async Task SystemOne_ScoreWithTooManyLevels_ThrowsArgumentException_Test()
    {
        var client = CreateClient();
        var levels = Enumerable.Range(1, 11).Select(i => $"Level {i}").ToList();
        var request = new SystemOneRequest
        {
            State = "some state",
            Questions = { ["q"] = SystemOneQuestion.Score("Rate it", levels) },
        };

        var exception = Assert.ThrowsExactly<ArgumentException>(() => client.SystemOne(request));
        await Assert.That(exception.Message).Contains("Question 'q' is invalid: Score questions require 2-10 levels, but criteria contains 11");
        await Assert.That(exception.ParamName).IsEqualTo(nameof(SystemOneRequest.Questions));
    }

    [Test]
    public async Task SystemOne_WhitespaceModel_ThrowsArgumentException_Test()
    {
        var client = CreateClient();
        var request = new SystemOneRequest
        {
            Model = "   ",
            State = "some state",
            Questions = { ["q"] = SystemOneQuestion.Noul("Is this a test?") },
        };

        var exception = Assert.ThrowsExactly<ArgumentException>(() => client.SystemOne(request));
        await Assert.That(exception.Message).Contains("Model must not be empty when set");
        await Assert.That(exception.ParamName).IsEqualTo(nameof(SystemOneRequest.Model));
    }

    [Test]
    public async Task SystemOneRequest_SerializesToExpectedJson_Test()
    {
        var request = new SystemOneRequest
        {
            State = "I was charged twice for my subscription.",
            Questions =
            {
                ["refund"] = SystemOneQuestion.Noul("Is the customer asking for money back?"),
                ["route"] = SystemOneQuestion.Choice("Which team should handle this?", new Dictionary<string, string?>
                {
                    ["billing"] = "Payments",
                    ["technical"] = "Bugs",
                }),
            },
        };

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(request, options));
        var root = document.RootElement;

        await Assert.That(root.TryGetProperty("state", out _)).IsTrue();
        await Assert.That(root.TryGetProperty("questions", out _)).IsTrue();
        await Assert.That(root.TryGetProperty("model", out _)).IsFalse();

        var refund = root.GetProperty("questions").GetProperty("refund");
        await Assert.That(refund.GetProperty("type").GetString()).IsEqualTo("noul");
        await Assert.That(refund.TryGetProperty("criteria", out _)).IsFalse();

        var route = root.GetProperty("questions").GetProperty("route");
        await Assert.That(route.GetProperty("type").GetString()).IsEqualTo("choice");
        await Assert.That(route.GetProperty("criteria").GetProperty("billing").GetString()).IsEqualTo("Payments");
    }
}
