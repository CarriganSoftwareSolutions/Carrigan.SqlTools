namespace Carrigan.SqlTools.Tags;
//TODO: Proof read Documentation
public abstract class SelectTagsBase
{
    /// <summary>
    /// Get all SelectTags associated with the instance, as an Enumeration.
    /// For SelectTag this will just be itself as an IEnumerable.
    /// </summary>
    /// <returns>
    /// All SelectTags associated with the instance, as a string. 
    /// For SelectTag this will just be itself.
    /// </returns>
    public abstract IEnumerable<SelectTag> All();

    /// <summary>
    /// Get all SelectTags associated with the instance, as a string.
    /// For SelectTag this will just be itself.
    /// For SelectTags this will be a comma separated list.
    /// </summary>
    /// <returns>
    /// All SelectTags associated with the instance, as a string. 
    /// For SelectTag this will just be itself.
    /// For SelectTags this will be a comma separated list.
    /// </returns>
    public abstract string ToSql();
    /// <summary>
    /// Get all TableTags associated with the instance.
    /// For SelectTag this will just it's TableTag as an Enumerable.
    /// For SelectTags this will be multiple TableTags.
    /// </summary>
    /// <returns>
    /// All TableTags associated with the instance.
    /// For SelectTag this will just it's TableTag as an Enumerable.
    /// For SelectTags this will be multiple TableTags.
    /// </returns>
    internal abstract IEnumerable<TableTag> GetTableTags();

    /// <summary>
    /// Determines if this instance contains any actual SelectTags
    /// For SelectTag, this should always be true.
    /// For SelectTags, this will be true if the underlying Enumeration is not empty.
    /// </summary>
    /// <returns>
    /// True if this instance contains any actual SelectTags
    /// For SelectTag, this should always be true.
    /// For SelectTags, this will be true if the underlying Enumeration is not empty.
    /// </returns>
    public abstract bool Any();

    /// <summary>
    /// Determines if this instance contains no SelectTags
    /// For SelectTag, this should always be false.
    /// For SelectTags, this will be true if the underlying Enumeration is empty.
    /// </summary>
    /// <returns>
    /// Determines if this instance contains no SelectTags
    /// For SelectTag, this should always be false.
    /// For SelectTags, this will be true if the underlying Enumeration is empty.
    /// </returns>
    public abstract bool Empty();
}
