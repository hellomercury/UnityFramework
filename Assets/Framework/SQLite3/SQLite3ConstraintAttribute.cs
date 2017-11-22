using System;

namespace Framework.SQLite3
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SQLite3ConstraintAttribute : Attribute
    {
        private string constraint;
        public string Constraint { get { return constraint; } }

        public SQLite3ConstraintAttribute(SQLite3Constraint InConstraint)
        {
            constraint = string.Empty;
            if ((InConstraint & SQLite3Constraint.PrimaryKey) == SQLite3Constraint.PrimaryKey)
                constraint += "PRIMARY KEY ";
            if ((InConstraint & SQLite3Constraint.AutoIncrement) == SQLite3Constraint.AutoIncrement)
                constraint += "AUTOINCREMENT ";
            if ((InConstraint & SQLite3Constraint.Unique) == SQLite3Constraint.Unique)
                constraint += "UNIQUE ";
            if ((InConstraint & SQLite3Constraint.NotNull) == SQLite3Constraint.NotNull)
                constraint += "NOT NULL ";
        }
    }
}