using System;
using LibraryManagement.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Application.Events;

public class BookReturnedEventHandler : INotificationHandler<BookReturnedEvent>
{
    private readonly ILogger<BookReturnedEventHandler> _logger;

    public BookReturnedEventHandler(ILogger<BookReturnedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(BookReturnedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Book returned: BookId={BookId}, MemberId={MemberId}, ReturnDate={ReturnDate}, Fine={FineAmount}, WasOverdue={WasOverdue}",
            notification.BookId,
            notification.MemberId,
            notification.ReturnDate,
            notification.FineAmount,
            notification.WasOverdue);

        if (notification.WasOverdue)
        {
            _logger.LogWarning(
                "Book was returned overdue with fine: BookId={BookId}, MemberId={MemberId}, Fine={FineAmount}",
                notification.BookId,
                notification.MemberId,
                notification.FineAmount);
        }

        // - Send return confirmation email

        return Task.CompletedTask;
    }
}
