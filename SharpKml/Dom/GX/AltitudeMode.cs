using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>
    /// Can be used instead of the OGC KML standard <see cref="SharpKml.Dom.AltitudeMode"/>.
    /// </summary>
    public enum AltitudeMode
    {
        /// <summary>
        /// Interprets the altitude as a value in meters above the sea floor.
        /// </summary>
        /// <remarks>
        /// If the KML feature is above land rather than sea, the altitude will
        /// be interpreted as being above the ground.
        /// </remarks>
        [KmlElement("clampToSeaFloor")]
        ClampToSeafloor = 0,

        /// <summary>
        /// The altitude specification is ignored, and the KML feature will be
        /// positioned on the sea floor.
        /// </summary>
        /// <remarks>
        /// If the KML feature is on land rather than at sea, ClampToSeaFloor
        /// will instead clamp to ground.
        /// </remarks>
        [KmlElement("relativeToSeaFloor")]
        RelativeToSeafloor
    }
}
