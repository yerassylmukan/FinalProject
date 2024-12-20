﻿namespace ApplicationCore.Interfaces;

public interface IEmailSender
{
    Task EmailSendAsync(string email, string subject, string message, CancellationToken cancellationToken);

    Task SendSupportAsync(string firstName, string lastName, string subject, string message,
        CancellationToken cancellationToken);

    Task EmailSendByUserIdAsync(string userId, string subject, string message, CancellationToken cancellationToken);
}