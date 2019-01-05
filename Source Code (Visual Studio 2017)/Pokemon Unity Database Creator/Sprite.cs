using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon_Unity_Database_Creator
{
    class Sprite
    {
        public PokemonSprite MaleFront;
        public PokemonSprite MaleBack;

        public bool HasFemaleSprite { get; set; } = false;

        public PokemonSprite FemaleFront;
        public PokemonSprite FemaleBack;
    }
}
