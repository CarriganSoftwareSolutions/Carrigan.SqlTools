using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public abstract partial class SqlGeneratorBase<T>
{
    /// <summary>
    /// Generates a SQL <c>DELETE</c> statement for the specified entity,
    /// using only its key properties to identify the record to remove.
    /// </summary>
    /// <param name="entity">
    /// An instance of the data model that supplies the key property values
    /// used to locate the record to delete.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>DELETE</c> statement.
    /// </returns>
    /// <remarks>
    /// When generating SQL, only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="entity"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown if a key column lacks a <see cref="ParameterTag"/> during parameter generation.
    /// </exception>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new() { Id = 42 };
    /// SqlQuery query = customerGenerator.Delete(entity);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Customer] ([Customer].[Id] = @Id_1)
    /// ]]></code>
    /// </example>
    protected virtual SqlQuery BaseDelete(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Predicates predicates = new And
        (
            KeyColumnInfo.Select(columnInfo => GetColumnValue(columnInfo, entity))
        );
        return BaseDelete(null, null, predicates);
    }

    /// <summary>
    /// Generates a SQL <c>DELETE</c> statement that removes all rows
    /// from the table represented by the data model type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>DELETE</c> statement.
    /// </returns>
    /// <remarks>
    /// When building the SQL, only public properties with a getter are considered.
    /// </remarks>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// SqlQuery query = customerGenerator.DeleteAll();
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Customer];
    /// ]]></code>
    /// </example>
    protected virtual SqlQuery BaseDeleteAll()
    {
        static IEnumerable<ISqlFragment> GetFragments()
        {
            yield return new SqlFragmentText("DELETE FROM ");
            yield return Table;
            yield return ISqlFragment.Semicolon;
        }
        return new SqlQuery(Dialect, CommandType.Text, GetFragments());
    }

    /// <summary>
    /// Generates a SQL <c>DELETE</c> statement that deletes rows by primary key values.
    /// </summary>
    /// <param name="entities">
    /// A collection of entities used only as key-value holders. Only key properties are used.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>DELETE</c> statement.
    /// </returns>
    /// <remarks>
    /// When generating SQL, only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="entities"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NoPrimaryKeyPropertyException{T}">
    /// Thrown when <typeparamref name="T"/> has no key property metadata but a key-based delete was requested.
    /// </exception>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new() { Id = 42 };
    /// SqlQuery query = customerGenerator.DeleteById(entity);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Customer] WHERE ([Customer].[Id] = @Parameter_Id)
    /// ]]></code>
    /// </example>
    protected virtual SqlQuery BaseDeleteById(params IEnumerable<T> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (HasKeyProperty is false)
            throw new NoPrimaryKeyPropertyException<T>();
        else
            return BaseDelete(null, null, new Or(entities.Select(entity => GetByKeyPredicates(entity))));
    }

    /// <summary>
    /// Generates a SQL <c>DELETE</c> statement for the table represented by
    /// <typeparamref name="T"/>, with optional joins and filter predicates.
    /// </summary>
    /// <param name="usings"></param>
    /// <param name="joins">
    /// Optional <see cref="Joins"/> that specify related tables to join when forming the delete statement.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>DELETE</c> statement.
    /// </returns>
    /// <remarks>
    /// When generating SQL, only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="InvalidTableException">
    /// Thrown when a <see cref="TableTag"/> referenced by <paramref name="predicates"/> is not present
    /// in the <paramref name="joins"/> set nor equal to the primary table.
    /// </exception>
    /// <example>
    /// <para>Example with null Joins and null predicates</para>
    /// <code language="csharp"><![CDATA[
    /// SqlQuery query = customerGenerator.Delete(null, null);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Customer];
    /// ]]></code>
    /// </example>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// ColumnValue<Customer> columnValue = new(nameof(Customer.Name), "Hank");
    /// SqlQuery query = customerGenerator.Delete(null, columnValue);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Customer] 
    /// WHERE ([Customer].[Name] = @Parameter_Name)
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>Note: <see cref="ColumnEqualsColumnBase{leftT, righT}"/> validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// InnerJoin<Customer> join = new(predicate);
    /// 
    /// SqlQuery query = orderGenerator.Delete(join, null);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE [Order] 
    /// FROM [Order] 
    /// INNER JOIN [Customer] 
    /// ON ([Customer].[Id] = [Order].[CustomerId])
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>Note: ColumnEqualsColumn&lt;Customer, Order&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <para>Note: ColumnValues&lt;T&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// InnerJoin<Customer> join = new(predicate);
    /// 
    /// ColumnValue<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");
    /// 
    /// SqlQuery query = orderGenerator.Delete(join, customerEmail);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE [Order]
    /// FROM [Order] 
    /// INNER JOIN [Customer] 
    /// ON ([Customer].[Id] = [Order].[CustomerId]) 
    /// WHERE ([Customer].[Email] = @Parameter_Email)
    /// ]]></code>
    /// </example>
    /// <param name="predicates">
    /// Optional <see cref="Predicates"/> representing the <c>WHERE</c> conditions
    /// that determine which rows to delete.
    /// </param>
    protected virtual SqlQuery BaseDelete(IEnumerable<TableTag>? usings, JoinsBase? joins, Predicates? predicates)
    {
        IEnumerable<ISqlFragment> GetFragments()
        {
            yield return new SqlFragmentText("DELETE");
            if (joins.IsNotNullOrEmpty())
            {
                if (usings.IsNotNullOrEmpty())
                    yield return new SqlFragmentText(" FROM ");
                yield return ISqlFragment.Space;
                yield return Table;
            }
            if(usings.IsNotNullOrEmpty())
            {
                yield return new SqlFragmentText(" USING ");
                foreach (ISqlFragment fragment in usings.JoinFragments(", "))
                    yield return fragment;
            }
            else
            {
                yield return new SqlFragmentText(" FROM ");
                yield return Table;
            }

            if (joins?.IsNotNullOrEmpty() ?? false)
                foreach (ISqlFragment sqlFragment in joins.ToSqlFragments(Dialect))
                    yield return sqlFragment;

            if (predicates is not null)
            {
                yield return new SqlFragmentText(" WHERE ");
                foreach (ISqlFragment sqlFragment in predicates.ToSqlFragments(Dialect))
                    yield return sqlFragment;
            }
        }
        if (predicates is null && joins.IsNullOrEmpty())
        {
            return BaseDeleteAll();
        }
        else
        {
            IEnumerable<TableTag> selectTableTags = (usings ?? []).Prepend(Table).Concat(joins?.TableTags ?? []).Distinct();
            IEnumerable<TableTag> predicateTableTags = [.. predicates?.DescendantColumns?.Select(static col => col.TableTag)?.Distinct() ?? []];
            IEnumerable<TableTag> invalidTags = predicateTableTags.Except(selectTableTags);
            if (invalidTags.Any())
                throw new InvalidTableException(invalidTags);
            
            return new SqlQuery(Dialect, CommandType.Text, GetFragments());
        }
    }
}
