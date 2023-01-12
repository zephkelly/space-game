public enum ShipClass
{
    Scout,
    Fighter,
    Bomber,
    Dreadnaught
}

public enum ShipEngine
{
    Ion,
    Rocket,
    Plasma,
    Warp,
    Hyperdrive
}

public enum ShipPrimaryWeapon
{
    Laser,
    Plasma,
    Railgun
}

public enum ShipSecondaryWeapon
{
    Missile,
    Mine,
    Torpedo
}

public enum ShipSpecialWeapon
{
    EMP,
    Shield,
    Cloak
}

public class Ship 
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }

    public int Fuel { get; set; }
    public int MaxFuel { get; set; }

    public ShipClass ShipClass { get; set; }
    public ShipEngine ShipEngine { get; set; }
    
    public ShipPrimaryWeapon ShipPrimaryWeapon { get; set; }
    public ShipSecondaryWeapon ShipSecondaryWeapon { get; set; }
    public ShipSpecialWeapon ShipSpecialWeapon { get; set; }
}