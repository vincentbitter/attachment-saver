namespace AttachmentSaver.Application;

public static class Setup
{
    public static void AddServices(IServiceCollection services)
    {
        services.AddHostedService<MailHandler>();
    }
}