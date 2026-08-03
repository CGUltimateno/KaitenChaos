using System.Collections.Generic;
using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-03
// AI assistance: VS Code (autocomplete), Claude (architecture review)

public class SeatManager : MonoBehaviour
{
    [Header("Seat Setup")]
    [SerializeField] private Transform[] seatPositions = new Transform[4];

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 5f;

    private Customer[] occupants;
    private float spawnTimer;
    private int turnedAwayCount;

    public int TurnedAwayCount => turnedAwayCount;

    private void Awake()
    {
        occupants = new Customer[seatPositions.Length];
        spawnTimer = 0f;
        turnedAwayCount = 0;
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnCustomer();
        }
    }

    private void TrySpawnCustomer()
    {
        int freeSeat = FindFreeSeat();

        if (freeSeat < 0)
        {
            turnedAwayCount++;
            Debug.Log($"[SeatManager] All seats full — turned away #{turnedAwayCount}");
            return;
        }

        Ticket ticket = OrderGenerator.Generate(freeSeat);

        GameObject customerObj = new GameObject($"Customer_Seat{freeSeat}");
        customerObj.transform.position = seatPositions[freeSeat].position;

        Customer customer = customerObj.AddComponent<Customer>();
        customer.Init(ticket, freeSeat);

        occupants[freeSeat] = customer;

        Debug.Log($"[SeatManager] Seated customer at seat {freeSeat} — " +
                  $"wants {ticket.requestedCount}x {ticket.cravingTrait}: \"{ticket.phrase}\"");
    }

    private int FindFreeSeat()
    {
        List<int> freeSeats = new List<int>();
        for (int i = 0; i < occupants.Length; i++)
        {
            if (occupants[i] == null)
                freeSeats.Add(i);
        }

        if (freeSeats.Count == 0)
            return -1;

        return freeSeats[Random.Range(0, freeSeats.Count)];
    }

    public void FreeSeat(int seatIndex)
    {
        if (seatIndex >= 0 && seatIndex < occupants.Length && occupants[seatIndex] != null)
        {
            Destroy(occupants[seatIndex].gameObject);
            occupants[seatIndex] = null;
        }
    }

    public Customer GetCustomerAtSeat(int seatIndex)
    {
        if (seatIndex >= 0 && seatIndex < occupants.Length)
            return occupants[seatIndex];
        return null;
    }
}
