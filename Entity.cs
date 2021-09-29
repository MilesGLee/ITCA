using System;
using System.Collections.Generic;
using System.Text;

namespace ITCA
{
    class Entity : Object //An entity is an object that can move from where it was placed and has stats.
    {
        public int Health; //The stats of the entity
        public int AttackPower;
        public int DefensePower;
        public void Move(string direction) //Used to move the entity on the map.
        {
            if (direction == "Up" && PositionY > 0 && Game.map[PositionY - 1, PositionX] != "[]") //if the position the entity is trying to move to is not outside the bounds of the matrix and is an open/movable location.
                PositionY--;
            if (direction == "Down" && PositionY < 4 && Game.map[PositionY + 1, PositionX] != "[]") //if the position the entity is trying to move to is not outside the bounds of the matrix and is an open/movable location.
                PositionY++;
            if (direction == "Right" && PositionX < 4 && Game.map[PositionY, PositionX + 1] != "[]") //if the position the entity is trying to move to is not outside the bounds of the matrix and is an open/movable location. 
                PositionX++;
            if (direction == "Left" && PositionX > 0 && Game.map[PositionY, PositionX - 1] != "[]")//if the position the entity is trying to move to is not outside the bounds of the matrix and is an open/movable location.
                PositionX--;
        }

        public float TakeDamage(float damageAmount) //For when the entity takes damage from another entity.
        {
            float damageTaken = damageAmount - DefensePower;

            if (damageTaken < 0)
            {
                damageTaken = 0;
            }
            Health -= (int)damageTaken;
            return damageTaken;
        }

        public float Attack(Entity Victim) //When this entity is damaging another entity.
        {
            return Victim.TakeDamage(AttackPower);
        }
    }
}
