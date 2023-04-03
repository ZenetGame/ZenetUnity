using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceSimulation : MonoBehaviour
{
    public int numberOfSticks = 4;
    public GameObject stickPrefab;
    public float dropHeight = 5.0f;
    public Vector2 dropZoneRange = new Vector2(-2, 2);

    public Button throwAgainButton;
    public TMP_Text whiteSidesText;



    private int whiteSideUpCount = 0;

    void Start()
    {
        throwAgainButton.onClick.AddListener(ThrowSticks);
        StartCoroutine(DropSticks());
    }



    public void ThrowSticks()
    {
        // Destroy existing sticks
        GameObject[] sticks = GameObject.FindGameObjectsWithTag("Stick");
        foreach (GameObject stick in sticks)
        {
            Destroy(stick);
        }

        // Reset white side count and start the simulation again
        whiteSideUpCount = 0;
        whiteSidesText.text = "White sides up: 0";
        StartCoroutine(DropSticks());
    }



    IEnumerator DropSticks()
    {
        // Set the spawn position to be the "throwing hand" position
        Vector3 spawnPosition = new Vector3(0, dropHeight, -2);

        for (int i = 0; i < numberOfSticks; i++)
        {
            Quaternion randomRotation = Quaternion.Euler(
                Random.Range(0, 360),
                Random.Range(0, 360),
                Random.Range(0, 360)
            );

            GameObject stick = Instantiate(stickPrefab, spawnPosition, randomRotation);

            // Apply a directed force to the stick to simulate a throwing motion
            Rigidbody stickRigidbody = stick.GetComponent<Rigidbody>();
            Vector3 throwForce = new Vector3(
                Random.Range(-10.0f, 10.0f),
                Random.Range(-20.0f, 0.0f),
                Random.Range(50.0f, 100.0f)
            );
            stickRigidbody.AddForce(throwForce, ForceMode.Impulse);

            // Apply a random torque to the stick
            Vector3 randomTorque = new Vector3(
                Random.Range(-10.0f, 10.0f),
                Random.Range(-10.0f, 10.0f),
                Random.Range(-10.0f, 10.0f)
            );
            stickRigidbody.AddTorque(randomTorque, ForceMode.Impulse);
        }

        // Wait for the sticks to settle down before counting the white sides
        yield return new WaitForSeconds(3.0f);
        CountWhiteSides();
    }






    void CountWhiteSides()
    {
        GameObject[] sticks = GameObject.FindGameObjectsWithTag("Stick");

        foreach (GameObject stick in sticks)
        {
            Vector3 upVector = stick.transform.up;
            if (Vector3.Dot(upVector, Vector3.up) > 0)
            {
                whiteSideUpCount++;
            }
        }

        whiteSidesText.text = "White sides up: " + whiteSideUpCount;
    }
}
