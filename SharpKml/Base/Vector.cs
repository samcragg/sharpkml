// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System;

    /// <summary>
    /// Represents a point in either 2D or, if <see cref="Altitude"/> is set, 3D space.
    /// </summary>
    public sealed class Vector : IEquatable<Vector>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> class.
        /// </summary>
        public Vector()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> class with
        /// the specified values.
        /// </summary>
        /// <param name="latitude">The latitude for this instance.</param>
        /// <param name="longitude">The longitude for this instance.</param>
        public Vector(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> class with
        /// the specified values.
        /// </summary>
        /// <param name="latitude">The latitude for this instance.</param>
        /// <param name="longitude">The longitude for this instance.</param>
        /// <param name="altitude">The altitude for this instance.</param>
        public Vector(double latitude, double longitude, double altitude)
        {
            this.Altitude = altitude;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Gets or sets the Altitude. Altitude can be null to indicate there
        /// is no altitude.
        /// </summary>
        public double? Altitude { get; set; }

        /// <summary>
        /// Gets or sets the Latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the Longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="left">The first vector to add.</param>
        /// <param name="right">The second vector to add.</param>
        /// <returns>The summed vector.</returns>
        public static Vector operator +(Vector left, Vector right)
        {
            return new Vector
            {
                Altitude = right.Altitude + left.Altitude,
                Longitude = right.Longitude + left.Longitude,
                Latitude = right.Latitude + left.Latitude
            };
        }

        /// <summary>
        /// Negates the specified vector.
        /// </summary>
        /// <param name="value">The vector to negate.</param>
        /// <returns>The negated vector.</returns>
        public static Vector operator -(Vector value)
        {
            return new Vector
            {
                Altitude = -value.Altitude,
                Longitude = -value.Longitude,
                Latitude = -value.Latitude
            };
        }

        /// <summary>
        /// Subtracts the second vector from the first.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>
        /// The vector that results from subtracting <c>right</c> from <c>left</c>.
        /// </returns>
        public static Vector operator -(Vector left, Vector right)
        {
            return left + (-right);
        }

        /// <summary>
        /// Multiples a vector by a scalar value.
        /// </summary>
        /// <param name="left">The vector.</param>
        /// <param name="right">The scalar value.</param>
        /// <returns>The scaled vector.</returns>
        public static Vector operator *(Vector left, double right)
        {
            return new Vector
            {
                Altitude = left.Altitude * right,
                Longitude = left.Longitude * right,
                Latitude = left.Latitude * right
            };
        }

        /// <summary>
        /// Determines whether this instance and the specified object have the
        /// same value.
        /// </summary>
        /// <param name="obj">
        /// An object, which must be a Vector, to compare to this instance.
        /// </param>
        /// <returns>
        /// true if the object is a Vector and the value of the object is the
        /// same as this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Vector);
        }

        /// <summary>
        /// Determines whether this instance and the specified Vector have the
        /// same value.
        /// </summary>
        /// <param name="other">The Vector to compare to this instance.</param>
        /// <returns>
        /// true if the location of the value parameter is the same as this
        /// instance; otherwise, false.
        /// </returns>
        public bool Equals(Vector other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.Altitude == other.Altitude) &&
                   (this.Latitude == other.Latitude) &&
                   (this.Longitude == other.Longitude);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            int value = this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode();
            if (this.Altitude.HasValue)
            {
                value ^= this.Altitude.GetHashCode();
            }

            return value;
        }
    }
}
