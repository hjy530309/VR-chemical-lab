using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons Buttons;
    public Vector3 ControllerPosition;
    public Quaternion ControllerRotation;
}

public static class NetworkInputButton
{
    public const int Trigger = 0;
    public const int Grip = 1;
    public const int Primary = 2;
    public const int Secondary = 3;
}