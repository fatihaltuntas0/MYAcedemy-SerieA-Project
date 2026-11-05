using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Common;
using SerieA.API.Context;
using SerieA.API.DTOs.Matches.Events;
using SerieA.API.Entities;
using SerieA.API.Enums;

namespace SerieA.API.Services.MatchEvents
{
    public class MatchEventService(ApiContext _context, IMapper _mapper) : IMatchEventService
    {
        private static int ParseMinute(string minute)
        {
            if (string.IsNullOrWhiteSpace(minute)) return 0;
            var parts = minute.Trim().Split('+');
            if (int.TryParse(parts[0], out var baseMin))
            {
                var extra = parts.Length > 1 && int.TryParse(parts[1], out var ext) ? ext : 0;
                return baseMin * 100 + extra;
            }
            return 0;
        }

        public async Task<ApiResponse<Guid>> AddCardAsync(Guid matchId, CreateCardDto createCardDto)
        {
            var match = await _context.Matches.AsNoTracking().FirstOrDefaultAsync(m => m.Id == matchId);
            if (match is null)
            {
                return ApiResponse<Guid>.Fail("Olay eklenecek karşılaşma bulunamadı.");
            }

            if (createCardDto.TeamId != match.HomeTeamId && createCardDto.TeamId != match.AwayTeamId)
            {
                return ApiResponse<Guid>.Fail("Kartı gören takım, bu karşılaşmanın ev sahibi veya deplasman takımı olmalıdır.");
            }

            var playerName = createCardDto.PlayerName.Trim();
            var cardMin = ParseMinute(createCardDto.Minute);

            // Domain Rule: A player who was sent off with a red card cannot receive another card
            var redCard = await _context.MatchCards
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.MatchId == matchId && c.PlayerName.ToLower() == playerName.ToLower() && c.CardType == CardType.Red);

            if (redCard != null)
            {
                return ApiResponse<Guid>.Fail($"'{playerName}' daha önce {redCard.Minute}. dakikada kırmızı kart gördüğü için tekrar kart göremez.");
            }

            // Domain Rule: A player cannot receive more than 2 yellow cards in the same match
            if (createCardDto.CardType == CardType.Yellow)
            {
                var yellowCount = await _context.MatchCards
                    .AsNoTracking()
                    .CountAsync(c => c.MatchId == matchId && c.PlayerName.ToLower() == playerName.ToLower() && c.CardType == CardType.Yellow);

                if (yellowCount >= 2)
                {
                    return ApiResponse<Guid>.Fail($"'{playerName}' bu müsabakada zaten 2 sarı kart görmüştür.");
                }
            }

            // Domain Rule: A substituted-out player cannot receive an on-pitch card after leaving
            var subOut = await _context.Substitutions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.MatchId == matchId && s.PlayerOut.ToLower() == playerName.ToLower());

            if (subOut != null && cardMin > ParseMinute(subOut.Minute))
            {
                return ApiResponse<Guid>.Fail($"'{playerName}' {subOut.Minute}. dakikada oyundan çıktığı için bu dakikada ({createCardDto.Minute}') sahada kart göremez.");
            }

            var card = _mapper.Map<MatchCard>(createCardDto);
            card.MatchId = matchId;

            await _context.MatchCards.AddAsync(card);
            await _context.SaveChangesAsync();

            return ApiResponse<Guid>.Success(card.Id, "Kart kaydı başarıyla eklendi.");
        }

        public async Task<ApiResponse<Guid>> AddGoalAsync(Guid matchId, CreateGoalDto createGoalDto)
        {
            var match = await _context.Matches.AsNoTracking().FirstOrDefaultAsync(m => m.Id == matchId);
            if (match is null)
            {
                return ApiResponse<Guid>.Fail("Olay eklenecek karşılaşma bulunamadı.");
            }

            if (createGoalDto.TeamId != match.HomeTeamId && createGoalDto.TeamId != match.AwayTeamId)
            {
                return ApiResponse<Guid>.Fail("Golü atan takım, bu karşılaşmanın ev sahibi veya deplasman takımı olmalıdır.");
            }

            var playerName = createGoalDto.PlayerName.Trim();
            var goalMin = ParseMinute(createGoalDto.Minute);

            // Domain Rule 1: A red-carded player cannot score a goal
            var redCard = await _context.MatchCards
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.MatchId == matchId && c.PlayerName.ToLower() == playerName.ToLower() && c.CardType == CardType.Red);

            if (redCard != null && goalMin >= ParseMinute(redCard.Minute))
            {
                return ApiResponse<Guid>.Fail($"'{playerName}' {redCard.Minute}. dakikada kırmızı kart gördüğü için bu dakikada ({createGoalDto.Minute}') gol atamaz.");
            }

            // Domain Rule 2: A player substituted out cannot score in subsequent minutes
            var subOut = await _context.Substitutions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.MatchId == matchId && s.PlayerOut.ToLower() == playerName.ToLower());

            if (subOut != null && goalMin >= ParseMinute(subOut.Minute))
            {
                return ApiResponse<Guid>.Fail($"'{playerName}' {subOut.Minute}. dakikada oyundan çıktığı için bu dakikada ({createGoalDto.Minute}') gol atamaz.");
            }

            // Domain Rule 3: A substituted-in player cannot score before entering the pitch
            var subIn = await _context.Substitutions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.MatchId == matchId && s.PlayerIn.ToLower() == playerName.ToLower());

            if (subIn != null && goalMin < ParseMinute(subIn.Minute))
            {
                return ApiResponse<Guid>.Fail($"'{playerName}' henüz oyuna girmediği için ({createGoalDto.Minute}') gol atamaz. (Oyuna girdiği dakika: {subIn.Minute}').");
            }

            var goal = _mapper.Map<MatchGoal>(createGoalDto);
            goal.MatchId = matchId;

            await _context.MatchGoals.AddAsync(goal);
            await _context.SaveChangesAsync();

            return ApiResponse<Guid>.Success(goal.Id, "Gol kaydı başarıyla eklendi.");
        }

        public async Task<ApiResponse<Guid>> AddSubstitutionAsync(Guid matchId, CreateSubstitutionDto createSubstitutionDto)
        {
            var match = await _context.Matches.AsNoTracking().FirstOrDefaultAsync(m => m.Id == matchId);

            if (match is null)
            {
                return ApiResponse<Guid>.Fail("Olay eklenecek karşılaşma bulunamadı.");
            }

            if (createSubstitutionDto.TeamId != match.HomeTeamId && createSubstitutionDto.TeamId != match.AwayTeamId)
            {
                return ApiResponse<Guid>.Fail("Değişiklik yapan takım, bu karşılaşmanın ev sahibi veya deplasman takımı olmalıdır.");
            }

            var playerIn = createSubstitutionDto.PlayerIn.Trim();
            var playerOut = createSubstitutionDto.PlayerOut.Trim();
            var subMin = ParseMinute(createSubstitutionDto.Minute);

            if (string.Equals(playerIn, playerOut, StringComparison.OrdinalIgnoreCase))
            {
                return ApiResponse<Guid>.Fail("Oyuna giren futbolcu ile oyundan çıkan futbolcu aynı olamaz.");
            }

            // Domain Rule 1: Substituted-out player cannot have already been substituted out earlier
            var alreadyOut = await _context.Substitutions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.MatchId == matchId && s.PlayerOut.ToLower() == playerOut.ToLower());

            if (alreadyOut != null)
            {
                return ApiResponse<Guid>.Fail($"'{playerOut}' daha önce {alreadyOut.Minute}. dakikada zaten oyundan çıkmıştır.");
            }

            // Domain Rule 2: Substituted-in player cannot have already entered the match earlier
            var alreadyIn = await _context.Substitutions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.MatchId == matchId && s.PlayerIn.ToLower() == playerIn.ToLower());

            if (alreadyIn != null)
            {
                return ApiResponse<Guid>.Fail($"'{playerIn}' daha önce {alreadyIn.Minute}. dakikada zaten oyuna girmiştir.");
            }

            // Domain Rule 3: A red-carded player cannot be substituted off (the team plays with 10 players)
            var redCardOut = await _context.MatchCards
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.MatchId == matchId && c.PlayerName.ToLower() == playerOut.ToLower() && c.CardType == CardType.Red);

            if (redCardOut != null && subMin >= ParseMinute(redCardOut.Minute))
            {
                return ApiResponse<Guid>.Fail($"'{playerOut}' {redCardOut.Minute}. dakikada kırmızı kart gördüğü için oyundan değiştirilemez.");
            }

            // Domain Rule 4: A red-carded player cannot enter the match
            var redCardIn = await _context.MatchCards
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.MatchId == matchId && c.PlayerName.ToLower() == playerIn.ToLower() && c.CardType == CardType.Red);

            if (redCardIn != null)
            {
                return ApiResponse<Guid>.Fail($"'{playerIn}' kırmızı kart gördüğü için oyuna giremez.");
            }

            var subs = _mapper.Map<Substitution>(createSubstitutionDto);
            subs.MatchId = matchId;

            await _context.Substitutions.AddAsync(subs);
            await _context.SaveChangesAsync();

            return ApiResponse<Guid>.Success(subs.Id, "Oyuncu değişikliği kaydı başarıyla eklendi.");
        }

        public async Task<ApiResponse> DeleteCardAsync(Guid cardId)
        {
            var card = await _context.MatchCards.FindAsync(cardId);
            if (card is null)
            {
                return ApiResponse<Guid>.Fail("Silinecek kart kaydı bulunamadı.");
            }

            _context.MatchCards.Remove(card);
            await _context.SaveChangesAsync();

            return ApiResponse<Guid>.Success("Kart kaydı başarıyla silindi.");
        }

        public async Task<ApiResponse> DeleteGoalAsync(Guid goalId)
        {
            var goal = await _context.MatchGoals.FindAsync(goalId);

            if (goal is null)
            {
                return ApiResponse<Guid>.Fail("Silinecek gol kaydı bulunamadı.");
            }

            _context.MatchGoals.Remove(goal);
            await _context.SaveChangesAsync();

            return ApiResponse<Guid>.Success("Gol kaydı başarıyla silindi.");
        }

        public async Task<ApiResponse> DeleteSubstitutionAsync(Guid subId)
        {
            var subs = await _context.Substitutions.FindAsync(subId);

            if (subs is null)
            {
                return ApiResponse<Guid>.Fail("Silinecek değişiklik kaydı bulunamadı.");
            }

            _context.Substitutions.Remove(subs);
            await _context.SaveChangesAsync();

            return ApiResponse<Guid>.Success("Değişiklik kaydı başarıyla silindi.");
        }
    }
}
