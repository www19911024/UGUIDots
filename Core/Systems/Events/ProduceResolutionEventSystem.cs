using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Vella.Events;

namespace UGUIDots.Events.Systems {

    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class SpawnResolutionEventSystem : SystemBase {

        private int2 resolution;
        private EventQueue<ResolutionEvent> eventQueue;

        protected override void OnCreate() {
            resolution = new int2(Screen.width, Screen.height);

            eventQueue = World.GetOrCreateSystem<EntityEventSystem>().GetQueue<ResolutionEvent>();
        }

        protected override void OnUpdate() {
            var current = new int2(Screen.width, Screen.height);

            if (!current.Equals(resolution)) {
                eventQueue.Enqueue(new ResolutionEvent {
                    Value = current
                });

                resolution = current;
            }
        }
    }
}
