// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Engine
{
    using System;
    using SharpKml.Dom;

    /// <summary>
    /// Provides extension methods for <see cref="Update"/> objects.
    /// </summary>
    public static class UpdateExtensions
    {
        /// <summary>
        /// Provides in-place (destructive) processing of the <see cref="Update"/>.
        /// </summary>
        /// <param name="update">The update instance.</param>
        /// <param name="file">
        /// A KmlFile containing the <c>Update</c> and the update targets.
        /// </param>
        /// <exception cref="ArgumentNullException">file is null.</exception>
        public static void Process(this Update update, KmlFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            foreach (Element child in update.Updates)
            {
                if (child is ChangeCollection change)
                {
                    ProcessChange(change, file);
                    continue;
                }

                if (child is CreateCollection create)
                {
                    ProcessCreate(create, file);
                    continue;
                }

                if (child is DeleteCollection delete)
                {
                    ProcessDelete(delete, file);
                }
            }
        }

        private static void ProcessChange(ChangeCollection change, KmlFile file)
        {
            foreach (KmlObject source in change)
            {
                if (source.TargetId != null)
                {
                    KmlObject target = file.FindObject(source.TargetId);
                    if (target != null)
                    {
                        target.Merge(source);
                        target.TargetId = null; // Merge copied the TargetId from the source, but target shouldn't have it set
                    }
                }
            }
        }

        private static void ProcessCreate(CreateCollection create, KmlFile file)
        {
            foreach (Container source in create)
            {
                if (source.TargetId != null)
                {
                    // Make sure it was found and that the target was a Container
                    if (file.FindObject(source.TargetId) is Container target)
                    {
                        foreach (Feature feature in source.Features)
                        {
                            Feature clone = feature.Clone(); // We never give the original source.
                            target.AddFeature(clone);
                            file.AddFeature(clone);
                        }
                    }
                }
            }
        }

        private static void ProcessDelete(DeleteCollection delete, KmlFile file)
        {
            foreach (Feature source in delete)
            {
                if (source.TargetId != null)
                {
                    if (file.FindObject(source.TargetId) is Feature feature)
                    {
                        // Remove the Feature from the parent, which is either
                        // a Container or Kml
                        if (feature.Parent is Container container)
                        {
                            container.RemoveFeature(source.TargetId);
                        }
                        else
                        {
                            if (feature.Parent is Kml kml)
                            {
                                kml.Feature = null;
                            }
                        }

                        // Also remove it from the file
                        file.RemoveFeature(feature);
                    }
                }
            }
        }
    }
}
