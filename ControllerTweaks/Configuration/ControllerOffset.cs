namespace ControllerTweaks.Configuration
{
    public class ControllerOffset
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

        public ControllerOffset()
        {
        }

        public ControllerOffset(Vector3SO positionOffset, Vector3SO rotationOffset)
        {
            positionX = positionOffset.value.x;
            positionY = positionOffset.value.y;
            positionZ = positionOffset.value.z;
            rotationX = rotationOffset.value.x;
            rotationY = rotationOffset.value.y;
            rotationZ = rotationOffset.value.z;
        }

        public enum OffsetType
        {
            PositionX,
            PositionY,
            PositionZ,
            RotationX,
            RotationY,
            RotationZ
        }

        public void SetValue(OffsetType offsetType, float value)
        {
            switch (offsetType)
            {
                case OffsetType.PositionX:
                    positionX = value;
                    break;
                case OffsetType.PositionY:
                    positionY = value;
                    break;
                case OffsetType.PositionZ:
                    positionZ = value;
                    break;
                case OffsetType.RotationX:
                    rotationX = value;
                    break;
                case OffsetType.RotationY:
                    rotationY = value;
                    break;
                case OffsetType.RotationZ:
                    rotationZ = value;
                    break;
            }
        }
    }
}
