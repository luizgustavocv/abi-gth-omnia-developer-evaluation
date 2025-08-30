using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Command for retrieving a sale by its unique identifier.
/// </summary>
/// <remarks>
/// This command is used to fetch a specific sale from the repository.
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
/// that returns a <see cref="GetSaleResult"/>.
/// 
/// The command requires only the sale ID and is validated using the 
/// <see cref="GetSaleValidator"/> to ensure the ID is valid.
/// </remarks>
public class GetSaleCommand : IRequest<GetSaleResult>
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to retrieve
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of GetSaleCommand
    /// </summary>
    public GetSaleCommand() { }

    /// <summary>
    /// Initializes a new instance of GetSaleCommand with the specified ID
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    public GetSaleCommand(Guid id)
    {
        Id = id;
    }

    public ValidationResultDetail Validate()
    {
        var validator = new GetSaleValidator();
        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}