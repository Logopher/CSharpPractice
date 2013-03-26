using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlayingCards;

namespace Solitaire
{
	class Program
	{
		static void Main(string[] args)
		{
			Stack<Card>[] tableauPiles = new Stack<Card>[7];
			Stack<Card>[] foundationPiles = new Stack<Card>[4];
			Stack<Card> wastePile = new Stack<Card>();
			Deck stockPile = new Deck();

			for (int i = 0; i < foundationPiles.Length; ++i)
			{
				foundationPiles[i] = new Stack<Card>(13);
			}

			for (int i = 0; i < tableauPiles.Length; ++i)
			{
				tableauPiles[i] = new Stack<Card>(i + 1);
				tableauPiles[i].Push(stockPile.Pop());
			}

//			Card waste = wastePile.Peek();
//			Console.WriteLine("waste: {0} of {1}", waste.value, waste.suit);
			Console.WriteLine("tableau: {0}", tableauPiles.Map(pile =>  "\n" + pile.Peek().ToString()).Join(""));
			Console.ReadKey();
		}
	}
}
