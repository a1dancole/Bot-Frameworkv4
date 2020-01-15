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

            var luisApplication = new LuisApplication(
                configuration[LuisConfigurationConstants.ApplicationId],
                configuration[LuisConfigurationConstants.EndPointKey],
                "https://" + configuration[LuisConfigurationConstants.EndPoint]);

            _recognizer =
                new LuisRecognizer(
                    "");
        }

        public virtual async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
            => await _recognizer.RecognizeAsync(turnContext, cancellationToken);

        public virtual async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken)
            where T : IRecognizerConvert, new()
            => await _recognizer.RecognizeAsync<T>(turnContext, cancellationToken);
    }
}
