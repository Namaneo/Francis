using Francis.Database;
using Francis.Database.Entities;
using Francis.Models.Notification;
using Francis.Models.Options;
using Francis.Telegram.Client;
using Francis.Telegram.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace Francis.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly BotDbContext _context;

        private readonly ITelegramClient _client;

        private readonly IOptionsSnapshot<TelegramOptions> _options;


        public WebhookController(BotDbContext context, ITelegramClient client, IOptionsSnapshot<TelegramOptions> options)
        {
            _context = context;
            _client = client;
            _options = options;
        }


        [HttpPost]
        public async Task Webhook(Notification notification)
        {
            long.TryParse(notification.RequestId, out var requestId);

            var requestedUser = _context.BotUsers.FirstOrDefault(x => x.UserName == notification.RequestedUser);
            if (requestedUser != null && notification.Type != null && !requestedUser.WatchedItems.Any(x => x.RequestId == requestId))
            {
                requestedUser.WatchedItems.Add(WatchedItem.From(requestId, notification.Type.Value, requestedUser));
                _context.SaveChanges();
            }

            var handler = notification.NotificationType switch
            {
                NotificationType.Test => HandleTest(),
                NotificationType.NewRequest => HandleNewRequest(notification, requestId),
                NotificationType.RequestApproved => HandleRequestApproved(notification, requestId),
                NotificationType.RequestDeclined => HandleRequestDenied(notification, requestId),
                NotificationType.RequestAvailable => HandleRequestAvailable(notification, requestId),
                _ => Task.CompletedTask,
            };
            await handler;
        }

        private string FormatAnswer(Notification notification, string message)
        {
            //TODO: Messy seasons and episodes: maybe there's a way to change Ombi's behavior to send more accurate data?

            var result = $"{notification.Title} ({notification.Type} - {notification.Year})\n\n";
            if (!string.IsNullOrEmpty(notification.SeasonsList) && !string.IsNullOrEmpty(notification.EpisodesList))
            {
                result += $"Season(s) concerned: {notification.SeasonsList}\nEpisode(s) concerned: {notification.EpisodesList}\n\n";
            }
            return result + message;
        }

        private async Task HandleTest()
        {
            await _client.SendMessage(_options.Value.AdminChat, "This is a test message from Ombi! If you received this, your configuration is valid.");
        }

        private async Task HandleNewRequest(Notification notification, long requestId)
        {
            await _client.SendImage(_options.Value.AdminChat, notification.PosterImage, $"The user '{notification.RequestedUser}' has requested item: {notification.Title} ({notification.Type} - {notification.Year})", new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Approve", $"/approve_{notification.Type} {requestId}"),
                InlineKeyboardButton.WithCallbackData("Deny", $"/deny_{notification.Type} {requestId}"),
            }));
        }

        private async Task HandleRequestApproved(Notification notification, long requestId)
        {
            var users = _context.BotUsers.Where(x => x.WatchedItems.Any(x => x.RequestId == requestId) || x.Id == _options.Value.AdminChat).ToList();
            foreach (BotUser user in users)
            {
                await _client.SendMessage(user.Id, FormatAnswer(notification, "You're request has been approved. It will be available soon!"));
            }
        }

        private async Task HandleRequestDenied(Notification notification, long requestId)
        {
            var users = _context.BotUsers.Where(x => x.WatchedItems.Any(x => x.RequestId == requestId) || x.Id == _options.Value.AdminChat).ToList();
            foreach (BotUser user in users)
            {
                await _client.SendMessage(user.Id, FormatAnswer(notification, "You're request has been denied... Maybe your request doesn't match the conditions?"));
            }
        }

        private async Task HandleRequestAvailable(Notification notification, long requestId)
        {
            var users = _context.BotUsers.Where(x => x.WatchedItems.Any(x => x.RequestId == requestId) || x.Id == _options.Value.AdminChat).ToList();
            foreach (BotUser user in users)
            {
                await _client.SendMessage(user.Id, FormatAnswer(notification, "You're request is available. You can watch it now!"));
            }
        }
    }
}
