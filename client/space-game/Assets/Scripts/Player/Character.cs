using UnityEngine;

public enum CharacterType
{
    Player,
    Enemy
}

public interface ICharacter
{
    CharacterType CharacterType { get; set; }
    Ship Ship { get; set; } 

    void TakeDamage(int damageAmount);
    void Heal(int healAmount);
    void SetMaxHealth(int maxHealth);

    void Refuel(int refuelAmount);
    void SetMaxFuel(int maxFuel);

    void SetCharacterType(CharacterType characterType);

    void SetShip(Ship ship);
    void SetShipClass(ShipClass shipClass);
}

public class Character : MonoBehaviour, ICharacter
{
    public CharacterType CharacterType { get; set; }
    public Ship Ship { get; set; }

    public void TakeDamage(int damageAmount)
    {
        Ship.Health -= damageAmount;
    }

    public void Heal(int healAmount)
    {
       Ship.Health += healAmount;
    }

    public void SetMaxHealth(int maxHealth)
    {
        Ship.MaxHealth = maxHealth;
        Ship.Health = Ship.MaxHealth;
    }

    public void SetMaxFuel(int maxFuel) 
    {
        Ship.MaxFuel = maxFuel;
        Ship.Fuel = Ship.MaxFuel;
    }
    
    public void Refuel(int refuelAmount) {}

    public void SetCharacterType(CharacterType characterType) => CharacterType = characterType;

    public void SetShip(Ship ship) => Ship = ship;

    public void SetShipClass(ShipClass shipClass) => Ship.ShipClass = shipClass;
}