// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.ChangeTracking
{
    /// <summary>
    ///     <para>
    ///         Provides access to change tracking and loading information for a reference (i.e. non-collection)
    ///         navigation property that associates this entity to another entity.
    ///     </para>
    ///     <para>
    ///         Instances of this class are returned from methods when using the <see cref="ChangeTracker" /> API and it is
    ///         not designed to be directly constructed in your application code.
    ///     </para>
    /// </summary>
    public class ReferenceEntry : NavigationEntry
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [EntityFrameworkInternal]
        public ReferenceEntry([NotNull] InternalEntityEntry internalEntry, [NotNull] string name)
            : base(internalEntry, name, collection: false)
        {
            LocalDetectChanges();
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [EntityFrameworkInternal]
        public ReferenceEntry([NotNull] InternalEntityEntry internalEntry, [NotNull] INavigation navigation)
            : base(internalEntry, navigation)
        {
            LocalDetectChanges();
        }

        private void LocalDetectChanges()
        {
            if (!Metadata.IsDependentToPrincipal())
            {
                var target = GetTargetEntry();
                if (target != null)
                {
                    var context = InternalEntry.StateManager.Context;
                    if (context.ChangeTracker.AutoDetectChangesEnabled
                        && context.Model[ChangeDetector.SkipDetectChangesAnnotation] == null)
                    {
                        context.GetDependencies().ChangeDetector.DetectChanges(target);
                    }
                }
            }
        }

        /// <summary>
        ///     The <see cref="EntityEntry" /> of the entity this navigation targets.
        /// </summary>
        /// <value> An entry for the entity that this navigation targets. </value>
        public virtual EntityEntry TargetEntry
        {
            get
            {
                var target = GetTargetEntry();
                return target == null ? null : new EntityEntry(target);
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [EntityFrameworkInternal]
        protected virtual InternalEntityEntry GetTargetEntry()
            => CurrentValue == null
                ? null
                : InternalEntry.StateManager.GetOrCreateEntry(CurrentValue, Metadata.GetTargetType());
    }
}
