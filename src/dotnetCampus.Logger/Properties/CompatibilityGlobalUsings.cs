global using dotnetCampus.Logging.Properties;

// .NET 8.0 or later

#if NET8_0_OR_GREATER
global using ImmutableArrayILogger = System.Collections.Immutable.ImmutableArray<dotnetCampus.Logging.ILogger>;
global using ImmutableHashSetString = System.Collections.Immutable.ImmutableHashSet<string>;
#else
global using ImmutableArrayILogger = System.Collections.Generic.List<dotnetCampus.Logging.ILogger>;
global using ImmutableHashSetString = System.Collections.Generic.HashSet<string>;
#endif

// .NET 6.0 or later
#if NET6_0_OR_GREATER
global using System.Collections.Immutable;
global using Math = System.Math;

#else
global using Math = dotnetCampus.Logging.Properties.Compatibility;
#endif
