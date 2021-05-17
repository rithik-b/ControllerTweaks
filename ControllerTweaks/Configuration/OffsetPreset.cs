namespace ControllerTweaks.Configuration
{
    public class OffsetPreset
    {
        public virtual float positionX { get; set; }
        public virtual float positionY { get; set; }
        public virtual float positionZ { get; set; }
        public virtual float rotationX { get; set; }
        public virtual float rotationY { get; set; }
        public virtual float rotationZ { get; set; }

        public override string ToString()
        {
            return string.Format("Position: ({0:0.#}, {1:0.#}, {2:0.#}), Angle: ({3}, {4}, {5})", positionX * 100f, positionY * 100f, positionZ * 100f, rotationX, rotationY, rotationZ);
        }
    }
}
