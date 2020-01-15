using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using WhoIsWho.Models;
using WhoIsWho.Recognizers;

namespace WhoIsWho.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly LuisRecognizerImpl _luisRecognizer;

        public MainDialog(LuisRecognizerImpl luisRecognizer, ApplicationDialog applicationDialog, TeamMemberDialog teamMemberDialog) : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(applicationDialog);
            AddDialog(teamMemberDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var messageText = stepContext.Options?.ToString() ??
                              "What can I help you with today?\nSay something like \"I want to know what team someone works in? Or, I want to know which team to speak to regarding an application?\"";

            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions {Prompt = promptMessage},
                cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var luisResult =
                await _luisRecognizer.RecognizeAsync<WhoIsWhoLuisGenModel>(stepContext.Context, cancellationToken);

            switch (luisResult.TopIntent().intent)
            {
                case WhoIsWhoLuisGenModel.Intent.application:
                    return await stepContext.BeginDialogAsync(nameof(ApplicationDialog), null, cancellationToken);
                case WhoIsWhoLuisGenModel.Intent.team_member:
                    return await stepContext.BeginDialogAsync(nameof(TeamMemberDialog), null, cancellationToken);
                case WhoIsWhoLuisGenModel.Intent.None:
                default:
                    var didntUnderstandMessageText =
                        $"Sorry, I didn't get that. Please try asking in a different way (intent was {luisResult.TopIntent().intent})";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText,
                        didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }
    }

}
