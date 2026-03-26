using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CurrenciesLib.ConversionProviders
{
	/// <summary>
	/// Graph-based conversion provider that supports multi-hop currency conversion via BFS shortest path.
	/// Replaces CacheConversionProvider and ToDefaultCurrencyConversionProvider.
	/// </summary>
	public class RateGraph : ConversionProvider
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(RateGraph));

		readonly object sync = new object();

		/// <summary>
		/// Gets or sets the time after which an edge is no longer valid.
		/// </summary>
		public ulong CacheExpirationMillis { get; set; }

		/// <summary>
		/// Adjacency list: adjacency[from][to] = edge data
		/// </summary>
		readonly Dictionary<Currencies, Dictionary<Currencies, RateEdge>> adjacency = new Dictionary<Currencies, Dictionary<Currencies, RateEdge>>();

		/// <summary>
		/// Returns all direct (non-computed) edges as TimedQuotes, for API consumers.
		/// </summary>
		public List<TimedQuote> Quotes
		{
			get
			{
				lock (sync)
				{
					return adjacency.Values
						.SelectMany(inner => inner.Values)
						.Select(e => new TimedQuote
						{
							BaseCurrency = e.From,
							QuoteCurrency = e.To,
							Midpoint = e.Midpoint,
							SpreadBuy = e.SpreadBuy,
							SpreadSell = e.SpreadSell,
							UpdatedAtUTC = e.UpdatedAtUTC,
							IsInferred = e.IsInferred
						})
						.ToList();
				}
			}
		}

		public RateGraph(ulong cacheExpMillis = ConversionProviderFactory.DEFAULT_QUOTE_EXPIRATION_MILLIS)
		{
			CacheExpirationMillis = cacheExpMillis;
		}

		/// <summary>
		/// Adds or updates a rate edge and optionally its reverse.
		/// Same signature as CacheConversionProvider.UpdateCache for backward compatibility.
		/// </summary>
		public void UpdateCache(Quote quote, DateTime updatedAtUtc, bool inferOpposite = true)
		{
			if (quote.Midpoint == 0) return;

			lock (sync)
			{
				SetEdge(new RateEdge
				{
					From = quote.BaseCurrency,
					To = quote.QuoteCurrency,
					Midpoint = quote.Midpoint,
					SpreadBuy = quote.SpreadBuy,
					SpreadSell = quote.SpreadSell,
					UpdatedAtUTC = updatedAtUtc,
					IsInferred = quote.IsInferred
				});

				if (inferOpposite)
				{
					SetEdge(new RateEdge
					{
						From = quote.QuoteCurrency,
						To = quote.BaseCurrency,
						Midpoint = 1 / quote.Midpoint,
						SpreadBuy = quote.SpreadSell,
						SpreadSell = quote.SpreadBuy,
						UpdatedAtUTC = updatedAtUtc,
						IsInferred = quote.IsInferred
					});
				}
			}
		}

		public override bool CanConvert(Currencies source, Currencies dest)
		{
			return base.CanConvert(source, dest);
		}

		protected override TimedQuote getQuote(Currencies source, Currencies dest)
		{
			lock (sync)
			{
				// 1. Direct edge (O(1))
				if (TryGetFreshEdge(source, dest, out var direct))
				{
					var q = ToTimedQuote(direct, source, dest);
					log.Debug($"RateGraph direct: {q.BaseCurrency}/{q.QuoteCurrency} {q.Midpoint:#,##0.##}");
					return q;
				}

				// 2. BFS shortest path
				var path = FindShortestPath(source, dest);
				if (path == null)
				{
					log.Debug($"RateGraph no path: {source}/{dest}");
					return null;
				}

				// 3. Compound along path
				var result = CompoundPath(path, source, dest);
				log.Debug($"RateGraph {path.Count}-hop: {result.BaseCurrency}/{result.QuoteCurrency} {result.Midpoint:#,##0.##}, sbuy: {result.SpreadBuy:0.00}, ssell: {result.SpreadSell:0.00}");
				return result;
			}
		}

		public void InvalidateQuote(Quote quote)
		{
			lock (sync)
			{
				SetEdge(new RateEdge
				{
					From = quote.BaseCurrency,
					To = quote.QuoteCurrency,
					Midpoint = quote.Midpoint,
					SpreadBuy = quote.SpreadBuy,
					SpreadSell = quote.SpreadSell,
					UpdatedAtUTC = DateTime.MinValue,
					IsInferred = quote.IsInferred
				}, forceUpdate: true);

				SetEdge(new RateEdge
				{
					From = quote.QuoteCurrency,
					To = quote.BaseCurrency,
					Midpoint = 1 / quote.Midpoint,
					SpreadBuy = quote.SpreadSell,
					SpreadSell = quote.SpreadBuy,
					UpdatedAtUTC = DateTime.MinValue,
					IsInferred = quote.IsInferred
				}, forceUpdate: true);

			}
		}
		#region Private helpers (all called under lock)

		/// <summary>
		/// Adds a new edge or updates an existing edge in the adjacency map for currency rates.
		/// </summary>
		/// <remarks>If an edge between the specified currencies does not exist, it is added. If an edge already
		/// exists, it is updated based on the <paramref name="forceUpdate"/> flag and the timestamp of the new
		/// edge.</remarks>
		/// <param name="edge">The rate edge to add or update. Contains the source and destination currencies, rate information, and metadata.</param>
		/// <param name="forceUpdate">If set to <see langword="true"/>, updates the edge even if the existing edge is more recent. If <see
		/// langword="false"/>, only updates if the new edge is more recent.</param>
		private void SetEdge(RateEdge edge, bool forceUpdate = false)
		{
			if (!adjacency.TryGetValue(edge.From, out var neighbors))
			{
				neighbors = new Dictionary<Currencies, RateEdge>();
				adjacency[edge.From] = neighbors;
			}

			if (neighbors.TryGetValue(edge.To, out var existing))
			{
				if (forceUpdate || existing.UpdatedAtUTC <= edge.UpdatedAtUTC)
				{
					existing.Midpoint = edge.Midpoint;
					existing.SpreadBuy = edge.SpreadBuy;
					existing.SpreadSell = edge.SpreadSell;
					existing.UpdatedAtUTC = edge.UpdatedAtUTC;
					existing.IsInferred = edge.IsInferred;
				}
			}
			else
			{
				neighbors[edge.To] = edge;
			}
		}

		private bool TryGetFreshEdge(Currencies from, Currencies to, out RateEdge edge)
		{
			edge = null;
			if (!adjacency.TryGetValue(from, out var neighbors)) return false;
			if (!neighbors.TryGetValue(to, out edge)) return false;
			return edge.UpdatedAtUTC.AddMilliseconds(CacheExpirationMillis) >= DateTime.UtcNow;
		}

		private List<RateEdge> FindShortestPath(Currencies source, Currencies dest)
		{
			// BFS
			var visited = new HashSet<Currencies> { source };
			// Each entry: (current currency, path of edges to get here)
			var queue = new Queue<(Currencies node, List<RateEdge> path)>();
			queue.Enqueue((source, new List<RateEdge>()));

			while (queue.Count > 0)
			{
				var (current, path) = queue.Dequeue();

				if (!adjacency.TryGetValue(current, out var neighbors))
					continue;

				foreach (var kvp in neighbors)
				{
					var next = kvp.Key;
					var edge = kvp.Value;

					if (visited.Contains(next)) continue;
					if (edge.UpdatedAtUTC.AddMilliseconds(CacheExpirationMillis) < DateTime.UtcNow) continue; // stale

					var newPath = new List<RateEdge>(path) { edge };

					if (next == dest)
						return newPath;

					visited.Add(next);
					queue.Enqueue((next, newPath));
				}
			}

			return null; // no path found
		}

		private static TimedQuote CompoundPath(List<RateEdge> path, Currencies source, Currencies dest)
		{
			decimal midpoint = 1m;
			decimal spreadBuyCompound = 1m;
			decimal spreadSellCompound = 1m;
			DateTime oldestUpdate = DateTime.MaxValue;

			foreach (var edge in path)
			{
				midpoint *= edge.Midpoint;
				spreadBuyCompound *= (1 - edge.SpreadBuy);
				spreadSellCompound *= (1 - edge.SpreadSell);
				if (edge.UpdatedAtUTC < oldestUpdate)
					oldestUpdate = edge.UpdatedAtUTC;
			}

			return new TimedQuote
			{
				BaseCurrency = source,
				QuoteCurrency = dest,
				Midpoint = midpoint,
				SpreadBuy = 1 - spreadBuyCompound,
				SpreadSell = 1 - spreadSellCompound,
				UpdatedAtUTC = oldestUpdate,
				IsInferred = path.Count > 1
			};
		}

		private static TimedQuote ToTimedQuote(RateEdge edge, Currencies source, Currencies dest)
		{
			return new TimedQuote
			{
				BaseCurrency = source,
				QuoteCurrency = dest,
				Midpoint = edge.Midpoint,
				SpreadBuy = edge.SpreadBuy,
				SpreadSell = edge.SpreadSell,
				UpdatedAtUTC = edge.UpdatedAtUTC,
				IsInferred = edge.IsInferred
			};
		}

		#endregion

		/// <summary>
		/// Internal edge representation for the rate graph.
		/// </summary>
		class RateEdge
		{
			public Currencies From { get; set; }
			public Currencies To { get; set; }
			public decimal Midpoint { get; set; }
			public decimal SpreadBuy { get; set; }
			public decimal SpreadSell { get; set; }
			public DateTime UpdatedAtUTC { get; set; }
			public bool IsInferred { get; set; }
		}
	}
}
