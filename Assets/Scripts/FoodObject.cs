using UnityEngine;

public class FoodObject : CellObject
{


    public int AmountGranted = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayerEntered()
    {
        Destroy(gameObject);

        //increase food
        GameManager.Instance.ChangeFood(AmountGranted);
    }
}
