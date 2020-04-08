using Unity.Entities;
using Unity.Mathematics;

namespace UGUIDots.Events {
    
    public struct ResolutionEvent : IComponentData {
        public int2 Value;
    }
}
