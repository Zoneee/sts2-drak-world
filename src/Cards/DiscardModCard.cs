using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

public abstract class DiscardModCard : CustomCardModel
{
    private static readonly Lazy<IReadOnlyList<Type>> CardTypes = new(() => typeof(DiscardModCard).Assembly
        .GetTypes()
        .Where(type => !type.IsAbstract && typeof(DiscardModCard).IsAssignableFrom(type))
        .OrderBy(type => type.Name)
        .ToArray());

    private readonly string assetName;

    protected DiscardModCard(int cost, CardType type, CardRarity rarity, TargetType target, string assetName,
        bool showInCardLibrary = true)
        : base(cost, type, rarity, target, showInCardLibrary)
    {
        this.assetName = assetName;
    }

    public static IReadOnlyList<Type> AllCardTypes => CardTypes.Value;

    public override string PortraitPath => BuildGodotResourcePath(assetName);

    public override string BetaPortraitPath => PortraitPath;

    public override string CustomPortraitPath => BuildGodotResourcePath(assetName, "big");

    private static string BuildGodotResourcePath(string assetName, string? variant = null)
    {
        return string.IsNullOrEmpty(variant)
            ? $"{DiscardModMain.ModId}/images/card_portraits/{assetName}.png"
            : $"{DiscardModMain.ModId}/images/card_portraits/{variant}/{assetName}.png";
    }

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (!ReferenceEquals(card, this))
        {
            return;
        }

        LogLifecycle("discard-trigger:start", $"owner={Owner}; currentTarget={CurrentTarget}");
        await OnSelfDiscarded(choiceContext);
        LogLifecycle("discard-trigger:end", $"owner={Owner}; currentTarget={CurrentTarget}");
    }

    protected virtual Task OnSelfDiscarded(PlayerChoiceContext choiceContext) => Task.CompletedTask;

    protected void LogPlay(CardPlay cardPlay, string details)
    {
        LogLifecycle("play", $"target={DescribeCreature(cardPlay.Target)}; pile={cardPlay.ResultPile}; {details}");
    }

    protected void LogLifecycle(string stage, string details)
    {
        DiscardModMain.Logger.Info($"[{GetType().Name}] {stage} | {details}");
    }

    protected Creature GetRequiredTarget(CardPlay cardPlay)
    {
        return cardPlay.Target ?? throw new InvalidOperationException($"{GetType().Name} expected a target, but CardPlay.Target was null.");
    }

    protected IReadOnlyList<Creature> GetHittableEnemies()
    {
        var enemies = Owner.Creature.CombatState?.HittableEnemies;
        return enemies == null ? Array.Empty<Creature>() : enemies.ToList();
    }

    protected Creature? GetRandomEnemy()
    {
        var enemies = GetHittableEnemies();
        if (enemies.Count == 0)
        {
            return null;
        }

        return Owner.RunState.Rng.CombatTargets.NextItem<Creature>(enemies);
    }

    protected async Task AttackRandomEnemy(PlayerChoiceContext choiceContext, decimal damage)
    {
        var target = GetRandomEnemy();
        if (target == null)
        {
            LogLifecycle("random-attack:skip", "no hittable enemies");
            return;
        }

        LogLifecycle("random-attack", $"target={DescribeCreature(target)}; damage={damage}");
        await CommonActions.CardAttack(this, target, damage).Execute(choiceContext);
    }

    protected async Task AttackAllEnemies(PlayerChoiceContext choiceContext, decimal damage)
    {
        var enemies = GetHittableEnemies();
        if (enemies.Count == 0)
        {
            LogLifecycle("aoe-attack:skip", "no hittable enemies");
            return;
        }

        LogLifecycle("aoe-attack", $"targets={enemies.Count}; damage={damage}");
        foreach (var enemy in enemies)
        {
            await CommonActions.CardAttack(this, enemy, damage).Execute(choiceContext);
        }
    }

    protected async Task ApplyToAllEnemies<T>(decimal amount)
        where T : PowerModel
    {
        var enemies = GetHittableEnemies();
        if (enemies.Count == 0)
        {
            LogLifecycle("apply-all:skip", $"power={typeof(T).Name}; no hittable enemies");
            return;
        }

        LogLifecycle("apply-all", $"power={typeof(T).Name}; targets={enemies.Count}; amount={amount}");
        foreach (var enemy in enemies)
        {
            await CommonActions.Apply<T>(enemy, this, amount);
        }
    }

    protected async Task<IEnumerable<CardModel>> DrawCards(PlayerChoiceContext choiceContext, int count)
    {
        LogLifecycle("draw", $"count={count}");
        return await CardPileCmd.Draw(choiceContext, count, Owner);
    }

    protected async Task<IReadOnlyList<CardModel>> DiscardFromHand(PlayerChoiceContext choiceContext, int count)
    {
        if (count <= 0)
        {
            return Array.Empty<CardModel>();
        }

        var selectedCards = await CardSelectCmd.FromHandForDiscard(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, count),
            null,
            this);

        if (selectedCards == null)
        {
            LogLifecycle("discard-select", $"count={count}; selection returned null");
            return Array.Empty<CardModel>();
        }

        var cards = selectedCards.ToArray();
        LogLifecycle("discard-select", $"requested={count}; selected={cards.Length}; cards={string.Join(", ", cards.Select(DescribeCard))}");

        if (cards.Length > 0)
        {
            await CardCmd.Discard(choiceContext, cards);
        }

        return cards;
    }

    protected async Task GainFlatBlock(decimal amount)
    {
        LogLifecycle("gain-block", $"amount={amount}");
        await CreatureCmd.GainBlock(Owner.Creature, new BlockVar(amount, ValueProp.Move), null);
    }

    protected static string DescribeCard(CardModel? card)
    {
        return card == null ? "<null>" : $"{card.GetType().Name}";
    }

    protected static string DescribeCreature(Creature? creature)
    {
        return creature == null ? "<null>" : creature.GetType().Name;
    }
}
