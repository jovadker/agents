// Generated with EchoBot .NET Template version v4.22.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AIBot.Agents;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIBot.Bots
{
    /// <summary>
    /// Vadkerti's EchoBot
    /// </summary>
    public class KBAgentBot : TeamsActivityHandler
    {
        private BotState _conversationState;
        private BotState _userState;
        private KBAgent m_KBAgent;


        public KBAgentBot(KBAgent kBAgent, ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState;
            _userState = userState;
            this.m_KBAgent = kBAgent;
        }

        /// <summary>
        /// Save the state of the conversation and user data in every turn
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }


        /// <summary>
        /// Call Copilot Studio chatbot via DirectLine API to get the response
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            if (turnContext.Activity.Attachments != null && turnContext.Activity.Attachments.Any())
            {
                foreach (var attachment in turnContext.Activity.Attachments)
                {
                    // Process each attachment
                    var fileName = attachment.Name;
                    var fileContentUrl = attachment.ContentUrl;

                    // You can download the file content using the ContentUrl
                    // For example, using HttpClient
                    using (var httpClient = new HttpClient())
                    {
                        var fileContent = await httpClient.GetByteArrayAsync(fileContentUrl);
                        // Handle the file content as needed
                    }

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Received file: {fileName}"), cancellationToken);
                }
            }
            else
            {
                //get the state from memory storage
                var accessor = _conversationState.CreateProperty<ConversationData>("Data");
                var conversationData = await accessor.GetAsync(turnContext, () => new ConversationData());
                if (conversationData.ChatHistory == null)
                {
                    conversationData.ChatHistory = new List<HistoryContent>();
                }
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();

                string userMessage = turnContext.Activity.Text;

                var answer = await this.m_KBAgent.InvokeAgentAsync(userMessage, conversationData.ChatHistory);

                stopwatch.Stop();

                //send back to client
                var replyText = answer.Content;
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
                await turnContext.SendActivityAsync(MessageFactory.Text($"Time: {stopwatch.ElapsedMilliseconds} ms"), cancellationToken);
            }

        }

        /// <summary>
        /// New member joins to the conversation
        /// </summary>
        /// <param name="membersAdded"></param>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        /// <summary>
        /// Conversation ends
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task OnEndOfConversationActivityAsync(ITurnContext<IEndOfConversationActivity> turnContext, CancellationToken cancellationToken)
        {
            return base.OnEndOfConversationActivityAsync(turnContext, cancellationToken);
        }

        
    }
}
