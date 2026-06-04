namespace Carrigan.SqlTools.Paging;

/// <summary>
/// Represents a paging strategy that uses the LIMIT and OFFSET clauses, commonly supported by databases like PostgreSQL and MySQL.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// LimitOffset limitOffset = new(25, 50);
/// SqlQuery query = customerGenerator.Select(null, null, null, null, null, null, limitOffset);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT "Customer".*
/// FROM "Customer"
/// ORDER BY "Customer"."Id" ASC
/// LIMIT 25 OFFSET 50
/// ]]></code>
/// </example>
///
/// <example>
/// <code language="csharp"><![CDATA[
/// LimitOffset limitOffset = new(25, 50);
/// OrderBy<Customer> orderBy = new(nameof(Customer.Name));
/// SqlQuery query = customerGenerator.Select(null, null, null, null, null, orderBy, limitOffset);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT "Customer".*
/// FROM "Customer"
/// ORDER BY
///     "Customer"."Name" ASC,
///     "Customer"."Id" ASC
/// LIMIT 25 OFFSET 50
/// ]]></code>
/// </example>
///
/// <example>
/// <code language="csharp"><![CDATA[
/// LimitOffset limitOffset = new(25, 50);
/// OrderBy<Customer> orderBy = new(nameof(Customer.Name), SortDirectionEnum.Descending);
/// SqlQuery query = customerGenerator.Select(null, null, null, null, null, orderBy, limitOffset);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT "Customer".*
/// FROM "Customer"
/// ORDER BY
///     "Customer"."Name" DESC,
///     "Customer"."Id" ASC
/// LIMIT 25 OFFSET 50
/// ]]></code>
/// </example>
///
/// <example>
/// <code language="csharp"><![CDATA[
/// LimitOffset limitOffset = new(25, 0);
/// SqlQuery query = customerGenerator.Select(null, null, null, null, null, null, limitOffset);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT "Customer".*
/// FROM "Customer"
/// ORDER BY "Customer"."Id" ASC
/// LIMIT 25
/// ]]></code>
/// </example>
///
/// <example>
/// <code language="csharp"><![CDATA[
/// LimitOffset limitOffset = new(1, 50);
/// SqlQuery query = customerGenerator.Select(null, null, null, null, null, null, limitOffset);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT "Customer".*
/// FROM "Customer"
/// ORDER BY "Customer"."Id"
/// ASC LIMIT 1 OFFSET 50
/// ]]></code>
/// </example>
public class LimitOffset : PagingBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LimitOffset"/> class.
    /// </summary>
    /// <param name="limit">The maximum number of rows to return.</param>
    /// <param name="offset">The number of rows to skip before starting to return rows.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="limit"/> is <c>0</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="offset"/> is <c>0</c>.
    /// </exception>
    public LimitOffset(uint limit, uint offset) : base(offset, limit)
    { }
}
