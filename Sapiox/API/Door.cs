using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using Sapiox.API.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sapiox.API
{
    public class Door
    {
        public bool Breakable => DoorVariant is BreakableDoor;
        public bool Destroyed => DoorVariant is BreakableDoor door && door.IsDestroyed;
        public bool Pryable => DoorVariant is PryableDoor;

        public DoorVariant DoorVariant { get; set; }
        public GameObject GameObject { get; set; }

        public DoorPermissions Permissions
        {
            get => DoorVariant.RequiredPermissions;
            set => DoorVariant.RequiredPermissions = value;
        }

        public bool Open
        {
            get => DoorVariant.IsConsideredOpen();
            set => DoorVariant.NetworkTargetState = value;
        }

        public bool Locked
        {
            get => DoorVariant.ActiveLocks > 0;
            set => DoorVariant.ServerChangeLock(DoorLockReason.SpecialDoorFeature, value);
        }

        public bool Break()
        {
            if (DoorVariant is BreakableDoor door && !door.IsDestroyed)
            {
                door.ServerDamage(ushort.MaxValue, DoorDamageType.ServerCommand);
                return true;
            }

            return false;
        }

        public bool Pry() => DoorVariant is PryableDoor door && door.TryPryGate();

        public Vector3 Position
        {
            get => DoorVariant.transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                DoorVariant.transform.position = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        public Quaternion Rotation
        {
            get => DoorVariant.transform.rotation;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                DoorVariant.transform.rotation = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        public Vector3 Scale
        {
            get => DoorVariant.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                DoorVariant.transform.localScale = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        public Door(DoorVariant door, Vector3 position, Quaternion rotation, Vector3 scale, bool open = false)
        {
            DoorVariant = door;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Open = open;
            NetworkServer.Spawn(GameObject);
        }

        public void Spawn()
        {
            NetworkServer.Spawn(GameObject);
        }

        public void DeSpawn()
        {
            NetworkServer.UnSpawn(GameObject);
        }
    }
}
