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
  None,
  Missile,
  Mine,
  Torpedo
}

public enum ShipSpecialWeapon
{
  None,
  EMP,
  Shield,
  Cloak
}

public class Ship 
{
  public Ship(ShipClass shipClass, ShipEngine engine, ShipPrimaryWeapon primaryWeapon, ShipSecondaryWeapon secondaryWeapon, ShipSpecialWeapon specialWeapon)
  {
    SetShipClass(shipClass);
    SetShipEngine(engine);
    SetShipPrimaryWeapon(primaryWeapon);
    SetShipSecondaryWeapon(secondaryWeapon);
    SetShipSpecialWeapon(specialWeapon);
  }

  public int Health { get; set; }
  public int MaxHealth { get; set; }

  public int Fuel { get; set; }
  public int MaxFuel { get; set; }

  public int EngineSpeed { get; set; }
  public int EngineAcceleration { get; set; }
  public int EngineTurnSpeed { get; set; }

  public int PrimaryWeaponDamage { get; set; }
  public int PrimaryWeaponFireRate { get; set; }
  public int PrimaryWeaponRange { get; set; }

  public int SecondaryWeaponDamage { get; set; }
  public int SecondaryWeaponFireRate { get; set; }
  public int SecondaryWeaponRange { get; set; }

  public ShipClass ShipClass { get; set; }
  public ShipEngine ShipEngine { get; set; }
  
  public ShipPrimaryWeapon ShipPrimaryWeapon { get; set; }
  public ShipSecondaryWeapon ShipSecondaryWeapon { get; set; }
  public ShipSpecialWeapon ShipSpecialWeapon { get; set; }

  private void SetShipClass(ShipClass _shipClass)
  {
    ShipClass = _shipClass;
  }

  private void SetShipEngine(ShipEngine _shipEngine)
  {
    ShipEngine = _shipEngine;

    Health = 100;

    switch (ShipEngine)
    {
      case ShipEngine.Ion:
        EngineSpeed = 200;
        EngineAcceleration = 100;
        EngineTurnSpeed = 5;
        break;
      case ShipEngine.Rocket:
        EngineSpeed = 100;
        EngineAcceleration = 100;
        EngineTurnSpeed = 100;
        break;
      case ShipEngine.Plasma:
        EngineSpeed = 100;
        EngineAcceleration = 100;
        EngineTurnSpeed = 100;
        break;
      case ShipEngine.Warp:
        EngineSpeed = 100;
        EngineAcceleration = 100;
        EngineTurnSpeed = 100;
        break;
      case ShipEngine.Hyperdrive:
        EngineSpeed = 100;
        EngineAcceleration = 100;
        EngineTurnSpeed = 100;
        break;
    }
  }

  private void SetShipPrimaryWeapon(ShipPrimaryWeapon _shipPrimaryWeapon)
  {
    ShipPrimaryWeapon = _shipPrimaryWeapon;
  }

  private void SetShipSecondaryWeapon(ShipSecondaryWeapon _shipSecondaryWeapon)
  {
    ShipSecondaryWeapon = _shipSecondaryWeapon;
  }

  private void SetShipSpecialWeapon(ShipSpecialWeapon _shipSpecialWeapon)
  {
    ShipSpecialWeapon = _shipSpecialWeapon;
  }
}