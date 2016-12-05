using System;

namespace Unwind
{
	public enum ConstraintType
	{
		Absolute,
		Relative
	}

	public enum Anchor
	{
		Left,
		Right,
		Bottom,
		Top
	}

	public class Constraint
	{
		public Canvas canvas { get; } // The canvas in which the constraint appiles.
		public Canvas attachment { get; } // The object that the constraint is bound between in the case of a relative constraint.
		public ConstraintType type { get; }
		public Anchor anchor { get; } // The edge of the canvas the constraint applies.
		public float amount; // The amount of the constraint. Either represents a proportion or an absolute value. May be negative or zero.
		public bool scaleFixed = true; // Defines whether the scale of the canvas can change with the constraint.

		/* Constructor for an absolute or relative constriant from the edge of a particular canvas. */
		private Constraint(Canvas parent, ConstraintType type, Anchor anchor, float amount)
		{
			this.canvas = parent;
			this.type = type;
			this.anchor = anchor;
			this.amount = amount;
			this.attachment = null;
		}

		/* Constructor for a relative constraint between an edge
		 of a canvas and another canvas or object (attachment). */
		private Constraint(Canvas parent, ConstraintType type, Anchor anchor, float amount, Canvas attachment)
		{
			this.canvas = parent;
			this.type = type;
			this.anchor = anchor;
			this.amount = amount;
			this.attachment = attachment;
		}

		public static Constraint CreateAbsolute(Canvas parent, Anchor anchor, float amount)
		{
			var constraint = new Constraint(parent, ConstraintType.Absolute, anchor, amount);
			return constraint;
		}

		public static Constraint CreateRelative(Canvas parent, Anchor anchor, float amount)
		{
			var constraint = new Constraint(parent, ConstraintType.Relative, anchor, amount);
			return constraint;
		}

		public static Constraint CreateRelative(Canvas parent, Anchor anchor, float amount, Canvas attachment)
		{
			var constraint = new Constraint(parent, ConstraintType.Relative, anchor, amount, attachment);
			return constraint;
		}
	}
}
