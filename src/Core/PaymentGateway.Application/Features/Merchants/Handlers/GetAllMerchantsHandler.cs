using MediatR;
using PaymentGateway.Application.Features.Merchants.Queries;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Repositories;

namespace PaymentGateway.Application.Features.Merchants.Handlers;

public class GetAllMerchantsHandler : IRequestHandler<GetAllMerchantsQuery, BaseResultWithData<List<GetMerchantModel>>>
{
    private readonly IMerchantServices _services;

    public GetAllMerchantsHandler(IMerchantServices services)
    {
        _services = services;
    }

    public async Task<BaseResultWithData<List<GetMerchantModel>>> Handle(GetAllMerchantsQuery request, CancellationToken cancellationToken = default)
    {
        return await _services.GetMerchants();
    }
}