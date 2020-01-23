using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace WhoIsWho.Bots
{
    public class DialogBot<T> : ActivityHandler
        where T : Dialog
    {
        protected readonly Dialog Dialog;
        protected readonly BotState ConversationState;
        protected readonly BotState UserState;

        public DialogBot(ConversationState conversationState, UserState userState, T dialog)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(turnContext.Activity.Text))
            {
                dynamic value = turnContext.Activity.Value;
                string text = value["Text"];
                text = string.IsNullOrEmpty(text) ? "." : text;
                turnContext.Activity.Text = text;
            }

            await base.OnTurnAsync(turnContext, cancellationToken);

            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }
    }
}
