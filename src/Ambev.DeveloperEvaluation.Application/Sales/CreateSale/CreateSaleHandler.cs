using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The logger instance</param>
    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request
    /// </summary>
    /// <param name="command">The CreateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var sale = new Sale
        {
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName
        };

        foreach (var itemCommand in command.Items)
        {
            var saleItem = new SaleItem(
                itemCommand.ProductId,
                itemCommand.ProductName,
                itemCommand.UnitPrice,
                itemCommand.Quantity
            );

            sale.AddItem(saleItem);
        }

        var saleValidation = sale.Validate();

        if (!saleValidation.IsValid)
        {
            var errorMessages = string.Join(", ", saleValidation.Errors.Select(e => e.Error));
            throw new ValidationException($"Sale validation failed: {errorMessages}");
        }

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        var saleCreatedEvent = new SaleCreatedEvent(createdSale);
        
        _logger.LogInformation("Sale created: {@SaleCreatedEvent}", saleCreatedEvent);

        return _mapper.Map<CreateSaleResult>(createdSale);
    }
}