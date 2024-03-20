using MediatR;
using PaymentGateway.Domain.Common.ResponseBase;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Features.Merchants.Queries;

public record GetAllMerchantsQuery : IRequest<BaseResultWithData<List<GetMerchantModel>>>;
