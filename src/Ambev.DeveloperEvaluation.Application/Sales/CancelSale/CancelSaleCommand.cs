using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Command for cancelling a sale.
/// </summary>
/// <remarks>
/// This command is used to cancel a sale by ID.
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
/// that returns a <see cref="CancelSaleResult"/>.
/// 
/// The data provided in this command is validated using the 
/// <see cref="CancelSaleValidator"/> which extends 
/// <see cref="AbstractValidator{T}"/> to ensure that the ID is valid.
/// </remarks>
public class CancelSaleCommand : IRequest<CancelSaleResult>
{
    /// <summary>
    /// Gets or sets the ID of the sale to cancel.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the reason for cancellation.
    /// </summary>
    public string CancellationReason { get; set; } = "User cancelled the sale";

    /// <summary>
    /// Initializes a new instance of CancelSaleCommand.
    /// </summary>
    public CancelSaleCommand() { }

    /// <summary>
    /// Initializes a new instance of CancelSaleCommand with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the sale to cancel</param>
    /// <param name="cancellationReason">The reason for cancellation</param>
    public CancelSaleCommand(Guid id, string cancellationReason = "User cancelled the sale")
    {
        Id = id;
        CancellationReason = cancellationReason;
    }

    public ValidationResultDetail Validate()
    {
        var validator = new CancelSaleValidator();
        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}