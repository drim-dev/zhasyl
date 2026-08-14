using FluentValidation;
using MediatR;
using Zhasyl.Api.Common.Http;

namespace Zhasyl.Api.Features.Station;

public static class GetStationOverview
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/station/overview", async (
                string? locale,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(new Request(locale ?? "ru"), cancellationToken);
                return Results.Ok(response);
            });
        }
    }

    public sealed record Request(string Locale) : IRequest<Response>;

    public sealed record Response(
        string StationId,
        string StationName,
        string Locale,
        string Location,
        string Briefing,
        IReadOnlyList<LaboratorySummary> Laboratories);

    public sealed record LaboratorySummary(
        string Id,
        string Name,
        string Purpose,
        string Specialist,
        MissionSummary FirstMission);

    public sealed record MissionSummary(
        string Id,
        string Name,
        string Problem,
        string Status);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(request => request.Locale)
                .Equal("ru")
                .WithMessage("The requested locale is not published.")
                .WithErrorCode("content:locale:read:not_published");
        }
    }

    public sealed class RequestHandler : IRequestHandler<Request, Response>
    {
        public Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new Response(
                "zhasyl-1",
                "Станция «Жасыл-1»",
                request.Locale,
                "Равнина Аркадия · Марс · 2035 год",
                "Станция готовит инфраструктуру к прибытию большой группы поселенцев. Каждая лаборатория решает реальные задачи будущего поселения.",
                [
                    new LaboratorySummary(
                        "bioinformatics",
                        "Лаборатория биоинформатики",
                        "Исследует живые системы станции с помощью данных и программирования.",
                        "Лариса Ким",
                        new MissionSummary(
                            "bioscout",
                            "BioScout: код Красной планеты",
                            "В агрокомплексе обнаружены признаки неизвестной болезни растений.",
                            "Подготовка первого задания")),
                    new LaboratorySummary(
                        "materials",
                        "Лаборатория материалов",
                        "Проектирует безопасные материалы для ремонта и расширения станции.",
                        "Зарема Дадаева",
                        new MissionSummary(
                            "sealant-17",
                            "Герметик № 17",
                            "Нужно подобрать модель состава для герметизации жилого модуля.",
                            "Подготовка первого задания"))
                ]);

            return Task.FromResult(response);
        }
    }
}
