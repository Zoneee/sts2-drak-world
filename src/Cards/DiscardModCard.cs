using BaseLib.Abstracts;
using System.IO;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace DiscardMod.Cards;

public abstract class DiscardModCard : CustomCardModel
{
    private readonly string assetName;

    protected DiscardModCard(int cost, CardType type, CardRarity rarity, TargetType target, string assetName,
        bool showInCardLibrary = true)
        : base(cost, type, rarity, target, showInCardLibrary)
    {
        this.assetName = assetName;
    }

    public override string PortraitPath => Path.Join(DiscardModMain.ModId, "images", "card_portraits", $"{assetName}.png");

    public override string BetaPortraitPath => PortraitPath;

    public override string CustomPortraitPath => Path.Join(DiscardModMain.ModId, "images", "card_portraits", "big", $"{assetName}.png");
}