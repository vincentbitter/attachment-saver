using AttachmentSaver.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MailOptions>(builder.Configuration.GetSection(MailOptions.Mail));

AttachmentSaver.Application.Setup.AddServices(builder.Services);

var app = builder.Build();

app.Run();
