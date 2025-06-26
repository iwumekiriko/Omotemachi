using Omotemachi.Models.V1.Wacky.CCG;
using Microsoft.EntityFrameworkCore;
using System;

namespace Omotemachi.Services.Wacky;

public class PackMaintenanceService(IServiceProvider services) : BackgroundService
{
    private readonly IServiceProvider _services = services;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppContext>();
                await CreateNewPacks(context);
                await UpdateGeneralPack(context);

                await context.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
    private static async Task CreateNewPacks(AppContext context)
    {
        var seriesToProcess = await context.Cards
            .Include(c => c.Series)
            .GroupBy(c => c.Series)
            .Where(g => g.Count() >= 100)
            .Select(g => g.Key)
            .ToListAsync();

        foreach (var series in seriesToProcess)
        {
            var pack = await context.Packs
                .Include(p => p.Cards)
                .FirstOrDefaultAsync(p => p.IsSeriesSpecific && p.SpecificSeries == series);

            if (pack == null)
            {
                pack = new Pack
                {
                    Name = series!.Name,
                    IsSeriesSpecific = true,
                    SpecificSeries = series
                };
                context.Packs.Add(pack);
            }

            var cards = await context.Cards
                .Where(c => c.Series == series)
                .ToListAsync();

            pack.Cards.Clear();
            foreach (var card in cards)
            {
                pack.Cards.Add(card);
            }
        }
    }
    private static async Task UpdateGeneralPack(AppContext context)
    {
        var generalPack = await context.Packs
            .FirstOrDefaultAsync(p => !p.IsSeriesSpecific);

        if (generalPack == null)
        {
            generalPack = new Pack
            {
                Name = "General",
                IsSeriesSpecific = false
            };
            context.Packs.Add(generalPack);
        }
        else
        {
            await context.Entry(generalPack)
                .Collection(p => p.Cards)
                .LoadAsync();
        }

        var allCards = await context.Cards
            .Include(c => c.Series)
            .Where(c => c.Status == SuggestionStatus.APPROVED)
            .ToListAsync();

        generalPack.Cards.Clear();
        foreach (var card in allCards)
        {
            generalPack.Cards.Add(card);
        }
    }
}
