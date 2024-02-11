
using AttachmentSaver.Configuration;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;

namespace AttachmentSaver.Application;

public class MailClient : IDisposable
{
    private readonly ImapClient _client;

    public MailClient(MailOptions mailOptions)
    {
        _client = new ImapClient();
        _client.Connect(mailOptions.Host, mailOptions.Port, true);
        _client.Authenticate(mailOptions.Username, mailOptions.Password);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    public async Task<IEnumerable<uint>> GetUnreadMailsAsync(CancellationToken cancellationToken)
    {
        var inbox = _client.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite);
        return (await inbox.SearchAsync(SearchQuery.NotSeen, cancellationToken))
            .Select(m => m.Id).ToArray();
    }

    public async Task DeleteAsync(uint mailId, CancellationToken cancellationToken)
    {
        var inbox = _client.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite);
        await inbox.AddFlagsAsync(new UniqueId(mailId), MessageFlags.Deleted, true, cancellationToken);
        await inbox.ExpungeAsync();
    }

    public async Task SaveAttachmentsAsync(uint mailId, string path, CancellationToken cancellationToken)
    {
        var mail = await _client.Inbox.GetMessageAsync(new UniqueId(mailId), cancellationToken);
        foreach (var attachment in mail.Attachments.Where(a => a.ContentDisposition != null))
        {
            var fileName = attachment.ContentDisposition.FileName;
            var target = Path.Combine(path, fileName);
            var copy = 1;
            while (File.Exists(target))
            {
                copy++;
                var i = fileName.LastIndexOf('.');
                target = Path.Combine(path, fileName[..i] + "." + copy + fileName[i..]);
            }

            var data = ((MimePart)attachment).Content;
            using (var stream = new FileStream(target, FileMode.CreateNew))
            {
                data.DecodeTo(stream, cancellationToken);
            }
        }
    }
}