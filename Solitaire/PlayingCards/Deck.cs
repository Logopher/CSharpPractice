using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayingCards
{
	public class Deck
	{
		private List<Card> cards = new List<Card>(52);

		private Random random = new Random();

		public Deck()
		{
			this.init(false);
		}

		public Deck(bool jokers)
		{
			this.init(jokers);
		}

		private void init(bool jokers)
		{
			Type suitType = typeof(Card.Suit);
			int count = Enum.GetNames(suitType).Count();
			Card.Suit suit;
			for(int i0 = 1; i0 < count; ++i0)
			{
				suit = (Card.Suit)i0;
				for (int i1 = 1; i1 <= 13; ++i1)
				{
					this.Insert(new Card(i1, suit));
				}
			}

			if (jokers)
			{
				this.Insert(new Card(0, Card.Suit.NONE));
				this.Insert(new Card(0, Card.Suit.NONE));
			}
		}

		public Card Peek()
		{
			return this.cards[this.cards.Count - 1];
		}

		public Card Pop()
		{
			Card result = this.Peek();
			this.cards.RemoveAt(this.cards.Count - 1);
			return result;
		}

		public void Push(Card c)
		{
			this.cards.Add(c);
		}

		public void Insert(Card c)
		{
			this.cards.Insert(this.random.Next(this.cards.Count), c);
		}
	}
}
