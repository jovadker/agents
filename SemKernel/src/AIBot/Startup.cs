﻿// Generated with EchoBot .NET Template version v4.22.0

using AIBot.Agents;
using AIBot.Config;
using AIBot.Plugins;
using Azure;
using Azure.Identity;
using Azure.Search.Documents.Indexes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.SemanticKernel;
using System;

namespace AIBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHttpClient().AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.MaxDepth = HttpHelper.BotMessageSerializerSettings.MaxDepth;
            });

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            var storage = new MemoryStorage();
            // Create the User state passing in the storage layer.
            var userState = new UserState(storage);
            services.AddSingleton(userState);

            // Create the Conversation state passing in the storage layer.
            var conversationState = new ConversationState(storage);
            services.AddSingleton(conversationState);

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, Bots.KBAgentBot>();

            services.AddControllers();
            services.AddHttpClient();

            // Register Semantic Kernel
            var kernelBuilder = services.AddKernel();

            kernelBuilder.Services.AddLogging(services => services.AddDebug().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace));

            // Reading the appsettings.json or Azure Web App settings
            Configuration.GetSection("AIServices").Get<AIServices>();
            Configuration.GetSection("AIServices:AzureOpenAI").Get<AzureOpenAIConfig>();
            Configuration.GetSection("AIServices:AzureAISearch").Get<AzureAISearchConfig>();
            Configuration.GetSection("AIServices:AzureOpenAIEmbeddings").Get<AzureOpenAIEmbeddingsConfig>();
            Configuration.GetSection("AppCommonSettings").Get<AppCommonSettings>();

            kernelBuilder.Services.AddSingleton<SearchIndexClient>((_) => new SearchIndexClient(
                new Uri(AzureAISearchConfig.Endpoint),
                new AzureKeyCredential(AzureAISearchConfig.ApiKey),
                new Azure.Search.Documents.SearchClientOptions()));

            // Custom AzureAISearchService to configure request parameters and make a request.
            kernelBuilder.Services.AddSingleton<IAzureAISearchService, AzureAISearchService>();

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            // Embedding generation service to convert string query to vector
            kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(AzureOpenAIEmbeddingsConfig.DeploymentName,
                AzureOpenAIEmbeddingsConfig.Endpoint,
                AzureOpenAIConfig.ApiKey);
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            services.AddAzureOpenAIChatCompletion(
                deploymentName: AzureOpenAIConfig.DeploymentName,
                endpoint: AzureOpenAIConfig.Endpoint,
                apiKey: AzureOpenAIConfig.ApiKey);

            //Use the Azure CLI (for local) or Managed Identity (for Azure running app) to authenticate to the Azure OpenAI service
            /*credentials: new ChainedTokenCredential(
               new AzureCliCredential(),
               new ManagedIdentityCredential()
            )) */
            ;

            // Register Azure AI Search Plugin
            kernelBuilder.Plugins.AddFromType<AzureAISearchPlugin>();

            // Register the KBAgent
            services.AddTransient<KBAgent>();

            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
