using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Domain.ValueObjects;

public class DatePeriod : IEquatable<DatePeriod>
{
	public DateTime Start { get; }
	public DateTime? End { get; }

	private DatePeriod(DateTime start, DateTime end)
	{
		if (End is not null && Start > End)
			throw new ArgumentException("The first date can't be later than the second.", nameof(Start));
	}

	public static DatePeriod Create(DateTime start, DateTime end) => new(start, end);

	public override bool Equals(object? obj) => Equals(obj as DatePeriod);

	public bool Equals(DatePeriod? other) => other is not null && Start == other.Start && End == other.End;

	public override int GetHashCode() => End is not null ? HashCode.Combine(Start, End) : Start.GetHashCode();

	public override string ToString() => End is not null ? $"{Start:dd/MM/yyyy} - {End:dd/MM/yyyy}" : $"{Start:dd/MM/yyyy}";
}
