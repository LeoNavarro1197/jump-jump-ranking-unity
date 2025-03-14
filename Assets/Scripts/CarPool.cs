using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPool : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] public int poolSize;
    [SerializeField] public float amountTime;
    [SerializeField] public List<GameObject> carList;

    [SerializeField] public List<GameObject> pointList;

    private static CarPool instance;
    public static CarPool Instance { get { return instance; } }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddCarToPool(poolSize);  // Cantidad de espacios de la lista
        StartCoroutine(InstantiateTimeCars()); //posicionar los carros en la posicion X cada X tiempo
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Se guardan los prefabs de los carros en la lista
    private void AddCarToPool(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject car = Instantiate(carPrefab); // guardar la instancia en un gameobject
            car.SetActive(false);
            carList.Add(car);
            car.transform.parent = transform; // se instancian en una posicion en la jerarquia
        }
    }

    // Se activan los prefabs de la lista y añade uno mas a la lista
    public GameObject RequestCar()
    {
        for(int i = 0; i < carList.Count; i++)
        {
            if (!carList[i].activeSelf)
            {
                carList[i].SetActive(true);
                return carList[i];
            }
        }
        // añadimos una posicion mas a la lista. se instancian mas carros de la lista
        AddCarToPool(1);
        carList[carList.Count - 1].SetActive(true);
        return carList[carList.Count - 1];
    }

    //posicionar los carros en la posicion X cada X tiempo
    public IEnumerator InstantiateTimeCars()
    {
        int i = 0;
        while (i < poolSize)
        {
            i--;
            yield return new WaitForSeconds(amountTime);
            GameObject car = Instance.RequestCar(); // guardo la instancia del carro en un gameobject
            int random = Random.Range(0, poolSize);
            car.transform.position = pointList[random].transform.position; // se posicionan los carros en una lista de posiciones random de la jerarquia
            car.SetActive(true);
            i++;
        }
    }
}
