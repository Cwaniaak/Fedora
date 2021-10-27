using CommandSystem;
using MEC;
using Sapiox.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sapiox.Example
{
    public class SpawnFakePlayer : ICommand
    {
        public string Command { get; } = "SpawnFakePlayer";

        public string[] Aliases { get; } = new string[] { "spawnfakeplayer", "spawnfp", "spfakeplayer" };

        public string Description { get; } = "Spawn fake player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Server.GetPlayer(sender);
            var fp = new FakePlayer(player.Position, new Quaternion());
            Timing.RunCoroutine(Walk(player, fp));
            response = $"Spawned FakePlayer following the {player.NickName} !";
            return true;
        }

        private IEnumerator<float> Walk(Player _dood, FakePlayer _dummy)
        {
            for (; ; )
            {
                yield return Timing.WaitForSeconds(0.1f);
                try
                {
                    if (_dood == null)
                    { 
                        _dummy.DeSpawn();
                        continue;
                    }
                    _dummy.MoveState = PlayerMovementState.Walking;
                    _dummy.MoveDirection = Sapiox.API.Enum.MovementDirection.Forward;
                    _dummy.RotateToPosition(_dood.Position);
                    float distance = Vector3.Distance(_dood.Position, _dummy.Position);

                    if (distance <= 1.25f)
                        _dummy.MoveDirection = Sapiox.API.Enum.MovementDirection.None;
                }
                catch (Exception e)
                {
                    Log.Error($"Error:\n{e}");
                }
            }
        }
    }
}
