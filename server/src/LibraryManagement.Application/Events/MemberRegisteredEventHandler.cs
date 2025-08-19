using LibraryManagement.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Application.Events;

public class MemberRegisteredEventHandler : INotificationHandler<MemberRegisteredEvent>
{
    private readonly ILogger<MemberRegisteredEventHandler> _logger;

    public MemberRegisteredEventHandler(ILogger<MemberRegisteredEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(MemberRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "New member registered: MemberId={MemberId}, MembershipNumber={MembershipNumber}, Email={Email}, Name={FullName}",
            notification.MemberId,
            notification.MembershipNumber,
            notification.Email,
            notification.FullName);

        // - Send welcome email with library card details

        return Task.CompletedTask;
    }
}