using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handler for processing CancelSaleCommand requests.
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CancelSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="logger">The logger instance</param>
    public CancelSaleHandler(ISaleRepository saleRepository, ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CancelSaleCommand request.
    /// </summary>
    /// <param name="request">The CancelSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the cancel operation</returns>
    public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        if (sale.Status == Domain.Enums.SaleStatus.Cancelled)
        {
            throw new InvalidOperationException("Sale has already been cancelled");
        }

        sale.Cancel();

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        var saleCancelledEvent = new SaleCancelledEvent(sale, request.CancellationReason);

        _logger.LogInformation("Sale cancelled: {@SaleCancelledEvent}", saleCancelledEvent);

        foreach (var item in sale.Items)
        {
            var saleItemCancelledEvent = new SaleItemCancelledEvent(item, request.CancellationReason);
            _logger.LogInformation("Sale item cancelled: {@saleItemCancelledEvent}", saleItemCancelledEvent);
        }

        return new CancelSaleResult
        {
            Success = true,
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            Message = "Sale has been cancelled successfully",
            CancelledAt = sale.UpdatedAt ?? DateTime.UtcNow
        };
    }
}