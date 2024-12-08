using UnityEngine;
using Cinemachine;

public class RoomCameras : MonoBehaviour
{
    public CinemachineVirtualCamera[] roomCameras;
    public int currentRoomIndex = 0;

    void Start()
    {
        SwitchRoom(0);
    }

    public void SwitchRoom(int newRoomIndex)
    {
        roomCameras[currentRoomIndex].Priority = 0;
        currentRoomIndex = newRoomIndex;
        roomCameras[currentRoomIndex].Priority = 10;
    }
}
