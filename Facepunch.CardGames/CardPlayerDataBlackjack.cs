using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Facepunch.CardGames;

public class CardPlayerDataBlackjack : CardPlayerData
{
	public List<PlayingCard> SplitCards;

	public int splitBetThisRound;

	public int insuranceBetThisRound;

	public bool playingSplitCards;

	public CardPlayerDataBlackjack(int mountIndex, bool isServer)
		: base(mountIndex, isServer)
	{
		SplitCards = Pool.GetList<PlayingCard>();
	}

	public CardPlayerDataBlackjack(int scrapItemID, Func<int, StorageContainer> getStorage, int mountIndex, bool isServer)
		: base(scrapItemID, getStorage, mountIndex, isServer)
	{
		SplitCards = Pool.GetList<PlayingCard>();
	}

	public override void Dispose()
	{
		base.Dispose();
		Pool.FreeList<PlayingCard>(ref SplitCards);
	}

	public override int GetTotalBetThisRound()
	{
		return betThisRound + splitBetThisRound + insuranceBetThisRound;
	}

	public override List<PlayingCard> GetSecondaryCards()
	{
		return SplitCards;
	}

	protected override void ClearPerRoundData()
	{
		base.ClearPerRoundData();
		SplitCards.Clear();
		splitBetThisRound = 0;
		insuranceBetThisRound = 0;
		playingSplitCards = false;
	}

	public override void LeaveCurrentRound(bool clearBets, bool leftRoundEarly)
	{
		if (base.HasUserInCurrentRound)
		{
			if (clearBets)
			{
				splitBetThisRound = 0;
				insuranceBetThisRound = 0;
			}
			base.LeaveCurrentRound(clearBets, leftRoundEarly);
		}
	}

	public override void LeaveGame()
	{
		base.LeaveGame();
		if (base.HasUserInGame)
		{
			SplitCards.Clear();
		}
	}

	public override void Save(CardGame syncData)
	{
		base.Save(syncData);
		BlackjackCardPlayer val = Pool.Get<BlackjackCardPlayer>();
		val.splitCards = Pool.GetList<int>();
		foreach (PlayingCard splitCard in SplitCards)
		{
			val.splitCards.Add(base.SendCardDetails ? splitCard.GetIndex() : (-1));
		}
		val.splitBetThisRound = splitBetThisRound;
		val.insuranceBetThisRound = insuranceBetThisRound;
		val.playingSplitCards = playingSplitCards;
		if (syncData.blackjack.players == null)
		{
			syncData.blackjack.players = Pool.GetList<BlackjackCardPlayer>();
		}
		syncData.blackjack.players.Add(val);
	}

	public bool TrySwitchToSplitHand()
	{
		if (SplitCards.Count > 0 && !playingSplitCards)
		{
			SwapSplitCardsWithMain();
			playingSplitCards = true;
			return true;
		}
		return false;
	}

	private void SwapSplitCardsWithMain()
	{
		List<PlayingCard> list = Pool.GetList<PlayingCard>();
		list.AddRange(Cards);
		Cards.Clear();
		Cards.AddRange(SplitCards);
		SplitCards.Clear();
		SplitCards.AddRange(list);
		Pool.FreeList<PlayingCard>(ref list);
		int num = betThisRound;
		int num2 = splitBetThisRound;
		splitBetThisRound = num;
		betThisRound = num2;
	}
}
