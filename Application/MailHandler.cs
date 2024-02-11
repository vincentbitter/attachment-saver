

using AttachmentSaver.Configuration;
using Microsoft.Extensions.Options;

namespace AttachmentSaver.Application;

public class MailHandler : BackgroundService
{
    private readonly IOptions<MailOptions> _mailOptions;

    public MailHandler(IOptions<MailOptions> mailOptions)
    {
        _mailOptions = mailOptions;
    }

    protected async override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await HandleMails(cancellationToken);
            await Task.Delay(60000, cancellationToken);
        }
    }

    private async Task HandleMails(CancellationToken cancellationToken)
    {
        try
        {
            using var client = new MailClient(_mailOptions.Value);
            var newMails = await client.GetUnreadMailsAsync(cancellationToken);
            foreach (var mailId in newMails)
            {
                await client.SaveAttachmentsAsync(mailId, _mailOptions.Value.TargetFolder, cancellationToken);
                await client.DeleteAsync(mailId, cancellationToken);
            }
        }
        catch
        {

        }
    }
}