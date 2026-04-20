using UnityEngine;
using System.Collections.Generic;
public class CustomerController : MonoBehaviour
{
    public CustomerData customerData;               //¼Õ´Ô µ¥ÀÌÅÍ °¡Á®¿À±â
    bool isJoining = true;
    bool ableToOrder = false;
    public bool isQuitting = false;
    public bool isDone = false;
    public GameObject customerPrefab;

    private static CustomerController instance;

    public static CustomerController Instance
    {
        get
        {
            if (instance == null) instance = new CustomerController();
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

    void Start()
    {
        customerPrefab = gameObject;
    }

    void Update()
    {
        if(isJoining && ableToOrder == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, ShopManager.Instance.orderPosition.position, customerData.moveSpeed * Time.deltaTime);
        }
        if(transform.position == ShopManager.Instance.orderPosition.position && isJoining)
        {
            isJoining = false;
            ShopManager.Instance.isOrdering = true;
        }

        if(isQuitting)
        {
            transform.position = Vector3.MoveTowards(transform.position, ShopManager.Instance.joinPosition.position, customerData.moveSpeed * Time.deltaTime);
        }
        
        if(isQuitting && transform.position == ShopManager.Instance.joinPosition.position)
        {
            ShopManager.Instance.WaitNextCustomer();
            Debug.Log("¼Õ³ð»ç¸Á¤»¤»");
            Destroy(gameObject);
        }
    }
}
