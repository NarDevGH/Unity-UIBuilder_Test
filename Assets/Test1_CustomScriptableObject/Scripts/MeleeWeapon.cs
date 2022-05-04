
namespace TestProject1 
{
    public class MeleeWeapon :Item
    {
        private int _damage;
        private int _durability;

        public int Damage 
        { 
            get => _damage; 
            set => _damage = value; 
        }
        public int Durability 
        { 
            get => _durability; 
            set => _durability = value; 
        }
    }
}
