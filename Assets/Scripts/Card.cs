using UnityEngine;

namespace Global {

    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class Card : ScriptableObject
    {
        public string faceValue;
        public CardType cardType;
        public CardPack cardPack;
        public CostType costType;
        public int health;
        public int damage;
        public int cost;

        public enum CardType {
            Operation,
            Number
        }

        public enum CardPack 
        {
            Arithmetic,
            Calculus,
            LinearAlgebra,
            SmallNumbers,
            LargeNumbers
        }

        public enum CostType 
        {
            Variables,
            Bytes
        }

    }
}
