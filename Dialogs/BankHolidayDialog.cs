using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using WhoIsWho.Helpers;

namespace WhoIsWho.Dialogs
{
    public class BankHolidayDialog : ComponentDialog
    {
        private readonly BankHolidayCalculator _bankHolidayCalculator;
        public BankHolidayDialog(BankHolidayCalculator bankHolidayCalculator): base(nameof(BankHolidayDialog))
        {
            _bankHolidayCalculator = bankHolidayCalculator;

            AddDialog(new DateTimePrompt(nameof(DateTimePrompt), ValidateDateTime));
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
            var dateTimeCard = await CreateDateTimeSelectorCard();


            return await stepContext.PromptAsync(
                nameof(DateTimePrompt),
                new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(new Attachment
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dateTimeCard)),
                    }),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (stepContext.Result is IList<DateTimeResolution> datetimes)
            {
                var time = datetimes.First().Timex.GetDateTime();
                var isABankHoliday = _bankHolidayCalculator.IsBankHoliday(time);
                var messageText = $"{time.ToLongDateString()} is {(isABankHoliday ? "a" : "not a")} bank holiday";
                var responseMessage = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(responseMessage, cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            throw new InvalidOperationException("Result is not datetimes");

        }

        private Task<bool> ValidateDateTime(PromptValidatorContext<IList<DateTimeResolution>> promptContext,
            CancellationToken cancellationToken)
        {
            var results =
                promptContext.Recognized?.Value?.Where(o => !string.IsNullOrEmpty(o.Timex) && o.Timex.IsDateTime());
            return Task.FromResult(results?.Any() ?? false);
        }

        private async Task<AdaptiveCard> CreateDateTimeSelectorCard()
        {
            var card = new AdaptiveCard("1.0");

            card.Body.Add(new AdaptiveTextBlock
            {
                Text = "Please select a date you would like to know about",
                Size = AdaptiveTextSize.Large,
                Weight = AdaptiveTextWeight.Lighter,
            });

            var input = new AdaptiveDateInput
            {
                Id = "Text",
                Placeholder = "Select Date"
            };

            card.Body.Add(input);

            card.Actions.Add(new AdaptiveSubmitAction
            {
                Title = "Submit",
                Data = new { input.Value }
            });

            return await Task.FromResult(card);
        }

    }
}
