using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace DiscardMod.Utils;

public enum DiscardTriggerSource
{
    Unknown,
    CardEffect,
    ExplicitCardException,
}

public readonly record struct DiscardEventContext(
    int DiscardCountThisTurn,
    DiscardTriggerSource Source,
    bool ShouldTrigger)
{
    public bool IsOddDiscard => DiscardCountThisTurn % 2 == 1;
}

internal static class DiscardTriggerRuntime
{
    private sealed record DiscardScope(DiscardTriggerSource Source, CardModel? ExplicitCard);

    private readonly record struct TurnMarker(object? CombatState, int TurnNumber);

    private static readonly object SyncRoot = new();
    private static readonly Dictionary<CardModel, Queue<DiscardEventContext>> PendingEvents = new(ReferenceEqualityComparer.Instance);
    private static readonly Dictionary<object, int> PendingBonusDiscards = new(ReferenceEqualityComparer.Instance);
    private static readonly AsyncLocal<Stack<DiscardScope>?> ScopeStack = new();

    private static object? currentCombatState;
    private static int currentTurnNumber = -1;
    private static int discardCountThisTurn;

    public static IDisposable BeginCardEffectDiscardScope()
    {
        return PushScope(new DiscardScope(DiscardTriggerSource.CardEffect, null));
    }

    public static IDisposable BeginExplicitExceptionScope(CardModel explicitCard)
    {
        return PushScope(new DiscardScope(DiscardTriggerSource.ExplicitCardException, explicitCard));
    }

    public static string CurrentScopeDescription => GetCurrentScope().Source switch
    {
        DiscardTriggerSource.CardEffect => "card-effect",
        DiscardTriggerSource.ExplicitCardException => "explicit-card-exception",
        _ => "default",
    };

    public static void RegisterDiscards(PlayerChoiceContext choiceContext, IEnumerable<CardModel> cards)
    {
        var array = cards.Where(card => card != null).ToArray();
        if (array.Length == 0)
        {
            return;
        }

        lock (SyncRoot)
        {
            ResetForNewTurnIfNeeded(choiceContext, array[0]);

            var scope = GetCurrentScope();
            foreach (var card in array)
            {
                discardCountThisTurn += 1;
                var context = new DiscardEventContext(
                    discardCountThisTurn,
                    scope.Source,
                    scope.Source == DiscardTriggerSource.CardEffect
                        || (scope.Source == DiscardTriggerSource.ExplicitCardException && ReferenceEquals(card, scope.ExplicitCard)));

                if (!PendingEvents.TryGetValue(card, out var queue))
                {
                    queue = new Queue<DiscardEventContext>();
                    PendingEvents[card] = queue;
                }

                queue.Enqueue(context);
            }
        }
    }

    public static DiscardEventContext ConsumeDiscardEvent(PlayerChoiceContext choiceContext, CardModel card)
    {
        lock (SyncRoot)
        {
            ResetForNewTurnIfNeeded(choiceContext, card);

            if (!PendingEvents.TryGetValue(card, out var queue) || queue.Count == 0)
            {
                return new DiscardEventContext(discardCountThisTurn, DiscardTriggerSource.Unknown, false);
            }

            var context = queue.Dequeue();
            if (queue.Count == 0)
            {
                PendingEvents.Remove(card);
            }

            return context;
        }
    }

    public static void GrantBonusDiscard(object? owner, int amount)
    {
        if (owner == null || amount <= 0)
        {
            return;
        }

        lock (SyncRoot)
        {
            PendingBonusDiscards.TryGetValue(owner, out var current);
            PendingBonusDiscards[owner] = current + amount;
        }
    }

    public static int PeekBonusDiscard(object? owner)
    {
        if (owner == null)
        {
            return 0;
        }

        lock (SyncRoot)
        {
            return PendingBonusDiscards.TryGetValue(owner, out var amount) && amount > 0
                ? amount
                : 0;
        }
    }

    public static int ConsumeBonusDiscard(object? owner)
    {
        if (owner == null)
        {
            return 0;
        }

        lock (SyncRoot)
        {
            if (!PendingBonusDiscards.TryGetValue(owner, out var amount) || amount <= 0)
            {
                return 0;
            }

            PendingBonusDiscards.Remove(owner);
            return amount;
        }
    }

    private static void ResetForNewTurnIfNeeded(PlayerChoiceContext choiceContext, CardModel? card)
    {
        var marker = ResolveTurnMarker(choiceContext, card);
        if (marker.CombatState == null)
        {
            return;
        }

        if (!ReferenceEquals(currentCombatState, marker.CombatState) || currentTurnNumber != marker.TurnNumber)
        {
            PendingEvents.Clear();
            PendingBonusDiscards.Clear();
            currentCombatState = marker.CombatState;
            currentTurnNumber = marker.TurnNumber;
            discardCountThisTurn = 0;
        }
    }

    private static TurnMarker ResolveTurnMarker(PlayerChoiceContext choiceContext, CardModel? card)
    {
        var combatState = card?.Owner?.Creature?.CombatState
            ?? TryGetMemberValue(choiceContext, "CombatState")
            ?? TryGetMemberValue(card?.Owner, "CombatState")
            ?? TryGetMemberValue(card?.Owner, "PlayerCombatState");

        if (combatState == null)
        {
            return new TurnMarker(null, -1);
        }

        var turnNumber = TryResolveTurnNumber(combatState)
            ?? TryResolveTurnNumber(choiceContext)
            ?? 0;

        return new TurnMarker(combatState, turnNumber);
    }

    private static int? TryResolveTurnNumber(object? instance)
    {
        if (instance == null)
        {
            return null;
        }

        foreach (var name in new[] { "TurnCount", "CurrentTurn", "TurnNumber", "TurnIndex", "Turn", "turnCount", "currentTurn", "turnNumber" })
        {
            if (TryReadIntMember(instance, name, out var exact))
            {
                return exact;
            }
        }

        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        foreach (var property in instance.GetType().GetProperties(flags))
        {
            if (property.PropertyType != typeof(int) || !property.Name.Contains("Turn", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (TryReadIntMember(instance, property.Name, out var value))
            {
                return value;
            }
        }

        foreach (var field in instance.GetType().GetFields(flags))
        {
            if (field.FieldType != typeof(int) || !field.Name.Contains("Turn", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (TryReadIntMember(instance, field.Name, out var value))
            {
                return value;
            }
        }

        foreach (var nestedName in new[] { "CombatState", "PlayerCombatState" })
        {
            var nested = TryGetMemberValue(instance, nestedName);
            var nestedTurn = TryResolveTurnNumber(nested);
            if (nestedTurn.HasValue)
            {
                return nestedTurn;
            }
        }

        return null;
    }

    private static bool TryReadIntMember(object instance, string name, out int value)
    {
        value = default;

        var memberValue = TryGetMemberValue(instance, name);
        if (memberValue is int intValue)
        {
            value = intValue;
            return true;
        }

        return false;
    }

    private static object? TryGetMemberValue(object? instance, string name)
    {
        if (instance == null)
        {
            return null;
        }

        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var type = instance.GetType();
        var property = type.GetProperty(name, flags);
        if (property != null)
        {
            return property.GetValue(instance);
        }

        var field = type.GetField(name, flags);
        return field?.GetValue(instance);
    }

    private static DiscardScope GetCurrentScope()
    {
        var stack = ScopeStack.Value;
        return stack != null && stack.Count > 0
            ? stack.Peek()
            : new DiscardScope(DiscardTriggerSource.Unknown, null);
    }

    private static IDisposable PushScope(DiscardScope scope)
    {
        var stack = ScopeStack.Value;
        if (stack == null)
        {
            stack = new Stack<DiscardScope>();
            ScopeStack.Value = stack;
        }

        stack.Push(scope);
        return new ScopeHandle();
    }

    private sealed class ScopeHandle : IDisposable
    {
        public void Dispose()
        {
            var stack = ScopeStack.Value;
            if (stack == null || stack.Count == 0)
            {
                return;
            }

            stack.Pop();
        }
    }
}