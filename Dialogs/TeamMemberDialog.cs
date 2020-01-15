using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using WhoIsWho.Models;
using WhoIsWho.Recognizers;

namespace WhoIsWho.Dialogs
{
    public class TeamMemberDialog : ComponentDialog
    {
        private readonly LuisRecognizerImpl _luisRecognizer;
        public TeamMemberDialog(LuisRecognizerImpl luisRecognizer) : base(nameof(TeamMemberDialog))
        {
            _luisRecognizer = luisRecognizer;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
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
                              "Which team member would you like to know about?";

            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var luisResult =
                await _luisRecognizer.RecognizeAsync<WhoIsWhoLuisGenModel>(stepContext.Context, cancellationToken);

            switch (luisResult.TopIntent().intent)
            {
                case WhoIsWhoLuisGenModel.Intent.Central:
                    var centralMessageText = $"That person works in the Central Team";
                    var centralMessage = MessageFactory.Text(centralMessageText, centralMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(centralMessage, cancellationToken);
                    break;
                case WhoIsWhoLuisGenModel.Intent.Commercial:
                    var commercialMessageText = $"That person works in the Commercial Team";
                    var commercialMessage = MessageFactory.Text(commercialMessageText, commercialMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(commercialMessage, cancellationToken);
                    break;
                case WhoIsWhoLuisGenModel.Intent.Preconstruction_Operations:
                    var preConMessageText = $"That person works in the Pre-Con & Ops team";
                    var preConMessage = MessageFactory.Text(preConMessageText, preConMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(preConMessage, cancellationToken);
                    break;
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
