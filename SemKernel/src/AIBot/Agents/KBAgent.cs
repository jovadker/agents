using AIBot.Bots;
using Azure.Search.Documents.Indexes;
using Azure;
using Microsoft.Bot.Builder;
using Microsoft.Identity.Client;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using AIBot.Plugins;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.SemanticKernel.Data;

namespace AIBot.Agents
{

    public class KBAgent
    {
        private readonly Kernel _kernel;
        private const string AgentInstructions = """
            You are a friendly assistant that helps people find information in your knowledge base.
            Consider to use your plugins to provide the best answers to the user's questions and only use the internet when necessary.
            If you don't understand the question, you can ask the user to rephrase it or ask for more details.
                        
            """;
        /*make sure to format it nicely using an adaptive card.

        Respond in JSON format with the following JSON schema:

        {
            "contentType": "'Text' or 'AdaptiveCard' only",
            "content": "{The content of the response, may be plain text, or JSON based adaptive card}"
        }
        """;*/

        /// <summary>
        /// Initializes a new instance of the <see cref="KBAgent"/> class.
        /// </summary>
        /// <param name="kernel">An instance of <see cref="Kernel"/> for interacting with an LLM.</param>
        public KBAgent(Kernel kernel)
        {
            this._kernel = kernel;
            this._kernel.Plugins.Add(KernelPluginFactory.CreateFromType<DateTimePlugin>());
            this._kernel.Plugins.Add(KernelPluginFactory.CreateFromType<BingPlugin>());
        }

        /// <summary>
        /// Invokes the agent with the given input and returns the response.
        /// </summary>
        /// <param name="input">A message to process.</param>
        /// <returns>An instance of <see cref="WeatherForecastAgentResponse"/></returns>
        public async Task<ChatMessageContent> InvokeAgentAsync(string input, List<HistoryContent> history)
        {
            var chatCompletionService = this._kernel.GetRequiredService<IChatCompletionService>();
            //Chat history for Semantic Kernel
            ChatHistory chatHistory = new ChatHistory();
            foreach (HistoryContent msg in history)
            {
                chatHistory.Add(new ChatMessageContent(msg.AuthorRole, msg.Content));
            }
            // adding to chat history for Semantic Kernel
            ChatMessageContent message = new(AuthorRole.User, input);
            // save the data in ConvesationData (bot state)
            history.Add(new HistoryContent() { AuthorRole = AuthorRole.User, Content = input });
            chatHistory.Add(message);

            // Enable planning
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                ChatSystemPrompt = AgentInstructions
            };

            var result = await chatCompletionService.GetChatMessageContentAsync(
                    chatHistory: chatHistory,
                    executionSettings: openAIPromptExecutionSettings,
                    kernel: _kernel);

            //save the result in the bot state as well  
            history.Add(new HistoryContent() { AuthorRole = result.Role, Content = result.Content });
            return result;
        }
    }

}
