using System;
using System.Collections.Generic;
using System.Text;

namespace ITCA
{
    class Object
    {
        public string icon = "[]"; //The objects icon is what will be displayed on the map matrix.
        private int _x; //Objects position on the horizontal axis of the map
        private int _y; //Objects position on the vertical axis of the map
        public string name; //The objects name used for text displays.
        private int _itemPower;

        public int PositionX //Get or set the objects X component.
        {
            get 
            {
                return _x;
            }
            set 
            {
                _x = value;
            }
        }
        public int PositionY //Get or set the objects Y component.
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }
        public int ItemPower //Get or set the objects item power.
        {
            get
            {
                return _itemPower;
            }
            set
            {
                _itemPower = value;
            }
        }
    }
}
