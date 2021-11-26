using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sapiox.API
{
    public static class Map
    {
        public static IReadOnlyCollection<DoorVariant> Doors = (IReadOnlyCollection<DoorVariant>)UnityEngine.Object.FindObjectOfType<DoorVariant>();
        public static List<WorkstationController> WorkStations => WorkstationController.AllWorkstations.ToList();
        public static List<ShootingTarget> ShootingTargets = new List<ShootingTarget>();
        public static void PlayFemurBrakerSound() => ReferenceHub.HostHub.playerInteract.RpcContain106(null);
        public static AlphaWarheadController Warhead => AlphaWarheadController.Host;
        public static WorkstationController SpawnWorkStation(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            var bench = UnityEngine.Object.Instantiate(NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == "Work Station"));
            bench.gameObject.transform.localScale = scale;
            bench.gameObject.transform.position = position;
            bench.gameObject.transform.rotation = Quaternion.Euler(rotation);

            NetworkServer.Spawn(bench);
            WorkStations.Add(bench.GetComponent<WorkstationController>());

            return bench.GetComponent<WorkstationController>();
        }
    }
}
