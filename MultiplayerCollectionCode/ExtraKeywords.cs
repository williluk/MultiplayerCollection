using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerCollection.MultiplayerCollectionCode;


public static class ExtraKeywords
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Temporary; 

    public static bool IsTemporary(this CardModel card)
    {
        return card.Keywords.Contains(Temporary);
    }

}