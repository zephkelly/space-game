using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerFire : NetworkBehaviour
{
  private float fireRate = 0.5f;
  private float nextFireTime = 0f;

  [SerializeField]
  private PrimaryWeaponFireWrapper weaponParticleFire;

  private void Update()
  {
    if (!isLocalPlayer) {
      return;
    }

    if (Input.GetButton("Fire1") && Time.time > nextFireTime) {
        nextFireTime = Time.time + fireRate;
        Fire();
    }
  }

  private void Fire()
  {
    weaponParticleFire.Shoot();
    CmdFire();
  }

  [Command]
  private void CmdFire()
  {
    weaponParticleFire.Shoot();
    RpcFire();
  }

  [ClientRpc(includeOwner = false)]
  private void RpcFire()
  {
    weaponParticleFire.Shoot();
  }
}
