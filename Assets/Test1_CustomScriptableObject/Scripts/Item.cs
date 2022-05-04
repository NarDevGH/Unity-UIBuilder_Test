using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TestProject1 
{
    public class Item
    {
        private string _itemName;
        private Sprite _itemImage;
        private int _itemRarity;

        public string ItemName 
        { 
            get => _itemName; 
            set => _itemName = value; 
        }
        public Sprite ItemImage 
        { 
            get => _itemImage; 
            set => _itemImage = value; 
        }
        public int ItemRarity 
        { 
            get => _itemRarity; 
            set => _itemRarity = value; 
        }
    }
}
