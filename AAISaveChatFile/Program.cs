using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace AAISaveChatFile
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Setup
            var key = Environment.GetEnvironmentVariable("AzureOpenAI-Key");
            var endpoint = new Uri("https://systementoropenaiinstance.openai.azure.com/");
            var deploymentName = "Systementor-o4-mini";
            var client = new AzureOpenAIClient(endpoint, new AzureKeyCredential(key));

            ChatClient chatClient = client.GetChatClient(deploymentName);

            List<ChatMessage> messages = new List<ChatMessage>()
            {
                new SystemChatMessage("Du är en hjälpsam assistant."),
                new UserChatMessage("Mitt namn är Richard."),
            };

            Console.WriteLine("Skriv 'exit' för att avsluta");

            // Start the chat loop
            while (true)
            {
                Console.Write("Q: ");
                var userInput = Console.ReadLine();

                if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (string.IsNullOrEmpty(userInput)) break;
                messages.Add(new UserChatMessage(userInput));
                var response = chatClient.CompleteChat(messages);

                var reply = response.Value.Content[0].Text;
                messages.Add(new AssistantChatMessage(reply));

                Console.WriteLine("A: " + response.Value.Content[0].Text);
            }

            // Save chat to file
            var chatFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\ChatFiles"));
            Directory.CreateDirectory(chatFolder);

            var filePath = Path.Combine(
                chatFolder,
                $"chat_{DateTime.Now:yyyyMMdd_HHmm}.txt");

            File.WriteAllLines(filePath, messages.Select(m => m.Content[0].Text));

            Console.WriteLine($"Chathistorik sparad till {filePath}");
        }
    }
}
