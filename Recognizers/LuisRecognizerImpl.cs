using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using WhoIsWho.Constants;

namespace WhoIsWho.Recognizers
{
    public class LuisRecognizerImpl : IRecognizer
    {
        private readonly LuisRecognizer _recognizer;

        public LuisRecognizerImpl(IConfiguration configuration)
        {
            _recognizer =
                new LuisRecognizer(
                    "https://westeurope.api.cognitive.microsoft.com/luis/v2.0/apps/93c9b179-b142-4fc2-a41a-f96ef9a7033f?verbose=true&timezoneOffset=0&subscription-key=5161547ab4fe44a684f38f2b2b2c556a&q=");
        }

        public virtual async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
            => await _recognizer.RecognizeAsync(turnContext, cancellationToken);

        public virtual async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken)
            where T : IRecognizerConvert, new()
            => await _recognizer.RecognizeAsync<T>(turnContext, cancellationToken);
    }
}
