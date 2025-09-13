namespace Carrigan.SqlTools.Attributes;


/// <summary>
/// A data annotation to instruct Entity Framework to generate a class as a stored procedure.
/// This should have been part of Carrigan.SqlTools
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[Obsolete("Do not use until I fix procedures")]
public class ProcedureAttribute : Attribute
{
    /// <summary>
    /// Public getter to indicate the name of the stored procedures.
    /// </summary>
    public string Name { get; }
    public string Schema { get; }

    public ProcedureAttribute(string Name, string Schema = "")
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new ArgumentException("Procedure name cannot be null or empty.", nameof(Name));
        }
        this.Name = Name;
        this.Schema = Schema;
    }
}
