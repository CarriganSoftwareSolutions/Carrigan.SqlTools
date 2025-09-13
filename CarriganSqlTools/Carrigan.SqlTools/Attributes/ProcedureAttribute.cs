namespace Carrigan.SqlTools.Attributes;


/// <summary>
/// A data annotation to instruct Entity Framework to generate a class as a stored procedure.
/// This should have been part of Carrigan.SqlTools
/// ToDo: Move to Carrigan.SqlTools
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ProcedureAttribute : Attribute
{
    /// <summary>
    /// Public getter to indicate the name  of the stored procdures.
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
