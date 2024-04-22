using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using MyCode_Backend_Server.Data;

namespace MyCode_Backend_Server.Service.Bot
{
    public class FAQBot(DataContext dataContext) : ActivityHandler
    {
        private readonly DataContext _dataContext = dataContext;

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var message = turnContext.Activity.Text;
            var faq = _dataContext.BotDb!.FirstOrDefault(q => message.Contains(q.Question!));

            if (faq != null)
            {
                var replyText = faq.Answer;
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, I couldn't find an answer to your question."), cancellationToken);
            }
        }
    }
}
