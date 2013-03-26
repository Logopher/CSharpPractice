using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace PlayingCards
{
	public struct Card
	{
		public enum Suit
		{
			NONE,
			SPADES,
			HEARTS,
			CLUBS,
			DIAMONDS
		}

		public int value;
		public Suit suit;

		public Card(int value, Suit suit)
		{
			this.value = value;
			this.suit = suit;
		}

		public override string ToString()
		{
			int value = this.value;
			if (value == 0)
			{
				return "Joker";
			}

			string valueString;
			switch (value)
			{
				case 1:
					valueString = "A";
					break;
				case 11:
					valueString = "J";
					break;
				case 12:
					valueString = "Q";
					break;
				case 13:
					valueString = "K";
					break;
				default:
					valueString = value.ToString();
					break;
			}

			string suit = this.suit.ToString();
			string suitAsTitle = suit.Substring(0, 1).ToUpper() + suit.Substring(1).ToLower();

			return string.Format("{0} of {1}", valueString, suitAsTitle);
		}
	}
}
