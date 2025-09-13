namespace Carrigan.SqlTools.Attributes;


/// <summary>
/// A data annotation to instruct Entity Framework to generate a class as a stored procedure.
/// This should have been part of Carrigan.SqlTools
/// </summary>
//TODO:I think the procedure logic is flawed. I think I failed to provide a way return a result with a different data model than procedure arguments.
//This occurred I wrote with for a procedure that had no return values, don't make that same mistake when rewriting.
[AttributeUsage(AttributeTargets.Class)]
[Obsolete("Do not use until I fix procedures")]
public class ProcedureAttribute : Attribute
{
    /// <summary>
    /// Public getter to indicate the name  of the stored procedures.
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
