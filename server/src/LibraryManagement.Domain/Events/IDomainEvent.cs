using MediatR;

namespace LibraryManagement.Domain.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
