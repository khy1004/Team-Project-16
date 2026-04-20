using UnityEngine;
using System.Linq;
using System.Collections.Generic;
public class PotionEstimate : MonoBehaviour
{
    public PotionData craftedPotionData;
    public CustomerData currentCustomerData;

    private static PotionEstimate instance;

    public static PotionEstimate Instance
    {
        get
        {
            if (instance == null) instance = new PotionEstimate();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public bool EstimateStart()
    {
        bool isSame = craftedPotionData.potionRecipe.SequenceEqual(currentCustomerData.potionOrder.potionRecipe);
        if(isSame)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
