using System;
using LibraryManagement.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Application.Events;

public class BookBorrowedEventHandler : INotificationHandler<BookBorrowedEvent>
{
    private readonly ILogger<BookBorrowedEventHandler> _logger;

    public BookBorrowedEventHandler(ILogger<BookBorrowedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(BookBorrowedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Book borrowed: BookId={BookId}, MemberId={MemberId}, DueDate={DueDate}",
            notification.BookId,
            notification.MemberId,
            notification.DueDate);
            
        // - Send email notification to member and so on ...

        return Task.CompletedTask;
    }
}
