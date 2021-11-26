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
    public class ShootingTarget
    {
        public GameObject GameObject { get; internal set; }
        public ShootingTargetType Type { get; set; }

        public Vector3 Position
        {
            get => GameObject.transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.position = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public Quaternion Rotation
        {
            get => GameObject.transform.rotation;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.rotation = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        public Vector3 Scale
        {
            get => GameObject.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.localScale = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        public ShootingTarget(Vector3 position, Quaternion rotation, Vector3 scale, ShootingTargetType type)
        {
            Type = type;
            if (Type == ShootingTargetType.binary)
            {
                GameObject = UnityEngine.Object.Instantiate(NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "binaryTargetPrefab"));
            }
            else if (Type == ShootingTargetType.dboy)
            {
                GameObject = UnityEngine.Object.Instantiate(NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "dboyTargetPrefab"));
            }
            else if (Type == ShootingTargetType.sport)
            {
                GameObject = UnityEngine.Object.Instantiate(NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "sportTargetPrefab"));
            }
            Scale = scale;
            Position = position;
            Rotation = rotation;
            NetworkServer.Spawn(GameObject);
        }

        public void Spawn()
        {
            NetworkServer.Spawn(GameObject);
            Map.ShootingTargets.Add(this);
        }

        public void DeSpawn()
        {
            NetworkServer.UnSpawn(GameObject);
            Map.ShootingTargets.Remove(this);
        }
    }
}
