using Francis.Telegram.Extensions;
using Francis.Telegram.Contexts;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Francis.Telegram.Answers.MessageAnswers
{
    public class HelpAnswer : TelegramAnswer
    {
        internal override bool CanProcess => Context.Command == "/help" || Context.Command == "/start";


        public HelpAnswer(MessageAnswerContext context) : base(context)
        { }


        public override async Task Execute()
        {
            await Context.Bot.SendMessage(Context.Message.Chat, @"
Francis, at your service!

/help - Display this message
/disk - Get remaining free space

Any other message will be considered as a search!
");

            Context.Logger.LogInformation($"User '{Context.User.UserName}' requested help");
        }
    }
}
