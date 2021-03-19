using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public interface IDamageable 
{
    public void TakeDamage(float damage,Player damageDealer);

}
