using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stationary : Singleton<Stationary>
{
   private List<OrderProperties> _orders = new List<OrderProperties>();

   public void AddOrder(OrderProperties order)
   {
      _orders.Add(order);
      
      int index = _orders.IndexOf(order);

      order.OrderProcess = StartCoroutine(Cook(order));
   }

   IEnumerator Cook(OrderProperties cooking)
   {
      yield return new WaitForSeconds(0f);
   }
}
